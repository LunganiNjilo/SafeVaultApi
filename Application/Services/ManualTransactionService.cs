using Application.Common.Exceptions;
using Application.Enums;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using System.Net;

namespace Application.Services
{
    public class ManualTransactionService : IManualTransactionService
    {
        private readonly IAccountRepository _accounts;
        private readonly ITransactionRepository _transactions;
        private readonly IUnitOfWork _unitOfWork;

        public ManualTransactionService(
            IAccountRepository accounts,
            ITransactionRepository transactions,
            IUnitOfWork unitOfWork)
        {
            _accounts = accounts;
            _transactions = transactions;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserServiceResult> CreateManualAsync(CreateManualTransactionCommand cmd)
        {
            var account = await _accounts.GetByIdAsync(cmd.AccountId);

            if (account == null)
                throw new ApiException((int)HttpStatusCode.NotFound, ErrorType.NotFound, "Account not found.");

            if (account.IsClosed)
                throw new ApiException((int)HttpStatusCode.BadRequest, ErrorType.BadRequest, "Cannot post transactions to a closed account.");

            if (cmd.TransactionDate > DateTime.UtcNow)
                throw new ApiException((int)HttpStatusCode.BadRequest, ErrorType.BadRequest, "Transaction date cannot be in the future.");

            if (cmd.Amount <= 0)
                throw new ApiException((int)HttpStatusCode.BadRequest, ErrorType.BadRequest, "Amount must be greater than zero.");

            // --- Domain Behavior ---
            account.ApplyManualTransaction(cmd.Type, cmd.Amount);

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = cmd.Amount,
                Type = cmd.Type,
                Description = cmd.Description,
                CreatedAt = cmd.TransactionDate,
                BalanceAfter = account.Balance,
                IsManual = true
            };

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _transactions.AddAsync(transaction);
                await _accounts.UpdateAsync(account);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return new UserServiceResult
            {
                Success = true,
                Message = "Manual transaction created"
            };
        }


        public async Task<UserServiceResult> UpdateManualAsync(UpdateManualTransactionCommand cmd)
        {
            var transaction = await _transactions.GetByIdAsync(cmd.TransactionId);

            if (transaction == null)
                throw new ApiException((int)HttpStatusCode.NotFound, ErrorType.NotFound, "Transaction not found.");

            if (!transaction.IsManual)
                throw new ApiException((int)HttpStatusCode.BadRequest, ErrorType.BadRequest, "This transaction cannot be edited.");

            transaction.Amount = cmd.Amount;
            transaction.Type = cmd.Type;
            transaction.Description = cmd.Description;
            transaction.CreatedAt = DateTime.UtcNow; // capture update

            await _transactions.UpdateAsync(transaction);

            return new UserServiceResult { Success = true, Message = "Manual transaction updated" };
        }

        public async Task<UserServiceResult> DeleteManualAsync(Guid transactionId)
        {
            var transaction = await _transactions.GetByIdAsync(transactionId);

            if (transaction == null)
                throw new ApiException((int)HttpStatusCode.NotFound, ErrorType.NotFound, "Transaction not found.");

            if (!transaction.IsManual)
                throw new ApiException((int)HttpStatusCode.BadRequest, ErrorType.BadRequest, "This transaction cannot be deleted.");

            await _transactions.DeleteAsync(transaction);

            return new UserServiceResult { Success = true, Message = "Manual transaction deleted" };
        }
    }

}
