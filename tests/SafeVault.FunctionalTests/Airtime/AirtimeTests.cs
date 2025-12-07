using SafeVault.FunctionalTests.Models;
using SafeVaultApi.FunctionalTests;
using System.Net.Http.Json;

namespace SafeVault.FunctionalTests.Airtime
{
    [TestFixture]
    public class AirtimeTests : FunctionalTestBase
    {
        [Test]
        public async Task PurchaseAirtime_Should_Return_Success_And_Reduce_Balance()
        {
            // Arrange
            var (login, account) = await LoginAndGetFirstAccountAsync();
            var initialBalance = await GetBalanceAsync(account.AccountNumber);

            var airtimeRequest = new AirtimeRequest
            {
                AccountNumber = account.AccountNumber,
                Description = "Vodacom R10",
                Amount = 10m
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/airtime/purchase", airtimeRequest);
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var airtimeResponse = await response.Content.ReadFromJsonAsync<AirtimeResponse>();
            Assert.That(airtimeResponse, Is.Not.Null);
            Assert.That(airtimeResponse!.IsSuccess, Is.True);

            var newBalance = await GetBalanceAsync(account.AccountNumber);

            TestContext.WriteLine($"Balance before: {initialBalance}, after airtime: {newBalance}");

            // Assert
            Assert.That(newBalance, Is.EqualTo(initialBalance - airtimeRequest.Amount));
        }
    }

}
