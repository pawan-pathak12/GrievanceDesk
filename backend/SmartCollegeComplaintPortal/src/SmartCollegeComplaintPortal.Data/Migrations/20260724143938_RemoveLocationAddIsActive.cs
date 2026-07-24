using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCollegeComplaintPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLocationAddIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Complaints");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Complaints",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Complaints");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Complaints",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
