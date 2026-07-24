using SmartCollegeComplaintPortal.Domain.Enums;

namespace SmartCollegeComplaintPortal.Domain.Commons.Pagination
{
    public class UserQueryParams : QueryParams
    {
        public int? UserId { get; set; }
        public UserRole? Role { get; set; }
        public string? Email { get; set; }
    }
}
