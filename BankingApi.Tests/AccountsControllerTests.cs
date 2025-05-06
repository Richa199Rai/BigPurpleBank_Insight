using BankingApi.Controllers;
using BankingApi.Data;
using BankingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;


[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace BankingApi.Tests
{
    public class ExtendedAccountsControllerTests
    {
       
       private BankingContext GetInMemoryContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<BankingContext>()
                .UseInMemoryDatabase(databaseName)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options;

            var context = new BankingContext(options);
            context.Database.EnsureDeleted();  
            context.Database.EnsureCreated();
            return context;
        }
       
       [Fact]
        public async Task GetAccounts_ReturnsOkResultWithAccounts()
        {
            using var context = GetInMemoryContext(Guid.NewGuid().ToString());
            context.Accounts.AddRange(
                new Account { AccountId = Guid.NewGuid().ToString(), DisplayName = "Primary Savings", AccountType = "SAVINGS", AvailableBalance = 5000.00M, Currency = "AUD" },
                new Account { AccountId = Guid.NewGuid().ToString(), DisplayName = "Checking Account", AccountType = "CHECKING", AvailableBalance = 1500.00M, Currency = "AUD" }
            );
            context.SaveChanges();

            var controller = new AccountsController(context);
            var result = await controller.GetAccounts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AccountsResponse>(okResult.Value);

            Assert.Equal(4, response.data.accounts.Count);
        }

        [Fact]
        public async Task GetAccounts_WithNoAccounts_ReturnsEmptyArray()
        {
            using var context = GetInMemoryContext(Guid.NewGuid().ToString());
            var controller = new AccountsController(context);
            var result = await controller.GetAccounts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AccountsResponse>(okResult.Value);

            Assert.Empty(response.data.accounts);
        }

        [Fact]
        public async Task GetAccounts_WithVariousCurrencies_ReturnsAllAccounts()
        {
            using var context = GetInMemoryContext(Guid.NewGuid().ToString());
            context.Accounts.AddRange(
                new Account { AccountId = Guid.NewGuid().ToString(), DisplayName = "AUD Savings", AccountType = "SAVINGS", AvailableBalance = 5000.00M, Currency = "AUD" },
                new Account { AccountId = Guid.NewGuid().ToString(), DisplayName = "USD Checking", AccountType = "CHECKING", AvailableBalance = 1500.00M, Currency = "USD" },
                new Account { AccountId = Guid.NewGuid().ToString(), DisplayName = "EUR Savings", AccountType = "SAVINGS", AvailableBalance = 3000.00M, Currency = "EUR" }
            );
            context.SaveChanges();

            var controller = new AccountsController(context);
            var result = await controller.GetAccounts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AccountsResponse>(okResult.Value);

            Assert.Equal(5, response.data.accounts.Count);
        }
    }
}
