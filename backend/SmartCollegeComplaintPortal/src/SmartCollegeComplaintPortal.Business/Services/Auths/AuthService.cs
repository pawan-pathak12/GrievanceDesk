using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartCollegeComplaintPortal.Business.DTOs.Auth;
using SmartCollegeComplaintPortal.Business.DTOs.Otps;
using SmartCollegeComplaintPortal.Business.DTOs.Users;
using SmartCollegeComplaintPortal.Business.Interfaces.Auths;
using SmartCollegeComplaintPortal.Data.Interfaces;
using SmartCollegeComplaintPortal.Domain.Commons.Pagination;
using SmartCollegeComplaintPortal.Domain.Entities.Auths;

namespace SmartCollegeComplaintPortal.Business.Services.Auths
{
    public class AuthService : IAuthService
    {
        private readonly IOtpService _otpService;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<AuthService> _logger;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(
            IOtpService otpService,
            IUserService userService,
            IUserRepository userRepository,
            ITokenService tokenService,
            ICurrentUserService currentUserService, IPasswordHasher<User> passwordHasher,
            ILogger<AuthService> logger)
        {
            _otpService = otpService;
            _userService = userService;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _currentUserService = currentUserService;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        #region Registration Flow

        public async Task<Result> CheckEmailAsync(CheckEmailDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return Result.Failure("Email is required.");
            }

            // GenerateAsync already checks for existing users and handles cooldown/rate-limit
            var result = await _otpService.GenerateAsync(new GenerateOtpDto { Email = dto.Email });

            return result;
        }

        public async Task<Result> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var result = await _otpService.VerifyAsync(dto);
            if (!result.IsSuccess)
            {
                return result;
            }

            var verificationToken = _tokenService.GenerateVerificationToken(dto.Email);

            _logger.LogInformation("OTP verified. Verification token issued for Email: {Email}", dto.Email);

            return Result.Success(new
            {
                verificationToken
            });
        }

        public async Task<Result> CreatePasswordAsync(CreatePasswordDto dto)
        {
            var verificationToken = dto.VerificationToken?.Trim();

            if (verificationToken?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
            {
                verificationToken = verificationToken.Substring("Bearer ".Length).Trim();
            }

            var isValidToken = _tokenService.TryValidateVerificationToken(verificationToken!, out var tokenEmail);

            if (!isValidToken || !string.Equals(tokenEmail, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Create password failed. Invalid or mismatched verification token. Email: {Email}", dto.Email);
                return Result.Unauthorized("Invalid or expired verification token.");
            }


            var createDto = new CreateUserDto
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password
            };

            var result = await _userService.CreateAsync(createDto);
            if (!result.IsSuccess)
            {
                return result;
            }

            _logger.LogInformation("User registration completed successfully. Email: {Email}", dto.Email);

            return Result.Success("Account created successfully.");
        }

        #endregion

        #region Login

        public async Task<AuthResult> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAddressAsync(dto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed. User not found. Email: {Email}", dto.Email);
                return AuthResult.Failure("Unable to sign in. Please check your credentials and try again.");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login blocked. User is inactive. Email: {Email}", dto.Email);
                return AuthResult.Failure("Unable to sign in. Please check your credentials and try again.");
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Login failed. Incorrect password. Email: {Email}", dto.Email);
                return AuthResult.Failure("Unable to sign in. Please check your credentials and try again.");
            }

            var accessToken = _tokenService.GenerateJwtToken(user);
            if (accessToken == null)
            {
                return AuthResult.Failure("Failed to issue token");
            }
            _logger.LogInformation("User logged in successfully. UserId: {UserId}", user.UserId);

            var result = AuthResult.Success();
            result.AccessToken = accessToken;
            return result;
        }

        #endregion

        #region Password Management

        public async Task<Result> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            var user = await _userRepository.GetByIdAsync(userId.Value);
            if (user == null)
            {
                _logger.LogWarning("Change password failed. User not found. UserId: {UserId}", userId.Value);
                return Result.NotFound("User not found.");
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.CurrentPassword);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Change password failed. Incorrect current password. UserId: {UserId}", userId.Value);
                return Result.Failure("The current password you entered is incorrect.");
            }

            var newHash = _passwordHasher.HashPassword(user, dto.NewPassword);

            var updated = await _userRepository.UpdatePasswordAsync(userId.Value, newHash);
            if (!updated)
            {
                _logger.LogWarning("Password update failed in repository. UserId: {UserId}", userId.Value);
                return Result.Failure("Password update failed. Please try again later.");
            }

            _logger.LogInformation("Password changed successfully. UserId: {UserId}", userId.Value);

            return Result.Success("Password changed successfully.");
        }

        #endregion

        #region User Info

        public async Task<Result> GetMeAsync()
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            return await _userService.GetByIdAsync(userId.Value);
        }

        #endregion
    }
}