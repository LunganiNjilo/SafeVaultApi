using SafeVault.FunctionalTests.Models;
using SafeVaultApi.FunctionalTests;
using System.Net;
using System.Net.Http.Json;

namespace SafeVault.FunctionalTests.User
{
    public class UserProfileUpdateTests : FunctionalTestBase
    {
        [Test]
        public async Task UpdateProfile_Should_Succeed_And_Persist_Changes()
        {
            // Login first
            var login = await LoginAsync();

            // Deterministic changes that won’t break login
            var updatedFirstName = "UpdatedName";
            var updatedLastName = "UpdatedLast";
            var existingEmail = login.Email; // keep same email

            var body = new UpdateUserRequest
            {
                UserId = login.UserId,
                FirstName = updatedFirstName,
                LastName = updatedLastName,
                Email = existingEmail,
                IdNumber = "9001015800081",
                Password = null // unchanged
            };

            // Act
            var response = await Client.PutAsJsonAsync("/api/users/update-profile", body);

            // Assert HTTP OK
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var responseJson = await response.Content.ReadAsStringAsync();
            TestContext.Out.WriteLine($"API Response: {responseJson}");

            Assert.That(responseJson, Does.Contain("success"));
            Assert.That(responseJson, Does.Contain("Profile updated successfully"));

            // Fetch accounts → verify profile data reflects new name
            var accounts = (await GetAccountsForUserAsync(login.UserId)).ToList();
            Assert.That(accounts.Count, Is.GreaterThan(0));

            // Re-login: user must still authenticate with same credentials
            var loginAgain = await LoginAsync(existingEmail, "Pass@123");

            Assert.That(loginAgain.Email, Is.EqualTo(existingEmail));
            Assert.That(loginAgain.FirstName, Is.EqualTo(updatedFirstName));
            Assert.That(loginAgain.LastName, Is.EqualTo(updatedLastName));
        }
    }
}
