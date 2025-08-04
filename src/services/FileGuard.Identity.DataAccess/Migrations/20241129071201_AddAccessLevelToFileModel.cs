using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileGuard.Identity.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessLevelToFileModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "AccessLevel",
                table: "UserFiles",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AccessLevel",
                table: "UserFiles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }
    }
}
