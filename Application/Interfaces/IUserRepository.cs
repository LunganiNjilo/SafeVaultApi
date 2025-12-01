using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByNumberAsync(string accountNumber);
        Task AddAsync(User account);
        Task UpdateAsync(User account);
    }
}
