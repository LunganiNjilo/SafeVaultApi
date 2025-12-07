using SafeVault.FunctionalTests.Models;
using SafeVaultApi.FunctionalTests;
using System.Net;
using System.Net.Http.Json;

namespace SafeVault.FunctionalTests.Auth
{
    [TestFixture]
    public class AuthTests : FunctionalTestBase
    {
        [Test]
        public async Task Login_With_Valid_Credentials_Should_Return_User()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@safesystems.dev",     
                Password = "Pass@123"
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/auth/login", request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected login to succeed.");

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            Assert.That(loginResponse, Is.Not.Null, "Login response must not be null.");

            Assert.That(loginResponse!.UserId, Is.Not.EqualTo(Guid.Empty), "UserId should not be empty.");
            Assert.That(loginResponse.Email, Is.EqualTo(request.Email), "Returned email should match the login request.");

            TestContext.WriteLine($"Logged in user: {loginResponse.FirstName} {loginResponse.LastName} ({loginResponse.Email})");
        }

        [Test]
        public async Task Login_With_Invalid_Credentials_Should_Return_Unauthorized()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "does-not-exist@example.com",
                Password = "WrongPassword123!"
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/auth/login", request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

            var body = await response.Content.ReadAsStringAsync();
            TestContext.Out.WriteLine($"Unauthorized body: {body}");
        }
    }
}
