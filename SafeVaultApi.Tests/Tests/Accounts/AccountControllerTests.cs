using Domain.Entities;
using NSubstitute;
using SafeVaultApi.Models.Request;
using SafeVaultApi.Models.Response;
using SafeVaultApi.Tests.Infrastructure;
using System.Net;

namespace SafeVaultApi.Tests.Tests.Accounts
{
    internal class AccountControllerTests : TestBase
    {
        private SafeVaultApiClient _client;

        [SetUp]
        public void Setup()
        {
            _client = CreateApiClient<SafeVaultApiClient>();

            accountRepositoryMock.ClearReceivedCalls();
            transactionRepositoryMock.ClearReceivedCalls();
            uowMock.ClearReceivedCalls();
        }

        [Test]
        public async Task GetBalance_ReturnsAccountBalance()
        {
            // Arrange
            var acct = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "ACC123",
                Balance = 123.45m
            };

            accountRepositoryMock.GetByNumberAsync("ACC123").Returns(acct);

            // Act
            var result = await _client.GetAsync<decimal>($"/api/accounts/{acct.AccountNumber}/balance");

            // Assert
            Assert.That(result, Is.EqualTo(123.45m));
            await accountRepositoryMock.Received().GetByNumberAsync("ACC123");
        }

        [Test]
        public async Task Credit_Should_CreateTransactionAndCommit()
        {
            // Arrange
            var acct = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "CR123",
                Balance = 100m
            };

            accountRepositoryMock.GetByAccountNumberAsync("CR123").Returns(acct);

            // UoW and repo behaviors
            uowMock.BeginTransactionAsync().Returns(Task.CompletedTask);
            uowMock.SaveChangesAsync().Returns(1);
            uowMock.CommitAsync().Returns(Task.CompletedTask);

            transactionRepositoryMock.AddAsync(Arg.Any<Transaction>()).Returns(Task.CompletedTask);
            accountRepositoryMock.UpdateAsync(Arg.Any<Account>()).Returns(Task.CompletedTask);

            var request = new AmountRequest { Amount = 50m };

            // Act
            var response = await _client.PostAsync<TransactionResponse>($"/api/accounts/{acct.AccountNumber}/credit", request);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Amount, Is.EqualTo(50m));
            Assert.That(response.BalanceAfter, Is.EqualTo(acct.Balance));

            await accountRepositoryMock.Received().UpdateAsync(Arg.Is<Account>(a => a.AccountNumber == "CR123" && a.Balance == 150m));
            await transactionRepositoryMock.Received().AddAsync(Arg.Any<Transaction>());
            await uowMock.Received().CommitAsync();
        }

        [Test]
        public async Task Debit_Should_CreateTransactionAndCommit()
        {
            // Arrange
            var acct = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "DB123",
                Balance = 300m
            };

            accountRepositoryMock.GetByAccountNumberAsync("DB123").Returns(acct);

            uowMock.BeginTransactionAsync().Returns(Task.CompletedTask);
            uowMock.SaveChangesAsync().Returns(1);
            uowMock.CommitAsync().Returns(Task.CompletedTask);

            transactionRepositoryMock.AddAsync(Arg.Any<Transaction>()).Returns(Task.CompletedTask);
            accountRepositoryMock.UpdateAsync(Arg.Any<Account>()).Returns(Task.CompletedTask);

            var request = new AmountRequest { Amount = 120m };

            // Act
            var response = await _client.PostAsync<TransactionResponse>($"/api/accounts/{acct.AccountNumber}/debit", request);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Amount, Is.EqualTo(120m));
            Assert.That(response.BalanceAfter, Is.EqualTo(180m));

            await accountRepositoryMock.Received().UpdateAsync(Arg.Is<Account>(a => a.AccountNumber == "DB123" && a.Balance == 180m));
            await transactionRepositoryMock.Received().AddAsync(Arg.Any<Transaction>());
            await uowMock.Received().CommitAsync();
        }

        [Test]
        public async Task Debit_Should_ReturnBadRequest_When_InsufficientFunds()
        {
            // Arrange
            var acct = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "LOW123",
                Balance = 30m
            };

            accountRepositoryMock.GetByAccountNumberAsync("LOW123").Returns(acct);

            uowMock.BeginTransactionAsync().Returns(Task.CompletedTask);
            uowMock.RollbackAsync().Returns(Task.CompletedTask);

            var request = new AmountRequest { Amount = 50m };

            // Act
            var httpResponse = await _client.PostRawAsync($"/api/accounts/{acct.AccountNumber}/debit", request);

            // Assert
            Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            // Ensure update/add/commit were NOT called, rollback attempted
            await accountRepositoryMock.DidNotReceive().UpdateAsync(Arg.Any<Account>());
            await transactionRepositoryMock.DidNotReceive().AddAsync(Arg.Any<Transaction>());
            await uowMock.Received().RollbackAsync();
        }

        [Test]
        public async Task Credit_Should_ReturnBadRequest_When_AmountIsInvalid()
        {
            // Arrange
            var acct = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "INV123",
                Balance = 100m
            };

            accountRepositoryMock.GetByNumberAsync("INV123").Returns(acct);

            var request = new AmountRequest { Amount = 0m };

            // Act
            var httpResponse = await _client.PostRawAsync($"/api/accounts/{acct.AccountNumber}/credit", request);

            // Assert
            Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            // Make sure nothing changed
            await accountRepositoryMock.DidNotReceive().UpdateAsync(Arg.Any<Account>());
            await transactionRepositoryMock.DidNotReceive().AddAsync(Arg.Any<Transaction>());
            await uowMock.DidNotReceive().CommitAsync();
        }

        [Test]
        public async Task GetByUserId_ReturnsMappedAccounts()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var accounts = new[]
            {
                new Account { Id = Guid.NewGuid(), AccountNumber = "A1", Balance = 10m },
                new Account { Id = Guid.NewGuid(), AccountNumber = "A2", Balance = 20m }
            };

            accountRepositoryMock.GetByUserIdAsync(userId).Returns(accounts);

            // Act
            var result = await _client.GetAsync<IEnumerable<AccountResponse>>($"/api/accounts/userId/{userId}");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Select(a => a.AccountNumber), Is.EquivalentTo(new[] { "A1", "A2" }));
            await accountRepositoryMock.Received().GetByUserIdAsync(userId);
        }
    }
}
