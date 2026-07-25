using SmartCollegeComplaintPortal.Business.DTOs.Notices;
using SmartCollegeComplaintPortal.Domain.Commons.Pagination;

namespace SmartCollegeComplaintPortal.Business.Interfaces.Notices
{
    public interface INoticeService
    {
        Task<Result> CreateAsync(CreateNoticeDto request);
        Task<Result> UpdateAsync(int noticeId, UpdateNoticeDto request);
        Task<Result> PublishAsync(int noticeId);
        Task<Result> UnpublishAsync(int noticeId);
        Task<Result> SoftDeleteAsync(int noticeId);
        Task<Result> GetAllAsync();
        Task<Result> GetByIdAsync(int noticeId);
        Task<Result> GetPublishedAsync();
        Task<Result> GetPublishedByIdAsync(int noticeId);
    }
}
