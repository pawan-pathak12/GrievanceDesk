using Dapper;
using Microsoft.Extensions.Logging;
using SmartCollegeComplaintPortal.Data.Database;
using SmartCollegeComplaintPortal.Data.Interfaces;
using SmartCollegeComplaintPortal.Data.QueryResult;
using SmartCollegeComplaintPortal.Domain.Entities.Complaints;

namespace SmartCollegeComplaintPortal.Data.Repositories
{
    public class ComplaintRepository : IComplaintRepository
    {
        private readonly SystemDbContext _dbContext;
        private readonly ILogger<ComplaintRepository> _logger;

        public ComplaintRepository(SystemDbContext dbContext, ILogger<ComplaintRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region COMPLAINT CRUD

        public async Task<int> CreateAsync(Complaint complaint)
        {
            const string sql = @"
                INSERT INTO Complaints
                (
                    UserId,
                    Title,
                    Description,
                    Category,
                    Status,
                    IsDuplicate,
                    IsEdited,
                    IsActive,
                    CreatedAt,
                    UpdatedAt
                )
                VALUES
                (
                    @UserId,
                    @Title,
                    @Description,
                    @Category,
                    @Status,
                    @IsDuplicate,
                    @IsEdited,
                    @IsActive,
                    @CreatedAt,
                    @UpdatedAt
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            complaint.CreatedAt = complaint.CreatedAt == default
                ? DateTime.UtcNow
                : complaint.CreatedAt;

            using var connection = _dbContext.CreateConnection();

            var complaintId = await connection.QuerySingleAsync<int>(sql, complaint);

            _logger.LogInformation(
                "Created complaint with Id {ComplaintId}",
                complaintId);

            return complaintId;
        }

        public async Task<IEnumerable<ComplaintSummaryResult>> GetByUserIdAsync(int userId)
        {
            const string sql = @"
                SELECT
                    c.Id,
                    c.Title,
                    c.Category,
                    c.Status,
                    c.IsDuplicate,
                    c.IsEdited,
                    c.CreatedAt,
                    c.UpdatedAt
                FROM Complaints c
                WHERE c.UserId = @UserId
                  AND c.IsActive = 1
                  AND c.Status != 3
                ORDER BY c.CreatedAt DESC;";

            using var connection = _dbContext.CreateConnection();

            _logger.LogInformation(
                "Fetching complaints for UserId {UserId}",
                userId);

            return await connection.QueryAsync<ComplaintSummaryResult>(
                sql,
                new { UserId = userId });
        }

        public async Task<ComplaintDetailResult?> GetByIdAsync(int complaintId)
        {
            const string sql = @"
                SELECT
                    c.Id,
                    c.UserId,
                    c.Title,
                    c.Description,
                    c.Category,
                    c.Status,
                    c.IsDuplicate,
                    c.IsEdited,
                    c.CreatedAt,
                    c.UpdatedAt
                FROM Complaints c
                WHERE c.Id = @Id
                  AND c.IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            _logger.LogInformation(
                "Fetching complaint detail with Id {ComplaintId}",
                complaintId);

            return await connection.QueryFirstOrDefaultAsync<ComplaintDetailResult>(
                sql,
                new { Id = complaintId });
        }

        public async Task<IEnumerable<ComplaintSummaryResult>> GetAllAsync(
            int? status, int? category, string? search)
        {
            const string sql = @"
                SELECT
                    c.Id,
                    c.Title,
                    c.Category,
                    c.Status,
                    c.IsDuplicate,
                    c.IsEdited,
                    c.CreatedAt,
                    c.UpdatedAt
                FROM Complaints c
                WHERE c.IsActive = 1
                  AND (@Status IS NULL OR c.Status = @Status)
                  AND (@Category IS NULL OR c.Category = @Category)
                  AND (@Search IS NULL OR c.Title LIKE '%' + @Search + '%')
                  AND c.Status != 3
                ORDER BY c.CreatedAt DESC;";

            using var connection = _dbContext.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Status", status);
            parameters.Add("@Category", category);
            parameters.Add("@Search", string.IsNullOrWhiteSpace(search) ? null : search.Trim());

            _logger.LogInformation("Fetching all complaints with filters");

            return await connection.QueryAsync<ComplaintSummaryResult>(sql, parameters);
        }

        public async Task<IEnumerable<ComplaintSummaryResult>> GetIrrelevantAsync()
        {
            const string sql = @"
                SELECT
                    c.Id,
                    c.Title,
                    c.Category,
                    c.Status,
                    c.IsDuplicate,
                    c.IsEdited,
                    c.CreatedAt,
                    c.UpdatedAt
                FROM Complaints c
                WHERE c.IsActive = 1
                  AND c.Status = 3
                ORDER BY c.UpdatedAt DESC;";

            using var connection = _dbContext.CreateConnection();

            _logger.LogInformation("Fetching all irrelevant complaints");

            return await connection.QueryAsync<ComplaintSummaryResult>(sql);
        }

        public async Task<IEnumerable<ComplaintSummaryResult>> GetIrrelevantByUserIdAsync(int userId)
        {
            const string sql = @"
                SELECT
                    c.Id,
                    c.Title,
                    c.Category,
                    c.Status,
                    c.IsDuplicate,
                    c.IsEdited,
                    c.CreatedAt,
                    c.UpdatedAt
                FROM Complaints c
                WHERE c.UserId = @UserId
                  AND c.IsActive = 1
                  AND c.Status = 3
                ORDER BY c.UpdatedAt DESC;";

            using var connection = _dbContext.CreateConnection();

            _logger.LogInformation(
                "Fetching irrelevant complaints for UserId {UserId}",
                userId);

            return await connection.QueryAsync<ComplaintSummaryResult>(
                sql,
                new { UserId = userId });
        }

        public async Task<bool> UpdateAsync(int complaintId, string title, string description,
                                             int category, DateTime updatedAt)
        {
            const string sql = @"
                UPDATE Complaints
                SET
                    Title = @Title,
                    Description = @Description,
                    Category = @Category,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            var rows = await connection.ExecuteAsync(sql, new
            {
                Id = complaintId,
                Title = title,
                Description = description,
                Category = category,
                UpdatedAt = updatedAt
            });

            if (rows > 0)
            {
                _logger.LogInformation(
                    "Updated complaint with Id {ComplaintId}",
                    complaintId);

                return true;
            }

            _logger.LogWarning(
                "Failed to update complaint with Id {ComplaintId}",
                complaintId);

            return false;
        }

        public async Task<bool> UpdateStatusAsync(int complaintId, int newStatus, DateTime updatedAt)
        {
            const string sql = @"
                UPDATE Complaints
                SET
                    Status = @Status,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            var rows = await connection.ExecuteAsync(sql, new
            {
                Id = complaintId,
                Status = newStatus,
                UpdatedAt = updatedAt
            });

            if (rows > 0)
            {
                _logger.LogInformation(
                    "Updated status for complaint Id {ComplaintId} to {NewStatus}",
                    complaintId, newStatus);

                return true;
            }

            _logger.LogWarning(
                "Failed to update status for complaint Id {ComplaintId}",
                complaintId);

            return false;
        }

        public async Task<bool> UpdateDuplicateFlagAsync(int complaintId, bool isDuplicate, DateTime updatedAt)
        {
            const string sql = @"
                UPDATE Complaints
                SET
                    IsDuplicate = @IsDuplicate,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            var rows = await connection.ExecuteAsync(sql, new
            {
                Id = complaintId,
                IsDuplicate = isDuplicate,
                UpdatedAt = updatedAt
            });

            if (rows > 0)
            {
                _logger.LogInformation(
                    "Updated duplicate flag for complaint Id {ComplaintId} to {IsDuplicate}",
                    complaintId, isDuplicate);

                return true;
            }

            _logger.LogWarning(
                "Failed to update duplicate flag for complaint Id {ComplaintId}",
                complaintId);

            return false;
        }

        public async Task<bool> ApplyAdminUpdateAsync(
            int complaintId,
            int status,
            bool isDuplicate,
            DateTime updatedAt,
            IReadOnlyCollection<ComplaintUpdate> updates)
        {
            const string updateComplaintSql = @"
                UPDATE Complaints
                SET
                    Status = @Status,
                    IsDuplicate = @IsDuplicate,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id
                  AND IsActive = 1;";

            const string createUpdateSql = @"
                INSERT INTO ComplaintUpdates
                (
                    ComplaintId,
                    UpdatedByUserId,
                    UpdateType,
                    OldStatus,
                    NewStatus,
                    NoteText,
                    CreatedAt
                )
                VALUES
                (
                    @ComplaintId,
                    @UpdatedByUserId,
                    @UpdateType,
                    @OldStatus,
                    @NewStatus,
                    @NoteText,
                    @CreatedAt
                );";

            using var connection = _dbContext.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var rowsAffected = await connection.ExecuteAsync(
                    updateComplaintSql,
                    new
                    {
                        Id = complaintId,
                        Status = status,
                        IsDuplicate = isDuplicate,
                        UpdatedAt = updatedAt
                    },
                    transaction);

                if (rowsAffected == 0)
                {
                    transaction.Rollback();
                    return false;
                }

                foreach (var update in updates)
                {
                    await connection.ExecuteAsync(createUpdateSql, update, transaction);
                }

                transaction.Commit();

                _logger.LogInformation(
                    "Applied admin update to complaint {ComplaintId} with {UpdateCount} timeline entries",
                    complaintId,
                    updates.Count);

                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> MarkAsEditedAsync(int complaintId, DateTime updatedAt)
        {
            const string sql = @"
                UPDATE Complaints
                SET
                    IsEdited = 1,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            var rows = await connection.ExecuteAsync(sql, new
            {
                Id = complaintId,
                UpdatedAt = updatedAt
            });

            if (rows > 0)
            {
                _logger.LogInformation(
                    "Marked complaint Id {ComplaintId} as edited",
                    complaintId);

                return true;
            }

            _logger.LogWarning(
                "Failed to mark complaint Id {ComplaintId} as edited",
                complaintId);

            return false;
        }

        public async Task<bool> DeleteAsync(int complaintId)
        {
            const string sql = @"
                UPDATE Complaints
                SET
                    IsActive = 0,
                    UpdatedAt = GETUTCDATE()
                WHERE Id = @Id
                  AND IsActive = 1
                  AND Status = 0;";

            using var connection = _dbContext.CreateConnection();

            var rows = await connection.ExecuteAsync(sql, new { Id = complaintId });

            if (rows > 0)
            {
                _logger.LogInformation(
                    "Soft-deleted complaint with Id {ComplaintId}",
                    complaintId);

                return true;
            }

            _logger.LogWarning(
                "Failed to soft-delete complaint with Id {ComplaintId}. Not found, inactive, or not Pending.",
                complaintId);

            return false;
        }

        #endregion

        #region CHECK

        public async Task<bool> ExistsAsync(int complaintId)
        {
            const string sql = @"
                SELECT CASE
                    WHEN EXISTS
                    (
                        SELECT 1
                        FROM Complaints
                        WHERE Id = @Id
                          AND IsActive = 1
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                END;";

            using var connection = _dbContext.CreateConnection();

            return await connection.ExecuteScalarAsync<bool>(
                sql,
                new { Id = complaintId });
        }

        public async Task<int?> GetOwnerUserIdAsync(int complaintId)
        {
            const string sql = @"
                SELECT UserId
                FROM Complaints
                WHERE Id = @Id
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<int?>(
                sql,
                new { Id = complaintId });
        }

        public async Task<int?> GetStatusAsync(int complaintId)
        {
            const string sql = @"
                SELECT Status
                FROM Complaints
                WHERE Id = @Id
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<int?>(
                sql,
                new { Id = complaintId });
        }

        #endregion

        #region COMPLAINT UPDATES (History)

        public async Task<int> CreateUpdateAsync(ComplaintUpdate update)
        {
            const string sql = @"
                INSERT INTO ComplaintUpdates
                (
                    ComplaintId,
                    UpdatedByUserId,
                    UpdateType,
                    OldStatus,
                    NewStatus,
                    NoteText,
                    CreatedAt
                )
                VALUES
                (
                    @ComplaintId,
                    @UpdatedByUserId,
                    @UpdateType,
                    @OldStatus,
                    @NewStatus,
                    @NoteText,
                    @CreatedAt
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            update.CreatedAt = update.CreatedAt == default
                ? DateTime.UtcNow
                : update.CreatedAt;

            using var connection = _dbContext.CreateConnection();

            var updateId = await connection.QuerySingleAsync<int>(sql, update);

            _logger.LogInformation(
                "Created complaint update with Id {UpdateId} for ComplaintId {ComplaintId}",
                updateId, update.ComplaintId);

            return updateId;
        }

        public async Task<IEnumerable<ComplaintUpdateResult>> GetUpdatesByComplaintIdAsync(int complaintId)
        {
            const string sql = @"
                SELECT
                    cu.Id,
                    cu.ComplaintId,
                    cu.UpdatedByUserId,
                    cu.UpdateType,
                    cu.OldStatus,
                    cu.NewStatus,
                    cu.NoteText,
                    COALESCE(NULLIF(LTRIM(RTRIM(CONCAT(u.FirstName, ' ', u.LastName))), ''), NULLIF(u.Username, ''), 'System') AS UpdatedByName,
                    cu.CreatedAt
                FROM ComplaintUpdates cu
                LEFT JOIN Users u ON u.UserId = cu.UpdatedByUserId
                WHERE cu.ComplaintId = @ComplaintId
                ORDER BY cu.CreatedAt ASC;";

            using var connection = _dbContext.CreateConnection();

            _logger.LogInformation(
                "Fetching updates for ComplaintId {ComplaintId}",
                complaintId);

            return await connection.QueryAsync<ComplaintUpdateResult>(
                sql,
                new { ComplaintId = complaintId });
        }

        #endregion

        #region DASHBOARD

        public async Task<DashboardStatsResult> GetUserDashboardStatsAsync(int userId)
        {
            const string sql = @"
                SELECT
                    COUNT(*) AS TotalComplaints,
                    SUM(CASE WHEN Status = 0 THEN 1 ELSE 0 END) AS Pending,
                    SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) AS InProgress,
                    SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) AS Resolved,
                    SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END) AS Irrelevant
                FROM Complaints
                WHERE UserId = @UserId
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            _logger.LogInformation(
                "Fetching dashboard stats for UserId {UserId}",
                userId);

            return await connection.QuerySingleAsync<DashboardStatsResult>(
                sql,
                new { UserId = userId });
        }

        public async Task<AdminDashboardStatsResult> GetAdminDashboardStatsAsync()
        {
            const string sql = @"
                SELECT
                    COUNT(*) AS TotalComplaints,
                    SUM(CASE WHEN Status = 0 THEN 1 ELSE 0 END) AS Pending,
                    SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) AS InProgress,
                    SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) AS Resolved,
                    SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END) AS Irrelevant
                FROM Complaints
                WHERE IsActive = 1;

                SELECT COUNT(*) FROM Members;

                SELECT COUNT(*) FROM Members WHERE IsRegistered = 1;

                SELECT COUNT(*) FROM Complaints
                WHERE IsActive = 1
                  AND CAST(CreatedAt AS DATE) = CAST(GETUTCDATE() AS DATE);";

            using var connection = _dbContext.CreateConnection();

            _logger.LogInformation("Fetching admin dashboard stats");

            using var multi = await connection.QueryMultipleAsync(sql);

            var stats = await multi.ReadSingleAsync<AdminDashboardStatsResult>();
            stats.TotalMembers = await multi.ReadSingleAsync<int>();
            stats.RegisteredMembers = await multi.ReadSingleAsync<int>();
            stats.TodayComplaints = await multi.ReadSingleAsync<int>();

            return stats;
        }

        #endregion
    }
}
