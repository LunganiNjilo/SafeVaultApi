using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid id);
        Task<Account?> GetByNumberAsync(string accountNumber);
        Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId);

        Task<Account?> GetForUpdateAsync(Guid id);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task SaveChangesAsync();

        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}
