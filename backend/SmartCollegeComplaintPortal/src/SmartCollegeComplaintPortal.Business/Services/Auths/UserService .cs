using AutoMapper;
using DotNetApiStarterKit.Business.DTOs.Users;
using DotNetApiStarterKit.Business.Interfaces.Auths;
using DotNetApiStarterKit.Data.Interfaces;
using DotNetApiStarterKit.Domain.Commons.Pagination;
using DotNetApiStarterKit.Domain.Entities.Auths;
using DotNetApiStarterKit.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace DotNetApiStarterKit.Business.Services.Auths
{
    public class UserService : IUserService
    {
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private IEmailService _emailService;

        public UserService(IPasswordHasher<User> passwordHasher, IUserRepository userRepository, ILogger<UserService> logger, IMapper mapper, IEmailService emailService)
        {
            this.passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
            this._emailService = emailService;
        }
        #region CURD 
        public async Task<Result> CreateAsync(CreateUserDto dto)
        {
            _logger.LogInformation("Attempting to create new user with email: {Email}", dto.Email);

            var emailExists = await _userRepository.CheckEmailExistsAsync(dto.Email);
            if (emailExists)
            {
                _logger.LogWarning("User creation failed. Email already exists: {Email}", dto.Email);
                return Result.Failure($"Email already exists: {dto.Email}");
            }

            var user = _mapper.Map<User>(dto);
            user.Role = UserRole.User;
            user.IsActive = true;
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            var pas = passwordHasher.HashPassword(user, dto.Password);
            user.Password = pas;
            var userId = await _userRepository.AddAsync(user);

            if (userId <= 0)
            {
                _logger.LogWarning("User creation failed in repository. Email: {Email}", dto.Email);
                return Result.Failure("User creation failed.");
            }

            _logger.LogInformation("User created successfully. UserId: {UserId}", userId);

            var result = Result.Success();
            result.Id = userId;
            return result;
        }

        public async Task<Result> GetByIdAsync(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("GetById failed. Invalid UserId {UserId}", userId);
                return Result.Failure("Invalid user id.");
            }

            _logger.LogInformation("Fetching user with Id: {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found with Id: {UserId}", userId);
                return Result.NotFound($"User with ID {userId} not found.");
            }

            var responseDto = _mapper.Map<UserResponseDto>(user);
            return Result.Success(responseDto);
        }

        public async Task<Result> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("GetByEmail failed. Email is empty.");
                return Result.Failure("Email is required.");
            }

            _logger.LogInformation("Fetching user with email: {Email}", email);

            var user = await _userRepository.GetByEmailAddressAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User not found with email: {Email}", email);
                return Result.NotFound($"User with email '{email}' not found.");
            }

            var responseDto = _mapper.Map<UserResponseDto>(user);
            return Result.Success(responseDto);
        }

        public async Task<Result> GetAllAsync(UserQueryParams query)
        {
            _logger.LogInformation("Fetching all users");

            var users = await _userRepository.GetAllAsync(query);

            var responseDto = _mapper.Map<List<UserResponseDto>>(users.Data);

            var pagedResult = new PagedResult<UserResponseDto>
            {
                Data = responseDto,
                Page = users.Page,
                PageSize = users.PageSize,
                TotalCount = users.TotalCount
            };

            return Result.Success(pagedResult);
        }

        public async Task<Result> UpdateAsync(int userId, UpdateUserDto dto)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("User update failed. Invalid UserId {UserId}", userId);
                return Result.Failure("Invalid user id.");
            }

            if (userId != dto.UserId)
            {
                _logger.LogWarning(
                    "User update failed. Route id {RouteId} does not match body id {BodyId}",
                    userId, dto.UserId);

                return Result.Failure("Route ID and body ID do not match.");
            }

            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser == null)
            {
                _logger.LogWarning("User update failed. User with Id {UserId} not found", userId);
                return Result.NotFound($"User with ID {userId} not found.");
            }

            // Update only fields that are allowed to change from DTO
            existingUser.FirstName = dto.FirstName;
            existingUser.LastName = dto.LastName;
            existingUser.Email = dto.Email;
            existingUser.IsActive = dto.IsActive;

            var updated = await _userRepository.UpdateAsync(userId, existingUser);

            if (!updated)
            {
                _logger.LogWarning("User update failed in repository. UserId {UserId}", userId);
                return Result.Failure("Update failed.");
            }

            _logger.LogInformation("User with Id {UserId} updated successfully", userId);

            return Result.Success("User updated successfully.");
        }

        public async Task<Result> DeleteAsync(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("User delete failed. Invalid UserId {UserId}", userId);
                return Result.Failure("Invalid user id.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found for deletion. Id: {UserId}", userId);
                return Result.NotFound($"User with ID {userId} not found.");
            }

            if (user.Role == UserRole.Admin)
            {
                _logger.LogWarning("Attempt to delete an Admin user blocked. UserId {UserId}", userId);
                return Result.Failure("Cannot delete admin.");
            }

            var isDeleted = await _userRepository.DeleteAsync(userId);
            if (!isDeleted)
            {
                _logger.LogWarning("Failed to delete user with id {UserId}", userId);
                return Result.Failure($"Failed to delete user with id {userId}");
            }

            _logger.LogInformation("User with Id {UserId} deleted successfully", userId);

            return Result.Success();
        }

        public async Task<Result> CheckEmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Result.Failure("Email is required.");
            }

            var exists = await _userRepository.CheckEmailExistsAsync(email);
            return Result.Success(exists);
        }
        #endregion

        public async Task<Result> CreateUserByAdminAsync(
            CreateUserByAdminRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email))
            {
                return Result.Failure(
                    "First name, last name, and email are required.");
            }

            var email = request.Email.Trim().ToLowerInvariant();
            var firstName = request.FirstName.Trim();
            var lastName = request.LastName.Trim();

            _logger.LogInformation(
                "Admin is attempting to create a user with Email {Email}",
                email);

            var emailExists =
                await _userRepository.CheckEmailExistsAsync(email);

            if (emailExists)
            {
                _logger.LogWarning(
                    "Admin user creation failed. Email already exists: {Email}",
                    email);

                return Result.Failure(
                    $"A user with email {email} already exists.");
            }

            var temporaryPassword = GenerateTemporaryPassword();

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Role = UserRole.User,
                IsActive = true,

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            user.Password = passwordHasher.HashPassword(
                user,
                temporaryPassword);

            var userId = await _userRepository.AddAsync(user);

            if (userId <= 0)
            {
                _logger.LogWarning(
                    "Admin user creation failed in repository. Email: {Email}",
                    email);

                return Result.Failure("User creation failed.");
            }

            user.UserId = userId;

            var fullName = $"{firstName} {lastName}".Trim();


            await _emailService.SendTemporaryPasswordAsync(
                email,
                fullName,
                temporaryPassword);

            _logger.LogInformation(
                "User created by admin successfully. UserId: {UserId}, Email: {Email}",
                userId,
                email);

            var result = Result.Success(
                null,
                "User created successfully. The temporary password was sent by email.");

            result.Id = userId;

            return result;
        }

        #region Helper 
        private static string GenerateTemporaryPassword()
        {
            const string upperCase = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijkmnopqrstuvwxyz";
            const string digits = "23456789";
            const string specialCharacters = "!@#$%";
            const string allCharacters =
                upperCase + lowerCase + digits + specialCharacters;

            var characters = new List<char>
    {
        upperCase[RandomNumberGenerator.GetInt32(upperCase.Length)],
        lowerCase[RandomNumberGenerator.GetInt32(lowerCase.Length)],
        digits[RandomNumberGenerator.GetInt32(digits.Length)],
        specialCharacters[
            RandomNumberGenerator.GetInt32(specialCharacters.Length)]
    };

            while (characters.Count < 10)
            {
                characters.Add(
                    allCharacters[
                        RandomNumberGenerator.GetInt32(allCharacters.Length)]);
            }

            for (var index = characters.Count - 1; index > 0; index--)
            {
                var randomIndex =
                    RandomNumberGenerator.GetInt32(index + 1);

                (characters[index], characters[randomIndex]) =
                    (characters[randomIndex], characters[index]);
            }

            return new string(characters.ToArray());
        }
        #endregion

    }
}