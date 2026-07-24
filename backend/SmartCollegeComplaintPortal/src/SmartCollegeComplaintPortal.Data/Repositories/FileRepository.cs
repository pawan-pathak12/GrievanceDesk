using Dapper;
using DotNetApiStarterKit.Data.Database;
using DotNetApiStarterKit.Data.Interfaces;
using DotNetApiStarterKit.Domain.Entities.Auths;
using Microsoft.Extensions.Logging;

namespace DotNetApiStarterKit.Data.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly SystemDbContext _dbContext;
        private readonly ILogger<FileRepository> _logger;

        public FileRepository(
            SystemDbContext dbContext,
            ILogger<FileRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<int> AddAsync(FileMetadata fileMetadata)
        {
            const string sql = @"
             INSERT INTO FileMetadatas
             (
                 OriginalFileName,
                 StoredFileName,
                 RelativePath,
                 ContentType,
                 Size,
                 IsPublic,
                 UploadedAt,
                 UploadedByUserId,
                 EntityType,
                 EntityId,IsActive
             )
             VALUES
             (
                 @OriginalFileName,
                 @StoredFileName,
                 @RelativePath,
                 @ContentType,
                 @Size,
                 @IsPublic,
                 @UploadedAt,
                 @UploadedByUserId,
                 @EntityType,
                 @EntityId,@IsActive
             );

             SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = _dbContext.CreateConnection();

            int fileId = await connection.QuerySingleAsync<int>(
                sql,
                fileMetadata);

            _logger.LogInformation(
                "File metadata saved with Id {FileId}",
                fileId);

            return fileId;
        }

        public async Task<bool> DeleteAsync(int fileMetadataId)
        {
            const string sql = @"
             UPDATE FileMetadatas
             SET
                 IsActive = 0,
                 UpdatedAt = @UpdatedAt
             WHERE FileMetadataId = @FileMetadataId
               AND IsActive = 1;";

            using var connection = _dbContext.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                sql,
                new
                {
                    FileMetadataId = fileMetadataId,
                    UpdatedAt = DateTime.UtcNow
                });

            if (rowsAffected > 0)
            {
                _logger.LogInformation(
                    "Soft deleted user FileMetadata Id {FileMetadataId}",
                    fileMetadataId);

                return true;
            }

            _logger.LogWarning(
                "Failed to soft delete FileMetadata with Id {FileMetadataId}",
                fileMetadataId);

            return false;
        }

        public async Task<IEnumerable<FileMetadata>> GetByEntityAsync(
      string entityType,
      int entityId)
        {
            const string sql = @"
     SELECT
         FileMetadataId,
         OriginalFileName,
         StoredFileName,
         RelativePath,
         ContentType,
         Size,
         IsPublic,
         UploadedAt,
         UploadedByUserId,
         EntityType,
         EntityId
     FROM FileMetadatas
     WHERE EntityType = @EntityType
       AND EntityId = @EntityId and IsActive =1
     ORDER BY UploadedAt DESC;";

            using var connection = _dbContext.CreateConnection();

            return await connection.QueryAsync<FileMetadata>(
                sql,
                new
                {
                    EntityType = entityType,
                    EntityId = entityId
                });
        }

        public async Task<FileMetadata?> GetByIdAsync(
      int fileMetadataId)
        {
            const string sql = @"
     SELECT TOP 1 *
     FROM FileMetadatas
     WHERE FileMetadataId = @FileMetadataId and IsActive=1;";

            using var connection = _dbContext.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<FileMetadata>(
                sql,
                new
                {
                    FileMetadataId = fileMetadataId
                });
        }
    }
}
