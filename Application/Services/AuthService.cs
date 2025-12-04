using Application.Interfaces;
using Application.Models;
using Domain.Entities;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher _hasher;

        public AuthService(IUserRepository users, IPasswordHasher hasher)
        {
            _users = users;
            _hasher = hasher;
        }

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            var user = await _users.GetByEmailAsync(email);

            if (user == null)
                return new LoginResult { Success = false };

            if (!_hasher.Verify(password, user.PasswordHash))
                return new LoginResult { Success = false };

            return new LoginResult
            {
                Success = true,
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
        }
    }
}
