using Dapper;
using DotNetApiStarterKit.Data.Database;
using DotNetApiStarterKit.Data.Interfaces;
using DotNetApiStarterKit.Domain.Commons.Pagination;
using DotNetApiStarterKit.Domain.Entities.Auths;
using Microsoft.Extensions.Logging;

namespace DotNetApiStarterKit.Data.Repositories
{
    public class OtpRepository : IOtpRepository
    {
        private readonly SystemDbContext _dbContext;
        private readonly ILogger<OtpRepository> _logger;

        public OtpRepository(SystemDbContext dbContext, ILogger<OtpRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region CRUD Operations

        public async Task<int> AddAsync(Otp otp)
        {
            const string sql = @"
                INSERT INTO Otps
                (
                    Email,
                    Code,
                    ExpiresAt,
                    IsUsed,
                    IsActive,
                    CreatedAt
                )
                VALUES
                (
                    @Email,
                    @Code,
                    @ExpiresAt,
                    @IsUsed,
                    @IsActive,
                    @CreatedAt
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            otp.CreatedAt = otp.CreatedAt == default
                ? DateTime.UtcNow
                : otp.CreatedAt;

            using var connection = _dbContext.CreateConnection();

            var otpId = await connection.QuerySingleAsync<int>(sql, otp);

            _logger.LogInformation(
                "New OTP created with Id {OtpId} for Email {Email}",
                otpId, otp.Email);

            return otpId;
        }

        public async Task<PagedResult<Otp>> GetAllAsync(OtpQueryParams query)
        {
            ArgumentNullException.ThrowIfNull(query);
            var sortBy = query.SortBy?.Trim().ToLowerInvariant();

            var orderBy = sortBy switch
            {
                "otpid" => "OtpId",
                "email" => "Email",
                "expiresat" => "ExpiresAt",
                "isused" => "IsUsed",
                "createdat" => "CreatedAt",
                _ => "OtpId"
            };

            var sortOrder = query.SortOrder?.Trim().ToLowerInvariant() switch
            {
                "desc" => "DESC",
                _ => "ASC"
            };

            var sql = $@"
                SELECT COUNT(*)
                FROM Otps
                WHERE
                    IsActive = 1
                    AND (@OtpId IS NULL OR OtpId = @OtpId)
                    AND (@Email IS NULL OR LOWER(Email) = LOWER(@Email))
                    AND (@IsUsed IS NULL OR IsUsed = @IsUsed)
                    AND (
                        @Search IS NULL
                        OR Email LIKE '%' + @Search + '%'
                    );

                SELECT *
                FROM Otps
                WHERE
                    IsActive = 1
                    AND (@OtpId IS NULL OR OtpId = @OtpId)
                    AND (@Email IS NULL OR LOWER(Email) = LOWER(@Email))
                    AND (@IsUsed IS NULL OR IsUsed = @IsUsed)
                    AND (
                        @Search IS NULL
                        OR Email LIKE '%' + @Search + '%'
                    )
                ORDER BY {orderBy} {sortOrder}
                OFFSET (@Page - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY;";

            using var connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@OtpId", query.OtpId);

            parameters.Add("@Email", string.IsNullOrWhiteSpace(query.Email)
                ? null
                : query.Email.Trim());

            parameters.Add("@IsUsed", query.IsUsed);

            parameters.Add("@Search", string.IsNullOrWhiteSpace(query.Search)
                ? null
                : query.Search.Trim());

            parameters.Add("@Page", query.Page);
            parameters.Add("@PageSize", query.PageSize);

            using var multi = await connection.QueryMultipleAsync(sql, parameters);

            var totalCount = await multi.ReadSingleAsync<int>();
            var data = (await multi.ReadAsync<Otp>()).ToList();

            return new PagedResult<Otp>
            {
                Data = data,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Otp?> GetByIdAsync(int otpId)
        {
            const string sql = @"
                SELECT TOP 1 *
                FROM Otps
                WHERE OtpId = @Id
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Otp>(
                sql,
                new { Id = otpId });
        }

        public async Task<bool> DeleteAsync(int otpId)
        {
            const string sql = @"
                UPDATE Otps
                SET IsActive = 0
                WHERE OtpId = @OtpId
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                sql,
                new { OtpId = otpId });

            if (rowsAffected > 0)
            {
                _logger.LogInformation(
                    "Soft deleted OTP with Id {OtpId}",
                    otpId);

                return true;
            }

            _logger.LogWarning(
                "Failed to soft delete OTP with Id {OtpId}",
                otpId);

            return false;
        }

        #endregion

        #region Otp Specific Operations

        public async Task<Otp?> GetByEmailAsync(string email)
        {
            const string sql = @"
                SELECT TOP 1 *
                FROM Otps
                WHERE Email = @Email
                  AND IsActive = 1
                ORDER BY CreatedAt DESC;";

            using var connection = _dbContext.CreateConnection();

            var otp = await connection.QueryFirstOrDefaultAsync<Otp>(
                sql,
                new { Email = email });

            if (otp == null)
            {
                _logger.LogWarning(
                    "No active OTP found for Email {Email}",
                    email);
            }

            return otp;
        }

        public async Task<Otp?> GetLatestValidOtpAsync(string email, string code)
        {
            const string sql = @"
                SELECT TOP 1 *
                FROM Otps
                WHERE Email = @Email
                  AND Code = @Code
                  AND IsUsed = 0
                  AND IsActive = 1
                  AND ExpiresAt > @Now
                ORDER BY CreatedAt DESC;";

            using var connection = _dbContext.CreateConnection();

            var otp = await connection.QueryFirstOrDefaultAsync<Otp>(
                sql,
                new
                {
                    Email = email,
                    Code = code,
                    Now = DateTime.UtcNow
                });

            if (otp == null)
            {
                _logger.LogWarning(
                    "No valid OTP found for Email {Email}",
                    email);
            }

            return otp;
        }

        public async Task<bool> MarkAsUsedAsync(int otpId)
        {
            const string sql = @"
                UPDATE Otps
                SET IsUsed = 1
                WHERE OtpId = @OtpId
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                sql,
                new { OtpId = otpId });

            if (rowsAffected > 0)
            {
                _logger.LogInformation(
                    "Marked OTP as used. Id {OtpId}",
                    otpId);

                return true;
            }

            _logger.LogWarning(
                "Failed to mark OTP as used. Id {OtpId}",
                otpId);

            return false;
        }

        public async Task<bool> InvalidatePreviousOtpsAsync(string email)
        {
            const string sql = @"
                UPDATE Otps
                SET IsActive = 0
                WHERE Email = @Email
                  AND IsUsed = 0
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                sql,
                new { Email = email });

            _logger.LogInformation(
                "Invalidated {Count} previous OTP(s) for Email {Email}",
                rowsAffected, email);

            return rowsAffected > 0;
        }

        public async Task<int> CountOtpRequestsAsync(string email, DateTime since)
        {
            const string sql = @"
        SELECT COUNT(*)
        FROM Otps
        WHERE Email = @Email
          AND CreatedAt >= @Since
          AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            return await connection.QuerySingleAsync<int>(
                sql,
                new
                {
                    Email = email,
                    Since = since
                });
        }

        #endregion
    }
}