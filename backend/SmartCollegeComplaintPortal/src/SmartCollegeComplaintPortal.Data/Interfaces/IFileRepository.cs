using SmartCollegeComplaintPortal.Domain.Entities.Auths;

namespace SmartCollegeComplaintPortal.Data.Interfaces
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
