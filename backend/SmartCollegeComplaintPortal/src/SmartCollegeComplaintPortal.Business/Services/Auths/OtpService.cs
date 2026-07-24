using DotNetApiStarterKit.Business.DTOs.Otps;
using DotNetApiStarterKit.Business.Interfaces.Auths;
using DotNetApiStarterKit.Data.Interfaces;
using DotNetApiStarterKit.Domain.Commons.Pagination;
using DotNetApiStarterKit.Domain.Entities.Auths;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace DotNetApiStarterKit.Business.Services.Auths
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
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return Result.Failure("Email is required.");
            }

            var user = await _userRepository.GetByEmailAddressAsync(dto.Email);
            if (user != null)
            {
                _logger.LogWarning("OTP request blocked. User already exists: {Email}", dto.Email);
                return Result.Failure("User already exists.");
            }

            if (await IsCoolDownActiveAsync(dto.Email))
            {
                _logger.LogWarning("OTP request blocked by cooldown. Email: {Email}", dto.Email);
                return Result.Failure("Please wait 60 seconds before requesting a new OTP.");
            }

            if (await IsRateLimitExceededAsync(dto.Email))
            {
                _logger.LogWarning("OTP request blocked by rate limit. Email: {Email}", dto.Email);
                return Result.Failure("Too many OTP requests. Please try again after 15 minutes.");
            }

            await _otpRepository.InvalidatePreviousOtpsAsync(dto.Email);

            var otpCode = await GenerateAndSaveOtpAsync(dto.Email);

            await _emailService.SendOtpEmailAsync(dto.Email, otpCode);

            _logger.LogInformation("OTP generated and sent. Email: {Email}", dto.Email);

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