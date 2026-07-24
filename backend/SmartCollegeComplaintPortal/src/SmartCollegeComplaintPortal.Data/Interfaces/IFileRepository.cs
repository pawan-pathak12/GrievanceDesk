using DotNetApiStarterKit.Domain.Entities.Auths;

namespace DotNetApiStarterKit.Data.Interfaces
{
    public interface IFileRepository
    {
        Task<int> AddAsync(FileMetadata fileMetadata);

        Task<FileMetadata?> GetByIdAsync(int fileMetadataId);

        Task<IEnumerable<FileMetadata>> GetByEntityAsync(
            string entityType,
            int entityId);

        Task<bool> DeleteAsync(int fileMetadataId);
    }
}
