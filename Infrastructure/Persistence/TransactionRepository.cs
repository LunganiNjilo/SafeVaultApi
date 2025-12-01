using Application.Interfaces;

namespace Infrastructure.Persistence
{
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _db;
        public TransactionRepository(AppDbContext db) => _db = db;

        public async Task<Transaction?> GetByIdAsync(Guid id)
            => await _db.Transactions.Include(t => t.Account).FirstOrDefaultAsync(t => t.Id == id);

        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId, int skip = 0, int take = 50)
            => await _db.Transactions.Where(t => t.AccountId == accountId)
                                     .OrderByDescending(t => t.CreatedAt)
                                     .Skip(skip).Take(take)
                                     .ToListAsync();

        public async Task AddAsync(Transaction tx) => await _db.Transactions.AddAsync(tx);
    }
}
