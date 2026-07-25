using SmartCollegeComplaintPortal.Domain.Entities.Notices;

namespace SmartCollegeComplaintPortal.Data.Interfaces
{
    public interface INoticeRepository
    {
        Task<Notice> CreateAsync(Notice notice);
        Task<Notice> UpdateAsync(Notice notice);
        Task<bool> SoftDeleteAsync(int noticeId, int updatedBy, DateTime updatedAt);
        Task<Notice?> GetByIdAsync(int noticeId);
        Task<IReadOnlyList<Notice>> GetAllAsync();
        Task<IReadOnlyList<Notice>> GetPublishedAsync();
        Task<bool> ExistsAsync(int noticeId);
    }
}
