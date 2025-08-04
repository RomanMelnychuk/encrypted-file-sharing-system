using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileGuard.Identity.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessLevelToUserFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "AccessLevel",
                table: "UserFolders",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<long>(
                name: "SizeInBytes",
                table: "Files",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SizeInBytes",
                table: "Files");

            migrationBuilder.AlterColumn<string>(
                name: "AccessLevel",
                table: "UserFolders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }
    }
}
