namespace DotNetApiStarterKit.Domain.Commons.Pagination
{
    public class QueryParams : PaginationParams
    {
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public string SortOrder { get; set; } = "asc";
        public bool? IsActive { get; set; }
    }
}
