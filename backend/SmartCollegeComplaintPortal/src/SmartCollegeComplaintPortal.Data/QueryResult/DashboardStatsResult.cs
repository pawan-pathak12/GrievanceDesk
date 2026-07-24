namespace SmartCollegeComplaintPortal.Data.QueryResult
{
    public class DashboardStatsResult
    {
        public int TotalComplaints { get; set; }
        public int Pending { get; set; }
        public int InProgress { get; set; }
        public int Resolved { get; set; }
        public int Closed { get; set; }
        public int Irrelevant { get; set; }
    }

    public class AdminDashboardStatsResult : DashboardStatsResult
    {
        public int TotalMembers { get; set; }
        public int RegisteredMembers { get; set; }
        public int TodayComplaints { get; set; }
    }
}
