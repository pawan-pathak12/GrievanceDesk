using SmartCollegeComplaintPortal.Business.DTOs.Complaints;
using SmartCollegeComplaintPortal.Domain.Commons.Pagination;

namespace SmartCollegeComplaintPortal.Business.Interfaces.Complaints
{
    public interface IComplaintCommentService
    {
        Task<Result> GetByComplaintIdAsync(int complaintId);
        Task<Result> CreateAsync(int complaintId, CreateComplaintCommentRequest request);
        Task<Result> UpdateAsync(int commentId, UpdateComplaintCommentRequest request);
        Task<Result> DeleteOwnAsync(int commentId);
        Task<Result> DeleteAsAdminAsync(int commentId);
    }
}
