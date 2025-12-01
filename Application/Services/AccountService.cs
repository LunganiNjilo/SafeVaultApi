using Application.Common.Exceptions;
using Application.Enums;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using System.Net;

namespace Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accounts;
        private readonly ITransactionRepository _transactions;
        private readonly IUnitOfWork _uow;

        public AccountService(IAccountRepository accounts,
                              ITransactionRepository transactions,
                              IUnitOfWork uow)
        {
            _accounts = accounts;
            _transactions = transactions;
            _uow = uow;
        }

        public async Task<decimal> GetBalanceAsync(string number)
        {
            var acct = await _accounts.GetByNumberAsync(number)
                ?? throw new ApiException((int)HttpStatusCode.NotFound, ErrorType.NotFound, "Account not found.");
            return acct.Balance;
        }

        public async Task<Transaction> CreditAsync(string number, decimal amount, string description)
        {
            if (amount <= 0) throw new ApiException((int)HttpStatusCode.BadRequest, ErrorType.BadRequest, "Amount must be > 0");
            return await PerformTransaction(number, amount, description, TransactionType.Credit);
        }

        public async Task<Transaction> DebitAsync(string number, decimal amount, string description)
        {
            if (amount <= 0) throw new ApiException((int)HttpStatusCode.BadRequest, ErrorType.BadRequest, "Amount must be > 0");
            return await PerformTransaction(number, -amount, description, TransactionType.Debit);
        }

        private async Task<Transaction> PerformTransaction(string number, decimal amountDelta, string description, TransactionType type)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var acct = await _accounts.GetByNumberAsync(number)
                    ?? throw new ApiException((int)HttpStatusCode.NotFound, ErrorType.NotFound, "Account not found.");

                var newBalance = acct.Balance + amountDelta;
                if (newBalance < 0)
                {
                    throw new ApiException((int)HttpStatusCode.BadRequest, ErrorType.BadRequest, "Insufficient funds.");
                }

                acct.Balance = newBalance;
                await _accounts.UpdateAsync(acct); // repository marks entity as modified in context

                var tx = new Transaction
                {
                    AccountId = acct.Id,
                    Amount = Math.Abs(amountDelta),
                    Fee = 0m,
                    BalanceAfter = acct.Balance,
                    Type = type,
                    Description = description,
                    CreatedAt = DateTime.UtcNow
                };

                await _transactions.AddAsync(tx);

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                return tx;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId)
        {
           return _accounts.GetByUserIdAsync(userId);
        }
    }
}
