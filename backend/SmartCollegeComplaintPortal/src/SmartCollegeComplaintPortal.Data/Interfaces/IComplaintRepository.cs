using SmartCollegeComplaintPortal.Data.QueryResult;
using SmartCollegeComplaintPortal.Domain.Entities.Complaints;

namespace SmartCollegeComplaintPortal.Data.Interfaces
{
    public interface IComplaintRepository
    {
        // ──── COMPLAINT CRUD ────

        // CREATE
        Task<int> CreateAsync(Complaint complaint);

        // READ - User
        Task<IEnumerable<ComplaintSummaryResult>> GetByUserIdAsync(int userId);
        Task<ComplaintDetailResult?> GetByIdAsync(int complaintId);

        // READ - Admin
        Task<IEnumerable<ComplaintSummaryResult>> GetAllAsync(
            int? status, int? category, string? search);
        Task<IEnumerable<ComplaintSummaryResult>> GetIrrelevantAsync();
        Task<IEnumerable<ComplaintSummaryResult>> GetIrrelevantByUserIdAsync(int userId);

        // UPDATE
        Task<bool> UpdateAsync(int complaintId, string title, string description,
                               int category, DateTime updatedAt);
        Task<bool> UpdateStatusAsync(int complaintId, int newStatus, DateTime updatedAt);
        Task<bool> UpdateDuplicateFlagAsync(int complaintId, bool isDuplicate, DateTime updatedAt);
        Task<bool> MarkAsEditedAsync(int complaintId, DateTime updatedAt);
        Task<bool> ApplyAdminUpdateAsync(
            int complaintId,
            int status,
            bool isDuplicate,
            DateTime updatedAt,
            IReadOnlyCollection<ComplaintUpdate> updates);

        // DELETE
        Task<bool> DeleteAsync(int complaintId);

        // CHECK
        Task<bool> ExistsAsync(int complaintId);
        Task<int?> GetOwnerUserIdAsync(int complaintId);
        Task<int?> GetStatusAsync(int complaintId);

        // ──── COMPLAINT UPDATES (History) ────

        // CREATE
        Task<int> CreateUpdateAsync(ComplaintUpdate update);

        // READ
        Task<IEnumerable<ComplaintUpdateResult>> GetUpdatesByComplaintIdAsync(int complaintId);

        // ──── DASHBOARD ────

        Task<DashboardStatsResult> GetUserDashboardStatsAsync(int userId);
        Task<AdminDashboardStatsResult> GetAdminDashboardStatsAsync();
    }
}
