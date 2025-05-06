using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BankingApi.Migrations
{
    /// <inheritdoc />
    public partial class BankDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccountStatus", "AccountType", "AvailableBalance", "Currency", "DisplayName", "OpeningDate" },
                values: new object[,]
                {
                    { "12345", "ACTIVE", "SAVINGS", 5000.00m, "AUD", "Primary Savings", "2023-01-01" },
                    { "67890", "ACTIVE", "CHECKING", 1500.00m, "AUD", "Checking Account", "2023-02-01" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: "12345");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: "67890");
        }
    }
}
