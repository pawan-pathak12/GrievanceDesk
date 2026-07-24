using Microsoft.Extensions.Logging;
using SmartCollegeComplaintPortal.Business.DTOs.Otps;
using SmartCollegeComplaintPortal.Business.Interfaces.Auths;
using SmartCollegeComplaintPortal.Data.Interfaces;
using SmartCollegeComplaintPortal.Domain.Commons.Pagination;
using SmartCollegeComplaintPortal.Domain.Entities.Auths;
using System.Security.Cryptography;
using System.Text;

namespace SmartCollegeComplaintPortal.Business.Services.Auths
{
    public class OtpService : IOtpService
    {
        private readonly IOtpRepository _otpRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<OtpService> _logger;

        public OtpService(
            IOtpRepository otpRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            ILogger<OtpService> logger)
        {
            _otpRepository = otpRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Result> GenerateAsync(GenerateOtpDto dto)
        {
            return await GenerateAsync(dto, requireExistingUser: false);
        }

        public async Task<Result> GenerateForPasswordResetAsync(GenerateOtpDto dto)
        {
            return await GenerateAsync(dto, requireExistingUser: true);
        }

        private async Task<Result> GenerateAsync(GenerateOtpDto dto, bool requireExistingUser)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return Result.Failure("Email is required.");
            }

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAddressAsync(email);

            if (!requireExistingUser && user != null)
            {
                _logger.LogWarning("Registration OTP request blocked. User already exists: {Email}", email);
                return Result.Failure("User already exists.");
            }

            if (requireExistingUser && (user == null || !user.IsActive))
            {
                _logger.LogWarning("Password reset OTP request blocked. User not found or inactive: {Email}", email);
                return Result.NotFound("No active account was found for this email.");
            }

            if (await IsCoolDownActiveAsync(email))
            {
                _logger.LogWarning("OTP request blocked by cooldown. Email: {Email}", email);
                return Result.Failure("Please wait 60 seconds before requesting a new OTP.");
            }

            if (await IsRateLimitExceededAsync(email))
            {
                _logger.LogWarning("OTP request blocked by rate limit. Email: {Email}", email);
                return Result.Failure("Too many OTP requests. Please try again after 15 minutes.");
            }

            await _otpRepository.InvalidatePreviousOtpsAsync(email);

            var otpCode = await GenerateAndSaveOtpAsync(email);

            await _emailService.SendOtpEmailAsync(email, otpCode);

            _logger.LogInformation("{OtpFlow} OTP generated and sent. Email: {Email}", requireExistingUser ? "Password reset" : "Registration", email);

            return Result.Success();
        }

        public async Task<Result> VerifyAsync(VerifyOtpDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Code))
            {
                return Result.Failure("Email and code are required.");
            }

            var hashedCode = HashOtp(dto.Code);

            var otp = await _otpRepository.GetLatestValidOtpAsync(dto.Email, hashedCode);
            if (otp == null)
            {
                _logger.LogWarning("OTP verification failed. Invalid or expired. Email: {Email}", dto.Email);
                return Result.Failure("Invalid or expired OTP.");
            }

            var marked = await _otpRepository.MarkAsUsedAsync(otp.OtpId);
            if (!marked)
            {
                _logger.LogWarning("Failed to mark OTP as used. OtpId: {OtpId}", otp.OtpId);
                return Result.Failure("OTP verification failed.");
            }

            _logger.LogInformation("OTP verified successfully. Email: {Email}", dto.Email);

            return Result.Success();
        }

        private static string HashOtp(string otp)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(otp);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private async Task<string> GenerateAndSaveOtpAsync(string email)
        {
            var rand = new Random();
            var otpCode = rand.Next(100000, 999999).ToString();

            var otp = new Otp
            {
                Email = email,
                Code = HashOtp(otpCode),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsActive = true,
                IsUsed = false
            };

            await _otpRepository.AddAsync(otp);

            return otpCode;
        }

        private async Task<bool> IsCoolDownActiveAsync(string email)
        {
            var lastOtp = await _otpRepository.GetByEmailAsync(email);
            if (lastOtp == null)
            {
                return false;
            }

            return (DateTime.UtcNow - lastOtp.CreatedAt).TotalSeconds < 60;
        }

        private async Task<bool> IsRateLimitExceededAsync(string email)
        {
            var fifteenMinutesAgo = DateTime.UtcNow.AddMinutes(-15);
            var count = await _otpRepository.CountOtpRequestsAsync(email, fifteenMinutesAgo);
            return count >= 5;
        }
    }
}
