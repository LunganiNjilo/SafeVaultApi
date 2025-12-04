using Application.Common.Exceptions;
using Application.Enums;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
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
            return await PerformTransaction(number, amount, description, TransactionType.Debit);
        }

        public async Task<UserServiceResult> CloseAccountAsync(Guid accountId)
        {
            var account = await _accounts.GetByIdAsync(accountId);

            if (account == null)
                throw new ApiException(
                    (int)HttpStatusCode.NotFound,
                    ErrorType.NotFound,
                    "Account not found."
                );

            if (account.Balance != 0)
                throw new ApiException(
                    (int)HttpStatusCode.BadRequest,
                    ErrorType.BadRequest,
                    "Cannot close an account with non-zero balance."
                );

            account.IsClosed = true;

            await _accounts.UpdateAsync(account);

            return new UserServiceResult
            {
                Success = true,
                Message = "Account closed successfully"
            };
        }

        public Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId)
        {
            return _accounts.GetByUserIdAsync(userId);
        }

        private async Task<Transaction> PerformTransaction(string number,decimal amount,string description,TransactionType type)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var acct = await _accounts.GetByAccountNumberAsync(number)
                    ?? throw new ApiException((int)HttpStatusCode.NotFound, ErrorType.NotFound, "Account not found.");

                switch (type)
                {
                    case TransactionType.Credit:
                        acct.Credit(amount);
                        break;

                    case TransactionType.Debit:
                        acct.Debit(amount);
                        break;
                }

                // Track changes
                await _accounts.UpdateAsync(acct);

                // Create transaction entry
                var tx = new Transaction
                {
                    AccountId = acct.Id,
                    Amount = amount,
                    Fee = 0M,
                    BalanceAfter = acct.Balance,
                    Type = type,
                    Description = description,
                    CreatedAt = DateTime.UtcNow
                };

                await _transactions.AddAsync(tx);

                // Commit UoW
                await _uow.CommitAsync();

                return tx;
            }
            catch (DomainException ex)
            {
                await _uow.RollbackAsync();
                throw new ApiException(
                    (int)HttpStatusCode.BadRequest,
                    ErrorType.BadRequest,
                    ex.Message
                );
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
    }
}
