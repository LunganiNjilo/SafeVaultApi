using Application.Models;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserServiceResult> UpdateUserProfileAsync(UpdateUserCommand request);
    }
}
