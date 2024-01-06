using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnapHub.Data.Migrations
{
    public partial class portfolioNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "Portfolio",
            columns: new[] { "Id", "Title" },
            values: new object[] { 1, "Portfolio" }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
