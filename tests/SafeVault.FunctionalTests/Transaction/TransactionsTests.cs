using SafeVault.FunctionalTests.Models;
using SafeVaultApi.FunctionalTests;
using System.Net.Http.Json;

namespace SafeVault.FunctionalTests.Transaction
{
    [TestFixture]
    public class TransactionsTests : FunctionalTestBase
    {
        [Test]
        public async Task GetTransactions_ShouldReturnList_AndContainRecentActivity()
        {
            // Arrange: ensure user + account + credit exists
            var (login, account) = await LoginAndGetFirstAccountAsync();

            decimal depositAmount = 20m;
            await Client.PostAsJsonAsync(
                $"/api/accounts/{account.AccountNumber}/credit",
                new { Amount = depositAmount });

            // Act: request history for this account
            var response = await Client.GetAsync(
                $"/api/transactions/{account.Id}?skip=0&take=50");

            Assert.That(response.IsSuccessStatusCode, Is.True,
                "GET /transactions must return 200 OK");

            var items = await response.Content.ReadFromJsonAsync<List<TransactionResponse>>();

            // Validate list response
            Assert.That(items, Is.Not.Null.And.Not.Empty,
                "There must be at least 1 transaction");

            var latest = items!.First();

            // Assert expected result fields
            Assert.That(latest.Amount,
                Is.EqualTo(depositAmount), "Last transaction amount should match deposit");

            Assert.That(latest.Type,
                Is.EqualTo("Credit").Or.EqualTo("Deposit"),
                "Transaction type expected");

            Assert.That(latest.Description,
                Is.Not.Null.And.Not.Empty);

            Assert.That(latest.CreatedAt.Date,
                Is.EqualTo(DateTime.UtcNow.Date),
                "Transaction date must match today's date");

            TestContext.Out.WriteLine($"Returned {items.Count} transaction(s)");
            TestContext.Out.WriteLine($"Latest: {latest.Id} | {latest.Amount} | {latest.Type}");
        }

        [Test]
        public async Task GetTransactions_InvalidAccount_ReturnsEmptyArray()
        {
            // Arrange: random account Guid
            var fakeAccount = Guid.NewGuid();

            // Act
            var response = await Client.GetAsync(
                $"/api/transactions/{fakeAccount}");

            Assert.That(response.IsSuccessStatusCode,
                Is.True,
                "Should return 200 even if no items exist");

            var items = await response.Content.ReadFromJsonAsync<List<TransactionResponse>>();

            Assert.That(items, Is.Not.Null);
            Assert.That(items, Is.Empty, "No transactions should exist for invalid account");
        }
    }
}
