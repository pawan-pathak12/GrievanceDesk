using SmartCollegeComplaintPortal.Data.QueryResult;
using SmartCollegeComplaintPortal.Domain.Entities.Complaints;

namespace SmartCollegeComplaintPortal.Data.Interfaces
{
    public interface IComplaintCommentRepository
    {
        Task<int> CreateAsync(ComplaintComment comment);
        Task<ComplaintCommentResult?> GetByIdAsync(int commentId);
        Task<IEnumerable<ComplaintCommentResult>> GetByComplaintIdAsync(int complaintId);
        Task<bool> UpdateAsync(int commentId, string comment, DateTime updatedAt);
        Task<bool> SoftDeleteAsync(int commentId, DateTime updatedAt);
    }
}
