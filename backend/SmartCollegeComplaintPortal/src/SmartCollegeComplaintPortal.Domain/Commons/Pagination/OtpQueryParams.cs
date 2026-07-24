namespace SmartCollegeComplaintPortal.Domain.Commons.Pagination
{
    public class OtpQueryParams : QueryParams
    {
        public int? OtpId { get; set; }
        public string? Email { get; set; }
        public bool? IsUsed { get; set; }
    }
}
