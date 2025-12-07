using SafeVault.FunctionalTests.Models;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SafeVaultApi.FunctionalTests;

public abstract class FunctionalTestBase : IDisposable
{
    protected HttpClient Client = default!;

    // 🔐 These must match your DbInitializer seeded user
    // Go to Infrastructure.Persistence.DbInitializer and ensure you have a user
    // with these credentials, OR change these constants to match your seed values.
    private const string TestUserEmail = "test@safesystems.dev";   // <-- CHANGE IF NEEDED
    private const string TestUserPassword = "Pass@123";      // <-- CHANGE IF NEEDED

    protected FunctionalTestBase()
    {
        // We don't do work in the constructor; HttpClient is created in [SetUp].
    }

    [NUnit.Framework.SetUp]
    public void Setup()
    {
        Client = new HttpClient
        {
            BaseAddress = new Uri("http://safevault-api:8080")
        };
    }

    [NUnit.Framework.TearDown]
    public void TearDown()
    {
        Client?.Dispose();
    }

    public void Dispose()
    {
        Client?.Dispose();
    }

    /// <summary>
    /// Logs in using the seeded test user credentials and returns the login response.
    /// Also configures the Authorization header on the HttpClient (for when you add auth).
    /// </summary>
    protected async Task<LoginResponse> LoginAsync(string? userEmail=null, string? testUserPassword=null)
    {
        var request = new LoginRequest
        {
            Email = userEmail??TestUserEmail,
            Password = testUserPassword??TestUserPassword
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception(
                $"Login failed. Status: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
        }

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (loginResponse == null)
        {
            throw new Exception("Failed to deserialize LoginResponse.");
        }

        if (!string.IsNullOrWhiteSpace(loginResponse.Token))
        {
            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", loginResponse.Token);
        }

        return loginResponse;
    }

    /// <summary>
    /// Fetches all accounts for a specific user Id.
    /// </summary>
    protected async Task<List<AccountResponse>> GetAccountsForUserAsync(Guid userId)
    {
        var response = await Client.GetAsync($"/api/accounts/userId/{userId}");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception(
                $"GetAccountsForUserAsync failed. Status: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
        }

        var accounts = await response.Content.ReadFromJsonAsync<List<AccountResponse>>();

        return accounts ?? new List<AccountResponse>();
    }

    /// <summary>
    /// Gets the current balance for a given account number.
    /// </summary>
    protected async Task<decimal> GetBalanceAsync(string accountNumber)
    {
        var response = await Client.GetAsync($"/api/accounts/{accountNumber}/balance");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception(
                $"GetBalanceAsync failed. Status: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
        }

        var content = await response.Content.ReadAsStringAsync();

        if (!decimal.TryParse(content, NumberStyles.Number, CultureInfo.InvariantCulture, out var balance))
        {
            throw new Exception($"Failed to parse balance from response: {content}");
        }

        return balance;
    }

    /// <summary>
    /// Credits an amount to the given account number and returns the transaction.
    /// </summary>
    protected async Task<TransactionResponse> CreditAsync(string accountNumber, decimal amount)
    {
        var payload = new
        {
            Amount = amount
        };

        var response = await Client.PostAsJsonAsync($"/api/accounts/{accountNumber}/credit", payload);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception(
                $"CreditAsync failed. Status: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
        }

        var tx = await response.Content.ReadFromJsonAsync<TransactionResponse>();

        if (tx == null)
        {
            throw new Exception("Failed to deserialize TransactionResponse from credit endpoint.");
        }

        return tx;
    }

    /// <summary>
    /// Debits an amount from the given account number and returns the transaction.
    /// </summary>
    protected async Task<TransactionResponse> DebitAsync(string accountNumber, decimal amount)
    {
        var payload = new
        {
            Amount = amount
        };

        var response = await Client.PostAsJsonAsync($"/api/accounts/{accountNumber}/debit", payload);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception(
                $"DebitAsync failed. Status: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
        }

        var tx = await response.Content.ReadFromJsonAsync<TransactionResponse>();

        if (tx == null)
        {
            throw new Exception("Failed to deserialize TransactionResponse from debit endpoint.");
        }

        return tx;
    }

    /// <summary>
    /// Closes the account using its Id (not account number).
    /// </summary>
    protected async Task CloseAccountAsync(Guid accountId)
    {
        var response = await Client.PostAsync($"/api/accounts/{accountId}/close", content: null);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception(
                $"CloseAccountAsync failed. Status: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
        }
    }

    protected async Task<(LoginResponse login, AccountResponse account)> LoginAndGetFirstAccountAsync()
    {
        var login = await LoginAsync();

        var accounts = await GetAccountsForUserAsync(login.UserId);
        Assert.That(accounts, Is.Not.Null);
        Assert.That(accounts.Count, Is.GreaterThan(0), "User must have at least one account.");

        var account = accounts[0];

        TestContext.Out.WriteLine(
            $"Authenticated user: {login.Email}, using account: {account.AccountNumber} ({account.Name})");

        return (login, account);
    }

}
