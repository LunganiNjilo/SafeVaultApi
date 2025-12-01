using Application.Interfaces;
using Application.Models;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserServiceResult> UpdateUserProfileAsync(UpdateUserCommand request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                return new UserServiceResult { Success = false, Message = "User not found" };

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;

            // Only update password if provided
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.PasswordHash = _passwordHasher.Hash(request.Password);
            }

            await _userRepository.UpdateAsync(user);

            return new UserServiceResult { Success = true, Message = "Profile updated successfully" };
        }
    }


}
