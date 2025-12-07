using Application.Models;
using NUnit.Framework;
using SafeVault.FunctionalTests.Models;
using SafeVaultApi.FunctionalTests;
using System.Linq;
using System.Net.Http.Json;
using TransferAccountResult = SafeVault.FunctionalTests.Models.TransferAccountResult;

namespace SafeVault.FunctionalTests.Transfers
{
    public class AccountTransferTests : FunctionalTestBase
    {
        [Test]
        public async Task Transfer_Should_Move_Funds_Between_Accounts()
        {
            // Login first
            var login = await LoginAsync();

            // Fetch accounts for this authenticated user
            var accounts = (await GetAccountsForUserAsync(login.UserId)).ToList();
            Assert.That(accounts.Any(), Is.True, "User must have accounts");

            // Pick valid accounts
            var fromAccount = accounts.First(a => a.Balance >= 100);
            var toAccount = accounts.First(a => a.Id != fromAccount.Id);

            // Current balances
            var initialFromBalance = await GetBalanceAsync(fromAccount.AccountNumber);
            var initialToBalance = await GetBalanceAsync(toAccount.AccountNumber);

            const decimal transferAmount = 50m;

            var body = new TransferRequest
            {
                FromAccountId = fromAccount.AccountNumber.ToString(),
                ToAccountId = toAccount.AccountNumber.ToString(),
                Amount = transferAmount
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/transfers", body);

            // Assert response success
            Assert.That(response.IsSuccessStatusCode,
                Is.True, "Transfer should return OK on success");

            var result = await response.Content.ReadFromJsonAsync<TransferAccountResult>();
            Assert.That(result, Is.Not.Null, "Response must deserialize");
            Assert.That(result!.IsSuccess, Is.True, "Result should indicate success");

            TestContext.Out.WriteLine($"Transfer Message: {result.Message}");
            TestContext.Out.WriteLine($"New Balance From: {result.FromAccountBalance}");
            TestContext.Out.WriteLine($"New Balance To: {result.ToAccountBalance}");

            // Validate returned balances
            Assert.That(result.FromAccountBalance,
                Is.EqualTo(initialFromBalance - transferAmount));

            Assert.That(result.ToAccountBalance,
                Is.EqualTo(initialToBalance + transferAmount));

            // Confirm server state
            var finalFromBalance = await GetBalanceAsync(fromAccount.AccountNumber);
            var finalToBalance = await GetBalanceAsync(toAccount.AccountNumber);

            Assert.That(finalFromBalance, Is.EqualTo(result.FromAccountBalance));
            Assert.That(finalToBalance, Is.EqualTo(result.ToAccountBalance));
        }
    }
}
