using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _db;

        public PaymentRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<Account?> GetAccountAsync(Guid accountId)
        {
            return _db.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);
        }

        public async Task UpdateBalanceAsync(Account account, decimal amount)
        {
            account.Balance -= amount;
            await _db.SaveChangesAsync();
        }

        public async Task AddTransactionAsync(Transaction tx)
        {
            _db.Transactions.Add(tx);
            await _db.SaveChangesAsync();
        }
    }
}
