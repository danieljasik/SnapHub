using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnapHub.Data.Migrations
{
    public partial class PortfolioTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PortfolioId",
                table: "Photo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Portfolio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolio", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photo_PortfolioId",
                table: "Photo",
                column: "PortfolioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photo_Portfolio_PortfolioId",
                table: "Photo",
                column: "PortfolioId",
                principalTable: "Portfolio",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photo_Portfolio_PortfolioId",
                table: "Photo");

            migrationBuilder.DropTable(
                name: "Portfolio");

            migrationBuilder.DropIndex(
                name: "IX_Photo_PortfolioId",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "PortfolioId",
                table: "Photo");
        }
    }
}
