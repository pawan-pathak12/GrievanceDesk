using SmartCollegeComplaintPortal.Business.DTOs.Complaints;
using SmartCollegeComplaintPortal.Domain.Commons.Pagination;

namespace SmartCollegeComplaintPortal.Business.Interfaces.Complaints
{
    public interface IComplaintService
    {
        // ──── User Operations ────
        Task<Result> CreateAsync(CreateComplaintRequest request);
        Task<Result> GetMyComplaintsAsync();
        Task<Result> GetAllForUsersAsync();
        Task<Result> GetMyComplaintByIdAsync(int complaintId);
        Task<Result> UpdateAsync(int complaintId, UpdateComplaintRequest request);
        Task<Result> DeleteAsync(int complaintId);
        Task<Result> GetMyIrrelevantAsync();

        // ──── Admin Operations ────
        Task<Result> GetAllAsync(int? status, int? category, string? search);
        Task<Result> GetComplaintByIdAsync(int complaintId);
        Task<Result> UpdateAdminComplaintAsync(int complaintId, AdminComplaintUpdateRequest request);
        Task<Result> MarkComplaintIrrelevantAsync(int complaintId, MarkIrrelevantRequest request);
        Task<Result> GetAllIrrelevantAsync();

        // ──── Dashboard ────
        Task<Result> GetUserDashboardAsync();
        Task<Result> GetAdminDashboardAsync();
    }
}
