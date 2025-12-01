using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string email, string password);
    }
}
