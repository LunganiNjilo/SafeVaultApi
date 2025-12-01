using Application.Models;
using Domain.Entities;
using NSubstitute;
using SafeVaultApi.Models.Request;
using SafeVaultApi.Tests.Infrastructure;

namespace SafeVaultApi.Tests.Tests.Transfers
{
    internal class TransferControllerTest : TestBase
    {
        private SafeVaultApiClient _SafeVaultApiClient;

        [SetUp]
        public void Setup()
        {
            _SafeVaultApiClient = CreateApiClient<SafeVaultApiClient>();

            accountRepositoryMock.ClearReceivedCalls();
            transactionRepositoryMock.ClearReceivedCalls();
            uowMock.ClearReceivedCalls();
        }

        [Test]
        public async Task Transfer_Should_Succeed_When_Funds_Are_Available()
        {
            // Arrange test data
            var from = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "111",
                Balance = 1000
            };

            var to = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "222",
                Balance = 500
            };

            // Mock repository responses
            accountRepositoryMock.GetByNumberAsync("111").Returns(from);
            accountRepositoryMock.GetByNumberAsync("222").Returns(to);

            // UoW must allow commit
            uowMock.BeginTransactionAsync().Returns(Task.CompletedTask);
            uowMock.SaveChangesAsync().Returns(Task.FromResult(1));

            uowMock.CommitAsync().Returns(Task.CompletedTask);

            var request = new TransferRequest
            {
                FromAccountId = from.AccountNumber,
                ToAccountId = to.AccountNumber,
                Amount = 200
            };

            // Act
            var result = await _SafeVaultApiClient.PostAsync<TransferAccountResult>("/api/transfers", request);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Amount, Is.EqualTo(200));
            Assert.That(result.FromAccountBalance, Is.EqualTo(800));
            Assert.That(result.ToAccountBalance, Is.EqualTo(700));

            // Ensure updates happened
            await accountRepositoryMock.Received().UpdateAsync(Arg.Any<Account>());
            await uowMock.Received().CommitAsync();
        }
    }
}
