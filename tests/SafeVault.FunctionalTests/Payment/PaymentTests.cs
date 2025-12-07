using SafeVault.FunctionalTests.Models;
using SafeVaultApi.FunctionalTests;
using System.Net;
using System.Net.Http.Json;

namespace SafeVault.FunctionalTests.Payment
{

    [TestFixture]
    public class PaymentTests : FunctionalTestBase
    {
        [Test]
        public async Task Pay_ShouldReturnSuccess_AndReduceBalance()
        {
            // Arrange
            var (login, account) = await LoginAndGetFirstAccountAsync();

            var initialBalance = await GetBalanceAsync(account.AccountNumber);
            var paymentAmount = 25m;

            var request = new PayRequest
            {
                FromAccountId = account.Id,
                Amount = paymentAmount,
                Description = "External Payment Test"
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/payment/pay", request);

            // Assert: API response success
            Assert.That(response.IsSuccessStatusCode,
                Is.True, "External payment should return 200 OK on success");

            var result = await response.Content.ReadFromJsonAsync<PaymentResult>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Success, Is.True, "Result.Success should be true");
            Assert.That(result.TransactionId, Is.Not.Null.And.Not.Empty,
                "TransactionId should be returned");
            Assert.That(result.NewBalance, Is.Not.Null, "NewBalance should be populated");
            Assert.That(result.Message, Is.Not.Null.And.Not.Empty);

            TestContext.Out.WriteLine($"Payment result: {result.Message}");
            TestContext.Out.WriteLine($"NewBalance returned: {result.NewBalance}");

            // Assert: Balance updated correctly on server
            var newBalance = await GetBalanceAsync(account.AccountNumber);

            Assert.That(newBalance,
                Is.EqualTo(initialBalance - paymentAmount),
                "Balance after payment must decrease by payment amount");
        }

        [Test]
        public async Task Pay_With_Insufficient_Funds_ShouldFail()
        {
            // Arrange
            var (login, account) = await LoginAndGetFirstAccountAsync();

            var currentBalance = await GetBalanceAsync(account.AccountNumber);
            var tooMuch = currentBalance + 10_000m; // way more than available

            var request = new PayRequest
            {
                FromAccountId = account.Id,
                Amount = tooMuch,
                Description = "Attempt overdraft"
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/payment/pay", request);

            // Assert: status code
            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.BadRequest),
                "Insufficient funds should return 400 BadRequest");

            var result = await response.Content.ReadFromJsonAsync<PaymentResult>();

            TestContext.Out.WriteLine("Insufficient funds response:");
            TestContext.Out.WriteLine(await response.Content.ReadAsStringAsync());

            // Body assertions based on PaymentResult
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Success, Is.False, "Success should be false on failure");
            // Message content is implementation-specific, so just ensure something is there
            Assert.That(result.Message, Is.Not.Null.And.Not.Empty);

            // Balance should remain unchanged
            var finalBalance = await GetBalanceAsync(account.AccountNumber);
            Assert.That(finalBalance,
                Is.EqualTo(currentBalance),
                "Balance should not change when payment fails");
        }
    }
}
