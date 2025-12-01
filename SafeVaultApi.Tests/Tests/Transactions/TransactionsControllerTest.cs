using Domain.Entities;
using Domain.Enums;
using NSubstitute;
using SafeVaultApi.Models.Response;
using SafeVaultApi.Tests.Infrastructure;

namespace SafeVaultApi.Tests.Tests.Transactions
{
    internal class TransactionsControllerTest : TestBase
    {
        private SafeVaultApiClient _client;

        [SetUp]
        public void Setup()
        {
            _client = CreateApiClient<SafeVaultApiClient>();

            transactionRepositoryMock.ClearReceivedCalls();
            accountRepositoryMock.ClearReceivedCalls();
            uowMock.ClearReceivedCalls();
        }

        [Test]
        public async Task GetTransactions_Should_ReturnMappedResponse()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var tx = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                Amount = 250,
                Fee = 5,
                BalanceAfter = 1000,
                Description = "Payment",
                Type = TransactionType.Debit,
                CreatedAt = DateTime.UtcNow
            };

            // Mock
            transactionRepositoryMock
                .GetByAccountIdAsync(accountId, 0, 50)
                .Returns(new[] { tx });

            // Act
            var response = await _client.GetAsync<IEnumerable<TransactionResponse>>(
                $"/api/transactions/{accountId}");

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Count(), Is.EqualTo(1));

            var item = response.First();
            Assert.That(item.Id, Is.EqualTo(tx.Id));
            Assert.That(item.Amount, Is.EqualTo(250));
            Assert.That(item.Fee, Is.EqualTo(5));
            Assert.That(item.BalanceAfter, Is.EqualTo(1000));
            Assert.That(item.Description, Is.EqualTo("Payment"));
            Assert.That(item.Type, Is.EqualTo("Debit"));
        }

        [Test]
        public async Task GetTransactions_Should_ReturnEmptyList_When_NoTransactions()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            // Mock
            transactionRepositoryMock
                .GetByAccountIdAsync(accountId, 0, 50)
                .Returns(Enumerable.Empty<Transaction>());

            // Act
            var result = await _client.GetAsync<IEnumerable<TransactionResponse>>(
                $"/api/transactions/{accountId}");

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetTransactions_Should_ForwardSkipAndTake()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            // Mock
            transactionRepositoryMock
                .GetByAccountIdAsync(accountId, 10, 20)
                .Returns(Array.Empty<Transaction>());

            // Act
            var response = await _client.GetAsync<IEnumerable<TransactionResponse>>(
                $"/api/transactions/{accountId}?skip=10&take=20");

            // Assert
            await transactionRepositoryMock
                .Received()
                .GetByAccountIdAsync(accountId, 10, 20);
        }
    }
}
