using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SemFinance.DAL.Migrations
{
    public partial class accountLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "Loans",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Loans");
        }
    }
}
