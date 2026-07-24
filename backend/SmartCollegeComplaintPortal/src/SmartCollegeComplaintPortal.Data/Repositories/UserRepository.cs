using Dapper;
using Microsoft.Extensions.Logging;
using SmartCollegeComplaintPortal.Data.Database;
using SmartCollegeComplaintPortal.Data.Interfaces;
using SmartCollegeComplaintPortal.Domain.Commons.Pagination;
using SmartCollegeComplaintPortal.Domain.Entities.Auths;

namespace SmartCollegeComplaintPortal.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SystemDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(SystemDbContext dbContext, ILogger<UserRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region CRUD Operations

        public async Task<int> AddAsync(User user)
        {
            const string sql = @"
                INSERT INTO Users
                (
                    FirstName,
                    LastName,
                    Email,
                    Password,
                    Role,
                    IsActive,
                    CreatedAt,
                    UpdatedAt
                )
                VALUES
                (
                    @FirstName,
                    @LastName,
                    @Email,
                    @Password,
                    @Role,
                    @IsActive,
                    @CreatedAt,
                    @UpdatedAt
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            user.CreatedAt = user.CreatedAt == default
                ? DateTime.UtcNow
                : user.CreatedAt;

            using var connection = _dbContext.CreateConnection();

            var userId = await connection.QuerySingleAsync<int>(sql, user);

            _logger.LogInformation(
                "New user created with Id {UserId}",
                userId);

            return userId;
        }

        public async Task<PagedResult<User>> GetAllAsync(UserQueryParams query)
        {
            var sortBy = query.SortBy?.Trim().ToLowerInvariant();

            var orderBy = sortBy switch
            {
                "userid" => "UserId",
                "firstname" => "FirstName",
                "lastname" => "LastName",
                "email" => "Email",
                "role" => "Role",
                "isactive" => "IsActive",
                "createdat" => "CreatedAt",
                "updatedat" => "UpdatedAt",
                _ => "UserId"
            };

            var sortOrder = query.SortOrder?.Trim().ToLowerInvariant() switch
            {
                "desc" => "DESC",
                _ => "ASC"
            };

            var sql = $@"
                SELECT COUNT(*)
                FROM Users
                WHERE
                    (@UserId IS NULL OR UserId = @UserId)
                    AND (@Role IS NULL OR Role = @Role)
                    AND (@IsActive IS NULL OR IsActive = @IsActive)
                    AND (@Email IS NULL OR LOWER(Email) = LOWER(@Email))
                    AND (
                        @Search IS NULL
                        OR FirstName LIKE '%' + @Search + '%'
                        OR LastName LIKE '%' + @Search + '%'
                        OR Email LIKE '%' + @Search + '%'
                    );

                SELECT *
                FROM Users
                WHERE
                    (@UserId IS NULL OR UserId = @UserId)
                    AND (@Role IS NULL OR Role = @Role)
                    AND (@IsActive IS NULL OR IsActive = @IsActive)
                    AND (@Email IS NULL OR LOWER(Email) = LOWER(@Email))
                    AND (
                        @Search IS NULL
                        OR FirstName LIKE '%' + @Search + '%'
                        OR LastName LIKE '%' + @Search + '%'
                        OR Email LIKE '%' + @Search + '%'
                    )
                ORDER BY {orderBy} {sortOrder}
                OFFSET (@Page - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY;";

            using var connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@UserId", query.UserId);

            parameters.Add("@Role", query.Role.HasValue
                ? (int?)query.Role.Value
                : null);

            parameters.Add("@IsActive", query.IsActive);

            parameters.Add("@Email", string.IsNullOrWhiteSpace(query.Email)
                ? null
                : query.Email.Trim());

            parameters.Add("@Search", string.IsNullOrWhiteSpace(query.Search)
                ? null
                : query.Search.Trim());

            parameters.Add("@Page", query.Page);
            parameters.Add("@PageSize", query.PageSize);

            using var multi = await connection.QueryMultipleAsync(sql, parameters);

            var totalCount = await multi.ReadSingleAsync<int>();
            var data = (await multi.ReadAsync<User>()).ToList();

            return new PagedResult<User>
            {
                Data = data,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            const string sql = @"
                SELECT TOP 1 *
                FROM Users
                WHERE UserId = @Id
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<User>(
                sql,
                new { Id = userId });
        }

        public async Task<bool> UpdateAsync(int id, User user)
        {
            const string sql = @"
                UPDATE Users
                SET
                    FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    Role = @Role,
                    IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt
                WHERE UserId = @UserId;";

            user.UserId = id;
            user.UpdatedAt = DateTime.UtcNow;

            using var connection = _dbContext.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(sql, user);

            if (rowsAffected > 0)
            {
                _logger.LogInformation(
                    "Updated user with Id {UserId}",
                    id);

                return true;
            }

            _logger.LogWarning(
                "Failed to update user with Id {UserId}",
                id);

            return false;
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            const string sql = @"
                UPDATE Users
                SET
                    IsActive = 0,
                    UpdatedAt = @UpdatedAt
                WHERE UserId = @UserId
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                sql,
                new
                {
                    UserId = userId,
                    UpdatedAt = DateTime.UtcNow
                });

            if (rowsAffected > 0)
            {
                _logger.LogInformation(
                    "Soft deleted user with Id {UserId}",
                    userId);

                return true;
            }

            _logger.LogWarning(
                "Failed to soft delete user with Id {UserId}",
                userId);

            return false;
        }

        #endregion

        #region User Specific Operations

        public async Task<string?> GetUserFullNameAsync(int userId)
        {
            const string sql = @"
                SELECT FirstName, LastName
                FROM Users
                WHERE UserId = @UserId
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            var user = await connection.QueryFirstOrDefaultAsync<(string FirstName, string LastName)>(
                sql,
                new { UserId = userId });

            var fullName = $"{user.FirstName} {user.LastName}".Trim();

            return string.IsNullOrWhiteSpace(fullName)
                ? null
                : fullName;
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            const string sql = @"
                SELECT CASE
                    WHEN EXISTS
                    (
                        SELECT 1
                        FROM Users
                        WHERE Email = @Email
                          AND IsActive = 1
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                END;";

            using var connection = _dbContext.CreateConnection();

            var result = await connection.QuerySingleAsync<bool>(
                sql,
                new { Email = email });

            if (!result)
            {
                _logger.LogWarning(
                    "Email does not exist. Email: {Email}",
                    email);
            }

            return result;
        }

        public async Task<User?> GetByEmailAddressAsync(string? email)
        {
            const string sql = @"
                SELECT TOP 1 *
                FROM Users
                WHERE Email = @Email
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            var user = await connection.QueryFirstOrDefaultAsync<User>(
                sql,
                new { Email = email });

            if (user == null)
            {
                _logger.LogWarning(
                    "User email does not exist. Email: {Email}",
                    email);
            }

            return user;
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string newPassword)
        {
            const string sql = @"
                UPDATE Users
                SET
                    Password = @Password,
                    UpdatedAt = @UpdatedAt
                WHERE UserId = @UserId
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                sql,
                new
                {
                    UserId = userId,
                    Password = newPassword,
                    UpdatedAt = DateTime.UtcNow
                });

            if (rowsAffected > 0)
            {
                _logger.LogInformation(
                    "Updated password for user with Id {UserId}",
                    userId);

                return true;
            }

            _logger.LogWarning(
                "Failed to update password for user with Id {UserId}",
                userId);

            return false;
        }

        #endregion
    }
}