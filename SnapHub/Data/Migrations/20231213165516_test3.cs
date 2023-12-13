using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnapHub.Data.Migrations
{
    public partial class test3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Session_Photo_PhotoId",
                table: "Session");

            migrationBuilder.DropIndex(
                name: "IX_Session_PhotoId",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Session");

            migrationBuilder.AlterColumn<int>(
                name: "SessionId",
                table: "Photo",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Photo_SessionId",
                table: "Photo",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photo_Session_SessionId",
                table: "Photo",
                column: "SessionId",
                principalTable: "Session",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photo_Session_SessionId",
                table: "Photo");

            migrationBuilder.DropIndex(
                name: "IX_Photo_SessionId",
                table: "Photo");

            migrationBuilder.AddColumn<int>(
                name: "PhotoId",
                table: "Session",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "Photo",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Session_PhotoId",
                table: "Session",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_Photo_PhotoId",
                table: "Session",
                column: "PhotoId",
                principalTable: "Photo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
