using Domain.Entities;
using NSubstitute;
using SafeVaultApi.Models.Request;
using SafeVaultApi.Tests.Infrastructure;
using System.Net;

namespace SafeVaultApi.Tests.Tests.Users
{
    internal class UserControllerTests : TestBase
    {
        private SafeVaultApiClient _client;

        [SetUp]
        public void Setup()
        {
            _client = CreateApiClient<SafeVaultApiClient>();
            accountRepositoryMock.ClearReceivedCalls();
            userRepositoryMock.ClearReceivedCalls();
        }

        [Test]
        public async Task DeleteUser_Should_ReturnOk_When_NoOpenAccounts()
        {
            var user = new User { Id = Guid.NewGuid() };

            userRepositoryMock.GetByIdAsync(user.Id).Returns(user);
            accountRepositoryMock.UserHasOpenAccountsAsync(user.Id).Returns(false);
            userRepositoryMock.DeleteAsync(user).Returns(Task.CompletedTask);

            var response = await _client.DeleteRawAsync($"/api/users/{user.Id}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            await userRepositoryMock.Received().DeleteAsync(user);
        }

        [Test]
        public async Task DeleteUser_Should_ReturnBadRequest_When_UserHasOpenAccounts()
        {
            var user = new User { Id = Guid.NewGuid() };

            userRepositoryMock.GetByIdAsync(user.Id).Returns(user);
            accountRepositoryMock.UserHasOpenAccountsAsync(user.Id).Returns(true);

            var response = await _client.DeleteRawAsync($"/api/users/{user.Id}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            await userRepositoryMock.DidNotReceive().DeleteAsync(Arg.Any<User>());
        }

        [Test]
        public async Task DeleteUser_Should_ReturnNotFound_When_UserDoesNotExist()
        {
            var id = Guid.NewGuid();

            userRepositoryMock.GetByIdAsync(id).Returns((User?)null);

            var response = await _client.DeleteRawAsync($"/api/users/{id}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

            await userRepositoryMock.DidNotReceive().DeleteAsync(Arg.Any<User>());
        }

        [Test]
        public async Task UpdateProfile_Should_ReturnOk_When_Valid()
        {
            var userId = Guid.NewGuid();
            var existingUser = new User { Id = userId };

            userRepositoryMock.GetByIdAsync(userId).Returns(existingUser);
            userRepositoryMock.GetByIdNumberAsync(Arg.Any<string>()).Returns((User?)null);
            userRepositoryMock.UpdateAsync(existingUser).Returns(Task.CompletedTask);

            var request = new UpdateUserRequest
            {
                UserId = userId,
                FirstName = "Updated",
                LastName = "User",
                Email = "new@example.com",
                IdNumber = "1234567890123",
                Password = null
            };

            var response = await _client.PutRawAsync("/api/users/update-profile", request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            await userRepositoryMock.Received().UpdateAsync(Arg.Any<User>());
        }

        [Test]
        public async Task UpdateProfile_Should_ReturnNotFound_When_UserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            userRepositoryMock.GetByIdAsync(userId).Returns((User?)null);

            var request = new UpdateUserRequest
            {
                UserId = userId,
                FirstName = "Fake",
                LastName = "User",
                Email = "none@example.com",
                IdNumber = "0000000000000"
            };

            var response = await _client.PutRawAsync("/api/users/update-profile", request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            await userRepositoryMock.DidNotReceive().UpdateAsync(Arg.Any<User>());
        }

        [Test]
        public async Task UpdateProfile_Should_ReturnBadRequest_When_IdNumber_AlreadyExists()
        {
            var userId = Guid.NewGuid();

            var currentUser = new User { Id = userId };
            var otherUser = new User { Id = Guid.NewGuid() };

            userRepositoryMock.GetByIdAsync(userId).Returns(currentUser);
            userRepositoryMock.GetByIdNumberAsync("8888888888888").Returns(otherUser);

            var request = new UpdateUserRequest
            {
                UserId = userId,
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com",
                IdNumber = "8888888888888"
            };

            var response = await _client.PutRawAsync("/api/users/update-profile", request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            await userRepositoryMock.DidNotReceive().UpdateAsync(Arg.Any<User>());
        }

        [Test]
        public async Task UpdateProfile_Should_Not_Update_Password_When_NullOrWhitespace()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, PasswordHash = "oldhash" };

            userRepositoryMock.GetByIdAsync(userId).Returns(user);
            userRepositoryMock.GetByIdNumberAsync(Arg.Any<string>()).Returns((User?)null);

            var request = new UpdateUserRequest
            {
                UserId = userId,
                FirstName = "Test",
                LastName = "User",
                Email = "something@test.com",
                IdNumber = "1231231231231",
                Password = "" // whitespace means NO password update
            };

            var response = await _client.PutRawAsync("/api/users/update-profile", request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(user.PasswordHash, Is.EqualTo("oldhash")); // unchanged
        }
    }
}
