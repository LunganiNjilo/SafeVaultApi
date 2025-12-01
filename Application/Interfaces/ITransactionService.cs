using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetByAccountAsync(
            Guid accountId,
            int skip = 0,
            int take = 50
        );
    }
}
