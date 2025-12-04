using Domain.Entities;
using Domain.Enums;
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

        [Test]
        public async Task CreateManualTransaction_Should_ReturnOK_When_Valid()
        {
            var account = new Account { Id = Guid.NewGuid(), Balance = 200 };
            var accountId = account.Id;

            accountRepositoryMock.GetByIdAsync(accountId).Returns(account);
            transactionRepositoryMock.AddAsync(Arg.Any<Transaction>()).Returns(Task.CompletedTask);

            var request = new CreateManualTransactionRequest
            {
                Amount = 50,
                Type = TransactionType.Debit,
                TransactionDate = DateTime.UtcNow,
                Description = "Test manual debit"
            };

            var response = await _client.PostRawAsync(
                $"/api/accounts/{accountId}/manual-transactions",
                request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            await transactionRepositoryMock.Received().AddAsync(Arg.Any<Transaction>());
            await accountRepositoryMock.Received().UpdateAsync(account);
        }

        [Test]
        public async Task CreateManualTransaction_Should_ReturnBadRequest_When_AccountClosed()
        {
            var account = new Account { Id = Guid.NewGuid(), Balance = 200, IsClosed = true };
            var accountId = account.Id;

            accountRepositoryMock.GetByIdAsync(accountId).Returns(account);

            var request = new CreateManualTransactionRequest
            {
                Amount = 50,
                Type = TransactionType.Debit,
                TransactionDate = DateTime.UtcNow
            };

            var response = await _client.PostRawAsync(
                $"/api/accounts/{accountId}/manual-transactions",
                request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            await transactionRepositoryMock.DidNotReceive().AddAsync(Arg.Any<Transaction>());
        }

        [Test]
        public async Task UpdateManualTransaction_Should_ReturnBadRequest_When_NotManual()
        {
            var tx = new Transaction { Id = Guid.NewGuid(), IsManual = false };

            transactionRepositoryMock.GetByIdAsync(tx.Id).Returns(tx);

            var request = new UpdateManualTransactionRequest
            {
                Amount = 100,
                Type = TransactionType.Debit,
                TransactionDate = DateTime.UtcNow
            };

            var response = await _client.PutRawAsync(
                $"/api/accounts/manual-transactions/{tx.Id}",
                request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            await transactionRepositoryMock.DidNotReceive().UpdateAsync(Arg.Any<Transaction>());
        }

        [Test]
        public async Task DeleteManualTransaction_Should_ReturnOK_When_Manual()
        {
            var tx = new Transaction { Id = Guid.NewGuid(), IsManual = true };

            transactionRepositoryMock.GetByIdAsync(tx.Id).Returns(tx);
            transactionRepositoryMock.DeleteAsync(tx).Returns(Task.CompletedTask);

            var response = await _client.DeleteRawAsync(
                $"/api/accounts/manual-transactions/{tx.Id}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            await transactionRepositoryMock.Received().DeleteAsync(tx);
        }

        [Test]
        public async Task DeleteManualTransaction_Should_ReturnBadRequest_When_NotManual()
        {
            var tx = new Transaction { Id = Guid.NewGuid(), IsManual = false };

            transactionRepositoryMock.GetByIdAsync(tx.Id).Returns(tx);

            var response = await _client.DeleteRawAsync(
                $"/api/accounts/manual-transactions/{tx.Id}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            await transactionRepositoryMock.DidNotReceive().DeleteAsync(Arg.Any<Transaction>());
        }

        [Test]
        public async Task CloseAccount_Should_ReturnNoContent_When_BalanceIsZero()
        {
            // Arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 0m,
                IsClosed = false
            };

            accountRepositoryMock.GetByIdAsync(account.Id).Returns(account);
            accountRepositoryMock.UpdateAsync(account).Returns(Task.CompletedTask);

            // Act
            var response = await _client.PostRawAsync($"/api/accounts/{account.Id}/close");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            await accountRepositoryMock.Received().UpdateAsync(Arg.Is<Account>(a =>
                a.Id == account.Id && a.IsClosed == true));
        }

        [Test]
        public async Task CloseAccount_Should_ReturnNotFound_When_AccountDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            accountRepositoryMock.GetByIdAsync(id).Returns((Account?)null);

            // Act
            var response = await _client.PostRawAsync($"/api/accounts/{id}/close");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            await accountRepositoryMock.DidNotReceive().UpdateAsync(Arg.Any<Account>());
        }

        [Test]
        public async Task CloseAccount_Should_ReturnBadRequest_When_BalanceNotZero()
        {
            // Arrange
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 50m,
                IsClosed = false
            };

            accountRepositoryMock.GetByIdAsync(account.Id).Returns(account);

            // Act
            var response = await _client.PostRawAsync($"/api/accounts/{account.Id}/close");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            // UpdateAsync should NOT be called
            await accountRepositoryMock.DidNotReceive()
                .UpdateAsync(Arg.Any<Account>());
        }
    }
}
