using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCollegeComplaintPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsForwardedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsForwarded",
                table: "Complaints");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsForwarded",
                table: "Complaints",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
