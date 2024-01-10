using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uyg1.Migrations
{
    /// <inheritdoc />
    public partial class mig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Questions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_AppUserId",
                table: "Questions",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_AspNetUsers_AppUserId",
                table: "Questions",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_AspNetUsers_AppUserId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_AppUserId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Questions");
        }
    }
}
