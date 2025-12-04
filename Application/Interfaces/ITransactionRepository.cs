using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(Guid id);
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId, int skip = 0, int take = 50);
        Task AddAsync(Transaction tx);
        Task UpdateAsync(Transaction tx);
        Task DeleteAsync(Transaction tx);

        Task<List<Transaction>> GetByAccountAsync(Guid accountId);
    }
}
