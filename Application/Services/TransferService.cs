using Application.Common.Exceptions;
using Application.Enums;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using System.Net;

namespace Application.Services
{
    public class TransferService : ITransferService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactions;
        private readonly IUnitOfWork _uow;

        public TransferService(IAccountRepository accountRepository, ITransactionRepository transactions, IUnitOfWork uow)
        {
            _accountRepository = accountRepository;
            _transactions = transactions;
            _uow = uow;
        }

        public async Task<TransferAccountResult> TransferAsync(TransferFundsCommand command)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var fromAccount = await _accountRepository.GetByNumberAsync(command.FromAccountId.ToString());

                var toAccount = await _accountRepository.GetByNumberAsync(command.ToAccountId.ToString());

                if (fromAccount == null || toAccount == null)
                { throw new ApiException((int)HttpStatusCode.NotFound, ErrorType.NotFound, "Account not found."); }

                if (fromAccount.Balance < command.Amount)
                { throw new ApiException((int)HttpStatusCode.BadRequest, ErrorType.BadRequest, "Insufficient funds."); }

                fromAccount.Balance -= command.Amount;
                toAccount.Balance += command.Amount;

                await _accountRepository.UpdateAsync(fromAccount);
                await _accountRepository.UpdateAsync(toAccount);

                await _transactions.AddAsync(new Transaction
                {
                    AccountId = fromAccount.Id,
                    Amount = command.Amount,
                    Type = TransactionType.Debit,
                    Description = "Transfer Debit",
                    BalanceAfter = fromAccount.Balance
                });

                await _transactions.AddAsync(new Transaction
                {
                    AccountId = toAccount.Id,
                    Amount = command.Amount,
                    Type = TransactionType.Credit,
                    Description = "Transfer Credit",
                    BalanceAfter = toAccount.Balance
                });

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();


                return new TransferAccountResult { 
                    IsSuccess = true, 
                    Message = "Transfer successful", 
                    FromAccountBalance = fromAccount.Balance ,
                    ToAccountBalance = toAccount.Balance, 
                    Amount = command.Amount 
                };
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                throw new ApiException((int)HttpStatusCode.BadRequest, ErrorType.BadRequest, $"Transfer failed: {ex.Message}");
            }
        }
    }
}
