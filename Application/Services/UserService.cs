using Application.Common.Exceptions;
using Application.Enums;
using Application.Interfaces;
using Application.Models;
using System.Net;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IAccountRepository accountRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserServiceResult> UpdateUserProfileAsync(UpdateUserCommand request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                 throw new ApiException((int)HttpStatusCode.NotFound, ErrorType.NotFound, "User not found");
           

            if (!string.IsNullOrWhiteSpace(request.IdNumber))
            {
                var existing = await _userRepository.GetByIdNumberAsync(request.IdNumber);

                if (existing != null && existing.Id != request.UserId)
                {
                    throw new ApiException((int)HttpStatusCode.BadRequest, ErrorType.BadRequest, "ID Number already exists");
                }

                user.IdNumber = request.IdNumber;
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.PasswordHash = _passwordHasher.Hash(request.Password);
            }

            await _userRepository.UpdateAsync(user);

            return new UserServiceResult { Success = true, Message = "Profile updated successfully" };
        }

        public async Task<UserServiceResult> DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new ApiException(
                    (int)HttpStatusCode.NotFound,
                    ErrorType.NotFound,
                    "User not found."
                );

            var hasOpenAccounts = await _accountRepository.UserHasOpenAccountsAsync(userId);

            if (hasOpenAccounts)
                throw new ApiException(
                    (int)HttpStatusCode.BadRequest,
                    ErrorType.BadRequest,
                    "User cannot be deleted because they have open accounts."
                );

            await _userRepository.DeleteAsync(user);

            return new UserServiceResult
            {
                Success = true,
                Message = "User deleted successfully"
            };
        }

    }
}
