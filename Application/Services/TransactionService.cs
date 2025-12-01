using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repo;

        public TransactionService(ITransactionRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Transaction>> GetByAccountAsync(Guid accountId, int skip = 0, int take = 50)
            => _repo.GetByAccountIdAsync(accountId, skip, take);
    }
}
