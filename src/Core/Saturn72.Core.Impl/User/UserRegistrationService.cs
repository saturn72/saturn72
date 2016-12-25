#region

using System;
using System.Threading.Tasks;
using Saturn72.Core.Caching;
using Saturn72.Core.Data.Repositories;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Security;
using Saturn72.Core.Services.User;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.User
{
    public class UserRegistrationService : DomainModelCrudServiceBase<UserDomainModel, long, long>,
        IUserRegistrationService
    {
        #region ctor

        public UserRegistrationService(IUserRepository userRepository, IEncryptionService encryptionService,
            UserSettings userSettings, IEventPublisher eventPublisher, IUserService userService,
            ICacheManager cacheManager, ITypeFinder typeFinder,
            IWorkContext<long> workContext, IWorkContext<long> webContext, IUserActivityLogRepository userActivityLogRepository) :
                base(userRepository, eventPublisher, cacheManager, typeFinder, workContext)
        {
            _encryptionService = encryptionService;
            _userSettings = userSettings;
            _userService = userService;
            _webContext = webContext;
            _userActivityLogRepository = userActivityLogRepository;
        }

        #endregion

        public async Task<UserRegistrationResponse> RegisterAsync(UserRegistrationRequest request)
        {
            //TODO: check request (bots, unapprovedIp's etc.
            var response = CheckUserRegistrationRequest(request);

            if (!response.Success)
                return response;

            EncryptPassword(request);

            var user = new UserDomainModel
            {
                Username = _userSettings.ValidateByEmail ? null : request.UsernameOrEmail,
                UserGuid = Guid.NewGuid(),
                Email = _userSettings.ValidateByEmail ? request.UsernameOrEmail : null,
                Password = request.Password,
                PasswordSalt = request.PasswordSalt,
                PasswordFormat = request.PasswordFormat,
                Active = _userSettings.ActivateUserAfterRegistration
            };
            await CreateAsync(user);
            EventPublisher.DomainModelCreated<UserDomainModel, long>(user);

            var regActLog = new UserActivityLogDomainModel
            {
                ActvityTypeCode = UserActivityType.Register.Code,
                ActivityDateUtc = DateTime.UtcNow,
                UserIpAddress = _webContext.CurrentUserIpAddress,
                UserGuid = user.UserGuid,
                ClientApp = _webContext.ClientId,
            };

            _userActivityLogRepository.AddUserActivityLog(regActLog);

            return response;
        }

        public bool ValidateUserByUsernameAndPassword(string usernameOrEmail, string password)
        {
            if (_userSettings.ValidateByEmail && !CommonHelper.IsValidEmail(usernameOrEmail))
                return false;

            var user = _userSettings.ValidateByEmail
                ? _userService.GetUserByEmail(usernameOrEmail)
                : _userService.GetUserByUsername(usernameOrEmail);
            if (user.IsNull() || !ValidatePassword(user, password))
                return false;

            //userActivityLog = new UserActivityLogDomainModel
            //{
            //    UserId = _webContext.CurrentUserId,
            //    ActvityTypeCode = UserActivityTypes.Login.Code,
            //    ActivityDateUtc = DateTime.UtcNow,
            //    UserIpAddress = _webContext.CurrentUserIpAddress
            //};

            //Task(() => _userActivityLogRepository.Add(userActivityLog));
            //TODO: Add user activity here 
            _userService.UpdateUser(user);

            return true;
        }

        private bool ValidatePassword(UserDomainModel user, string password)
        {
            switch (user.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    return password == user.Password;
                case PasswordFormat.Encrypted:
                    return user.Password == _encryptionService.EncryptText(password);
                case PasswordFormat.Hashed:
                    return user.Password ==
                           _encryptionService.CreatePasswordHash(password, user.PasswordSalt,
                               _userSettings.HashedPasswordFormat);
                default:
                    return false;
            }
        }

        private void EncryptPassword(UserRegistrationRequest request)
        {
            switch (request.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    return;
                case PasswordFormat.Encrypted:
                {
                    request.Password = _encryptionService.EncryptText(request.Password);
                    return;
                }
                case PasswordFormat.Hashed:
                {
                    var saltKey = _encryptionService.CreateSaltKey(5);
                    request.PasswordSalt = saltKey;
                    request.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey,
                        _userSettings.HashedPasswordFormat);
                }
                    return;
            }
        }

        private UserRegistrationResponse CheckUserRegistrationRequest(UserRegistrationRequest request)
        {
            var response = new UserRegistrationResponse();
            //Check username
            var usernameOrEmailNotEmpty = request.UsernameOrEmail.HasValue();
            if (!usernameOrEmailNotEmpty)
                response.AddError("Please specify user email or username");

            if (usernameOrEmailNotEmpty && !_userSettings.ValidateByEmail &&
                _userService.GetUserByUsername(request.UsernameOrEmail).NotNull())
                response.AddError("Username already exists");

            if (usernameOrEmailNotEmpty && _userSettings.ValidateByEmail &&
                _userService.GetUserByEmail(request.UsernameOrEmail).NotNull())
                response.AddError("Email already exists");

            return response;
        }

        #region Fields

        private readonly IEncryptionService _encryptionService;
        private readonly IUserService _userService;
        private readonly UserSettings _userSettings;
        private readonly IWorkContext<long> _webContext;
        private readonly IUserActivityLogRepository _userActivityLogRepository;

        #endregion
    }
}