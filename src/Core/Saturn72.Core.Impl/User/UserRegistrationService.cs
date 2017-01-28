#region

using System;
using System.Threading.Tasks;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Security;
using Saturn72.Core.Services.User;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.User
{
    public class UserRegistrationService : IUserRegistrationService
    {
        #region ctor

        public UserRegistrationService(IUserRepository userRepository, IEncryptionService encryptionService,
            UserSettings userSettings, IEventPublisher eventPublisher, IUserService userService,
            ICacheManager cacheManager, IUserActivityLogService userActivityLogService, AuditHelper auditHelper)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _userSettings = userSettings;
            _eventPublisher = eventPublisher;
            _userService = userService;
            _cacheManager = cacheManager;
            _userActivityLogService = userActivityLogService;
            _auditHelper = auditHelper;
        }

        #endregion

        public async Task<UserRegistrationResponse> RegisterAsync(UserRegistrationRequest request)
        {
            //TODO: check request (bots, unapprovedIp's etc.
            var response = CheckUserRegistrationRequest(request);

            if (!response.Success)
                return response;

            EncryptPassword(request);

            var user = new UserModel
            {
                Username = _userSettings.ValidateByEmail ? null : request.UsernameOrEmail,
                UserGuid = Guid.NewGuid(),
                Email = _userSettings.ValidateByEmail ? request.UsernameOrEmail : null,
                Password = request.Password,
                PasswordSalt = request.PasswordSalt,
                PasswordFormat = request.PasswordFormat,
                Active = _userSettings.ActivateUserAfterRegistration
            };
            _auditHelper.PrepareForCreateAudity(user);
            await Task.Run(() => _userRepository.Create(user));
            await _userActivityLogService.AddUserActivityLogAsync(UserActivityType.Register, user);

            _cacheManager.RemoveByPattern(SystemSharedCacheKeys.AllUserCacheKey);
            _eventPublisher.DomainModelCreated(user);

            return response;
        }

        public async Task<bool> ValidateUserByUsernameAndPasswordAsync(string usernameOrEmail, string password)
        {
            if (_userSettings.ValidateByEmail && !CommonHelper.IsValidEmail(usernameOrEmail))
                return false;

            var user = await (_userSettings.ValidateByEmail
                ? _userService.GetUserByEmail(usernameOrEmail)
                : _userService.GetUserByUsername(usernameOrEmail));
            if (user.IsNull() || !ValidatePassword(user, password))
                return false;
            return true;
        }

        private bool ValidatePassword(UserModel user, string password)
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

        private readonly IUserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IUserService _userService;
        private readonly ICacheManager _cacheManager;
        private readonly UserSettings _userSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IUserActivityLogService _userActivityLogService;
        private readonly AuditHelper _auditHelper;

        #endregion
    }
}