using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(string email, string password);
    }
}
