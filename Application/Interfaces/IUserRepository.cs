using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User account);
        Task UpdateAsync(User account);
        Task DeleteAsync(User account);
        Task<User?> GetByIdNumberAsync(string idNumber);
    }
}
