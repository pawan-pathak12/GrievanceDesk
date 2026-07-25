using Dapper;
using Microsoft.Extensions.Logging;
using SmartCollegeComplaintPortal.Data.Database;
using SmartCollegeComplaintPortal.Data.Interfaces;
using SmartCollegeComplaintPortal.Data.QueryResult;
using SmartCollegeComplaintPortal.Domain.Entities.Complaints;

namespace SmartCollegeComplaintPortal.Data.Repositories
{
    public class ComplaintCommentRepository : IComplaintCommentRepository
    {
        private readonly SystemDbContext _dbContext;
        private readonly ILogger<ComplaintCommentRepository> _logger;

        public ComplaintCommentRepository(SystemDbContext dbContext, ILogger<ComplaintCommentRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<int> CreateAsync(ComplaintComment comment)
        {
            const string sql = @"
                INSERT INTO ComplaintComments
                    (ComplaintId, UserId, Comment, IsEdited, CreatedAt, UpdatedAt, IsActive)
                VALUES
                    (@ComplaintId, @UserId, @Comment, @IsEdited, @CreatedAt, @UpdatedAt, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            comment.CreatedAt = comment.CreatedAt == default ? DateTime.UtcNow : comment.CreatedAt;
            using var connection = _dbContext.CreateConnection();
            var commentId = await connection.QuerySingleAsync<int>(sql, comment);
            _logger.LogInformation("Created comment {CommentId} for complaint {ComplaintId}", commentId, comment.ComplaintId);
            return commentId;
        }

        public async Task<ComplaintCommentResult?> GetByIdAsync(int commentId)
        {
            const string sql = @"
                SELECT
                    cc.ComplaintCommentId,
                    cc.ComplaintId,
                    cc.UserId,
                    cc.Comment,
                    cc.IsEdited,
                    cc.CreatedAt,
                    cc.UpdatedAt,
                    COALESCE(NULLIF(u.Username, ''), 'Anonymous') AS UserName,
                    u.Role AS UserRole
                FROM ComplaintComments cc
                INNER JOIN Users u ON u.UserId = cc.UserId
                WHERE cc.ComplaintCommentId = @CommentId
                  AND cc.IsActive = 1;";

            using var connection = _dbContext.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<ComplaintCommentResult>(sql, new { CommentId = commentId });
        }

        public async Task<IEnumerable<ComplaintCommentResult>> GetByComplaintIdAsync(int complaintId)
        {
            const string sql = @"
                SELECT
                    cc.ComplaintCommentId,
                    cc.ComplaintId,
                    cc.UserId,
                    cc.Comment,
                    cc.IsEdited,
                    cc.CreatedAt,
                    cc.UpdatedAt,
                    COALESCE(NULLIF(u.Username, ''), 'Anonymous') AS UserName,
                    u.Role AS UserRole
                FROM ComplaintComments cc
                INNER JOIN Users u ON u.UserId = cc.UserId
                WHERE cc.ComplaintId = @ComplaintId
                  AND cc.IsActive = 1
                ORDER BY cc.CreatedAt ASC, cc.ComplaintCommentId ASC;";

            using var connection = _dbContext.CreateConnection();
            return await connection.QueryAsync<ComplaintCommentResult>(sql, new { ComplaintId = complaintId });
        }

        public async Task<bool> UpdateAsync(int commentId, string comment, DateTime updatedAt)
        {
            const string sql = @"
                UPDATE ComplaintComments
                SET Comment = @Comment,
                    IsEdited = 1,
                    UpdatedAt = @UpdatedAt
                WHERE ComplaintCommentId = @CommentId
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();
            return await connection.ExecuteAsync(sql, new { CommentId = commentId, Comment = comment, UpdatedAt = updatedAt }) > 0;
        }

        public async Task<bool> SoftDeleteAsync(int commentId, DateTime updatedAt)
        {
            const string sql = @"
                UPDATE ComplaintComments
                SET IsActive = 0,
                    UpdatedAt = @UpdatedAt
                WHERE ComplaintCommentId = @CommentId
                  AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();
            return await connection.ExecuteAsync(sql, new { CommentId = commentId, UpdatedAt = updatedAt }) > 0;
        }
    }
}
