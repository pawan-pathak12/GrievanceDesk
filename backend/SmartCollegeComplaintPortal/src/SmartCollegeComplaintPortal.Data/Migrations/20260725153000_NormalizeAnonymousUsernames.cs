using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SmartCollegeComplaintPortal.Data.Database;

#nullable disable

namespace SmartCollegeComplaintPortal.Data.Migrations
{
    [DbContext(typeof(SystemEfCoreDbContext))]
    [Migration("20260725153000_NormalizeAnonymousUsernames")]
    public partial class NormalizeAnonymousUsernames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE Users
                SET Username = RIGHT(REPLICATE('0', 6) + CONVERT(varchar(12), UserId), 6);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Existing display names are intentionally not restored after anonymization.
        }
    }
}
