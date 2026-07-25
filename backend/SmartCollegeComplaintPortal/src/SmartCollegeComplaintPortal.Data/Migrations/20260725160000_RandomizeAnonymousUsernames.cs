using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SmartCollegeComplaintPortal.Data.Database;

#nullable disable

namespace SmartCollegeComplaintPortal.Data.Migrations
{
    [DbContext(typeof(SystemEfCoreDbContext))]
    [Migration("20260725160000_RandomizeAnonymousUsernames")]
    public partial class RandomizeAnonymousUsernames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @UserId int;
                DECLARE @Username nvarchar(50);
                DECLARE @RandomNumber int;

                DECLARE user_cursor CURSOR LOCAL FAST_FORWARD FOR
                SELECT UserId
                FROM dbo.Users
                ORDER BY UserId;

                OPEN user_cursor;
                FETCH NEXT FROM user_cursor INTO @UserId;

                WHILE @@FETCH_STATUS = 0
                BEGIN
                    SET @Username = NULL;

                    WHILE @Username IS NULL
                    BEGIN
                        SET @RandomNumber = ABS(CONVERT(bigint, CHECKSUM(NEWID()))) % 1000000;
                        SET @Username = CONCAT(
                            'user_',
                            RIGHT(CONCAT('000000', CONVERT(varchar(6), @RandomNumber)), 6));

                        IF EXISTS (SELECT 1 FROM dbo.Users WHERE Username = @Username)
                        BEGIN
                            SET @Username = NULL;
                        END
                    END

                    UPDATE dbo.Users
                    SET Username = @Username
                    WHERE UserId = @UserId;

                    FETCH NEXT FROM user_cursor INTO @UserId;
                END

                CLOSE user_cursor;
                DEALLOCATE user_cursor;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Random usernames intentionally remain in place when rolling back this data migration.
        }
    }
}
