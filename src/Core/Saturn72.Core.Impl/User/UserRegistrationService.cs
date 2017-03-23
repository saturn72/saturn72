﻿#region

using System;
using System.Threading.Tasks;
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

        public UserRegistrationService(IUserRepository userRepository, IEncryptionService encryptionService, UserSettings userSettings, IUserRegistrationRequestValidator registerRequestValidator, IEventPublisher eventPublisher, IUserService userService, IUserActivityLogService userActivityLogService, AuditHelper auditHelper, IWorkContext workContext)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _userSettings = userSettings;
            _registerRequestValidator = registerRequestValidator;
            _eventPublisher = eventPublisher;
            _userService = userService;
            _userActivityLogService = userActivityLogService;
            _auditHelper = auditHelper;
            _workContext = workContext;
        }

        #endregion

        public async Task<UserRegistrationResponse> RegisterAsync(UserRegistrationRequest request)
        {
            Guard.NotNull(request);
            var response = new UserRegistrationResponse();

            _registerRequestValidator.ValidateRequest(request).ForEachItem(err => response.AddError(err));

            if (!response.Success)
                return response;

            EncryptPassword(request);


            var user = new UserModel
            {
                Username = request.Username,
                UserGuid = Guid.NewGuid(),
                Email = request.Email,
                Password = request.Password,
                PasswordSalt = request.PasswordSalt,
                PasswordFormat = request.PasswordFormat,
                Active = _userSettings.ActivateUserAfterRegistration,
                LastIpAddress = _workContext.CurrentUserIpAddress,
                LastClientAppId = _workContext.ClientId
            };
            _auditHelper.PrepareForCreateAudity(user);
            await Task.Run(() => _userRepository.Create(user));
            await _userActivityLogService.AddUserActivityLogAsync(UserActivityType.UserRegistered, user);

            _eventPublisher.DomainModelCreated(user);

            return response;
        }

        public async Task<bool> ValidateUserByUsernameAndPasswordAsync(string usernameOrEmail, string password)
        {
            if (_userSettings.ValidateByEmail && !CommonHelper.IsValidEmail(usernameOrEmail))
                return false;

            var user = await (_userSettings.ValidateByEmail
                ? _userService.GetUserByEmail(usernameOrEmail)
                : _userService.GetUserByUsernameAsync(usernameOrEmail));
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

        #region Fields

        private readonly IUserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IUserService _userService;
        private readonly UserSettings _userSettings;
        private readonly IUserRegistrationRequestValidator _registerRequestValidator;
        private readonly IEventPublisher _eventPublisher;
        private readonly IUserActivityLogService _userActivityLogService;
        private readonly AuditHelper _auditHelper;
        private readonly IWorkContext _workContext;

        #endregion
    }
}