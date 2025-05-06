using BankingApi.Controllers;
using BankingApi.Data;
using BankingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BankingApi.Tests
{
    public class ExtendedAccountsControllerTests
    {
        private BankingContext GetInMemoryContext()
{
    var options = new DbContextOptionsBuilder<BankingContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    var context = new BankingContext(options);
    context.Database.EnsureDeleted(); // Ensure clean state
    context.Database.EnsureCreated();
    return context;
}
        
        [Fact]
        public async Task GetAccounts_ReturnsOkResultWithAccounts()
        {
            // Arrange
            using var context = GetInMemoryContext("TestDB1");
            context.Accounts.AddRange(
                new Account { AccountId = "12345", DisplayName = "Primary Savings", AccountType = "SAVINGS", AvailableBalance = 5000.00M, Currency = "AUD" },
                new Account { AccountId = "67890", DisplayName = "Checking Account", AccountType = "CHECKING", AvailableBalance = 1500.00M, Currency = "AUD" }
            );
            context.SaveChanges();

            var controller = new AccountsController(context);

            // Act
            var result = await controller.GetAccounts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AccountsResponse>(okResult.Value);

            Assert.NotNull(response);
            Assert.NotNull(response.data);
            Assert.NotNull(response.data.accounts);
            Assert.Equal(2, response.data.accounts.Count);
        }
        
        [Fact]
        public async Task GetAccounts_WithNoAccounts_ReturnsEmptyArray()
        {
            // Arrange
            using var context = GetInMemoryContext("TestDB2");
            var controller = new AccountsController(context);

            // Act
            var result = await controller.GetAccounts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AccountsResponse>(okResult.Value);

            Assert.NotNull(response);
            Assert.NotNull(response.data);
            Assert.NotNull(response.data.accounts);
            Assert.Empty(response.data.accounts);
        }
        
        [Fact]
        public async Task GetAccounts_WithVariousCurrencies_ReturnsAllAccounts()
        {
            // Arrange
            using var context = GetInMemoryContext("TestDB3");
            context.Accounts.AddRange(
                new Account { AccountId = "12345", DisplayName = "AUD Savings", AccountType = "SAVINGS", AvailableBalance = 5000.00M, Currency = "AUD" },
                new Account { AccountId = "67890", DisplayName = "USD Checking", AccountType = "CHECKING", AvailableBalance = 1500.00M, Currency = "USD" },
                new Account { AccountId = "24680", DisplayName = "EUR Savings", AccountType = "SAVINGS", AvailableBalance = 3000.00M, Currency = "EUR" }
            );
            context.SaveChanges();

            var controller = new AccountsController(context);

            // Act
            var result = await controller.GetAccounts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AccountsResponse>(okResult.Value);

            Assert.NotNull(response);
            Assert.NotNull(response.data);
            Assert.NotNull(response.data.accounts);
            Assert.Equal(3, response.data.accounts.Count);
            
            // Check each currency is represented
            Assert.Contains(response.data.accounts, a => a.Currency == "AUD");
            Assert.Contains(response.data.accounts, a => a.Currency == "USD");
            Assert.Contains(response.data.accounts, a => a.Currency == "EUR");
        }
    }
}