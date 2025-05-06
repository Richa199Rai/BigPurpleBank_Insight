using BankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Data
{
    public class BankingContext : DbContext
    {
        public BankingContext(DbContextOptions<BankingContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasKey(a => a.AccountId);

            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    AccountId = "12345",
                    DisplayName = "Primary Savings",
                    AccountType = "SAVINGS",
                    AccountStatus = "ACTIVE",
                    Currency = "AUD",
                    OpeningDate = "2023-01-01",
                    AvailableBalance = 5000.00M
                },
                new Account
                {
                    AccountId = "67890",
                    DisplayName = "Checking Account",
                    AccountType = "CHECKING",
                    AccountStatus = "ACTIVE",
                    Currency = "AUD",
                    OpeningDate = "2023-02-01",
                    AvailableBalance = 1500.00M
                });
        }
    }
}