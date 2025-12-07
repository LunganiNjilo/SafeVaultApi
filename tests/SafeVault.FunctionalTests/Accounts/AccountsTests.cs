using SafeVault.FunctionalTests.Models;
using SafeVaultApi.FunctionalTests;

namespace SafeVault.FunctionalTests.Accounts
{
    [TestFixture]
    public class AccountsTests : FunctionalTestBase
    {
        /// <summary>
        /// Helper: logs in and returns (LoginResponse, first account).
        /// Throws if the user has no accounts.
        /// </summary>
        private async Task<(LoginResponse login, AccountResponse account)> LoginAndGetFirstAccountAsync()
        {
            var login = await LoginAsync();

            var accounts = await GetAccountsForUserAsync(login.UserId);

            Assert.That(accounts.Count, Is.GreaterThan(0), "Seeded user should have at least one account.");

            var first = accounts[0];

            TestContext.Out.WriteLine($"Using account {first.AccountNumber} ({first.Name}) for user {login.Email}");

            return (login, first);
        }

        [Test]
        public async Task GetAccounts_By_UserId_Should_Return_Accounts()
        {
            // Arrange
            var (login, _) = await LoginAndGetFirstAccountAsync();

            // Act
            var accounts = await GetAccountsForUserAsync(login.UserId);

            // Assert
            Assert.That(accounts, Is.Not.Null);
            Assert.That(accounts.Count, Is.GreaterThan(0), "Expected at least one account for the user.");

            var json = System.Text.Json.JsonSerializer.Serialize(accounts);
            TestContext.Out.WriteLine($"Accounts JSON: {json}");
        }

        [Test]
        public async Task GetBalance_Should_Return_Current_Balance()
        {
            // Arrange
            var (_, account) = await LoginAndGetFirstAccountAsync();

            // Act
            var balance = await GetBalanceAsync(account.AccountNumber);

            // Assert
            TestContext.Out.WriteLine($"Balance for {account.AccountNumber}: {balance}");
     
            Assert.That(balance, Is.GreaterThanOrEqualTo(0m));
        }

        [Test]
        public async Task Credit_Should_Increase_Balance_By_Amount()
        {
            // Arrange
            var (_, account) = await LoginAndGetFirstAccountAsync();

            var initialBalance = await GetBalanceAsync(account.AccountNumber);
            var amountToCredit = 100;

            // Act
            var tx = await CreditAsync(account.AccountNumber, amountToCredit);
            var newBalance = await GetBalanceAsync(account.AccountNumber);

            // Assert
            TestContext.Out.WriteLine($"Initial balance: {initialBalance}, credited: {amountToCredit}, new balance: {newBalance}");

            Assert.That(tx.Amount, Is.EqualTo(amountToCredit));
            Assert.That(newBalance, Is.EqualTo(initialBalance + amountToCredit));
        }

        [Test]
        public async Task Debit_Should_Decrease_Balance_By_Amount()
        {
            // Arrange
            var (_, account) = await LoginAndGetFirstAccountAsync();

            // Ensure we have enough balance by crediting first if necessary.
            var currentBalance = await GetBalanceAsync(account.AccountNumber);
            var requiredBalance = 200m;
            if (currentBalance < requiredBalance)
            {
                var topUp = requiredBalance - currentBalance;
                await CreditAsync(account.AccountNumber, topUp);
                currentBalance = await GetBalanceAsync(account.AccountNumber);
            }

            var amountToDebit = 50m;

            // Act
            var tx = await DebitAsync(account.AccountNumber, amountToDebit);
            var newBalance = await GetBalanceAsync(account.AccountNumber);

            // Assert
            TestContext.Out.WriteLine($"Balance before debit: {currentBalance}, debited: {amountToDebit}, new balance: {newBalance}");

            Assert.That(tx.Amount, Is.EqualTo(amountToDebit));
            Assert.That(newBalance, Is.EqualTo(currentBalance - amountToDebit));
        }

        [Test]
        public async Task CloseAccount_WithNonZeroBalance_Should_Return_BadRequest()
        {
            // Arrange - login with the dedicated test user
            var login = await LoginAsync("user@safevault.io", "Pass@123");
            var userId = login.UserId;

            var accounts = await GetAccountsForUserAsync(userId);

            Assert.That(accounts.Count, Is.GreaterThanOrEqualTo(1),
                "Test user must have at least one account.");

            var target = accounts[^1];

            TestContext.Out.WriteLine(
                $"Attempting to close account {target.AccountNumber} ({target.Id}) for {login.Email}");

            // Act + Assert - we EXPECT this to fail with the business rule
            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await CloseAccountAsync(target.Id);
            });

            // Verify the exception contains the expected API response
            Assert.That(ex, Is.Not.Null, "Expected CloseAccountAsync to throw for non-zero balance.");

            Assert.That(ex.Message, Does.Contain("400 BadRequest"));
            Assert.That(ex.Message, Does.Contain("\"errorCode\":\"AV400\""));
            Assert.That(ex.Message, Does.Contain("Cannot close an account with non-zero balance"));
            TestContext.Out.WriteLine($"Got expected failure: {ex!.Message}");
        }
    }
}
