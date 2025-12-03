using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _db;
        public AccountRepository(AppDbContext db) => _db = db;

        public async Task<Account?> GetByIdAsync(Guid id)
            => await _db.Accounts
                .AsNoTracking()
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<Account?> GetByNumberAsync(string accountNumber)
            => await _db.Accounts

                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
            => await _db.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId)
            => await _db.Accounts
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .Include(a => a.Transactions)
                .ToListAsync();
        public async Task<Account?> GetForUpdateAsync(Guid id)
            => await _db.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Id == id);

        public Task AddAsync(Account account)
        {
            _db.Accounts.AddAsync(account);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Account account)
        {
            _db.Accounts.Update(account);
            return Task.CompletedTask;
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                await operation();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
