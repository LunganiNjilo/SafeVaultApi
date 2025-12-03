using Application.Common.Exceptions;
using Domain.Entities;
using NSubstitute;
using SafeVaultApi.Models.Request;
using SafeVaultApi.Models.Response;
using SafeVaultApi.Tests.Infrastructure;

namespace SafeVaultApi.Tests.Tests.Airtime
{
    internal class AirtimeControllerTest : TestBase
    {
        private SafeVaultApiClient _client;

        [SetUp]
        public void Setup()
        {
            _client = CreateApiClient<SafeVaultApiClient>();
            accountRepositoryMock.ClearReceivedCalls();
        }

        [Test]
        public async Task Purchase_Should_ReturnSuccess_When_DebitSucceeds()
        {
            // Arrange
            var acct = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = "12345",
                Balance = 500
            };

            // Account repository: return account
            accountRepositoryMock
                .GetByAccountNumberAsync("12345")
                .Returns(acct);

            // UoW transaction flow
            uowMock.BeginTransactionAsync().Returns(Task.CompletedTask);
            uowMock.SaveChangesAsync().Returns(1);
            uowMock.CommitAsync().Returns(Task.CompletedTask);

            // Transaction repository: record new tx
            transactionRepositoryMock
                .AddAsync(Arg.Any<Transaction>())
                .Returns(Task.CompletedTask);

            var request = new AirtimeRequest
            {
                AccountNumber = "12345",
                Description = "0720001111",
                Amount = 50
            };

            // Act
            var result = await _client.PostAsync<AirtimeResponse>(
                "/api/airtime/purchase", request);

            // Assert
            Assert.That(result.IsSuccess, Is.True);

            await accountRepositoryMock
                .Received()
                .UpdateAsync(Arg.Any<Account>());

            await transactionRepositoryMock
                .Received()
                .AddAsync(Arg.Any<Transaction>());

            await uowMock.Received().CommitAsync();
        }

    }
}
