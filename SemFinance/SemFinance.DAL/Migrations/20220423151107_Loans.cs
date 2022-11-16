using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SemFinance.DAL.Migrations
{
    public partial class Loans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoanID",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Loans_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LoanID",
                table: "Transactions",
                column: "LoanID");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_AccountID",
                table: "Loans",
                column: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Loans_LoanID",
                table: "Transactions",
                column: "LoanID",
                principalTable: "Loans",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Loans_LoanID",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_LoanID",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "LoanID",
                table: "Transactions");
        }
    }
}
