using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCollegeComplaintPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAttemptCountFromOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttemptCount",
                table: "Otps");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttemptCount",
                table: "Otps",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
