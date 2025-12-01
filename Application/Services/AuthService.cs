using Application.Interfaces;
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

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _users.GetByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            return _hasher.Verify(password, user.PasswordHash)
                ? user
                : null;
        }
    }
}
