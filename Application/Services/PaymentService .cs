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
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repo;
        private readonly IHttpPaymentProvider _provider;

        public PaymentService(IPaymentRepository repo, IHttpPaymentProvider provider)
        {
            _repo = repo;
            _provider = provider;
        }

        public async Task<PaymentResult> PayAsync(PayCommand request)
        {
            // Validate amount
            if (request.Amount <= 0)
                throw new ApiException(
                    (int)HttpStatusCode.BadRequest,
                    ErrorType.BadRequest,
                    "Amount must be greater than zero."
                );

            // Fetch account
            var account = await _repo.GetAccountAsync(request.FromAccountId);
            if (account == null)
                throw new ApiException(
                    (int)HttpStatusCode.NotFound,
                    ErrorType.NotFound,
                    "Account not found."
                );

            // Call provider BEFORE touching balance
            var providerResponse = await _provider.SendPaymentAsync();
            if (providerResponse == null || !providerResponse.Success)
            {
                return new PaymentResult
                {
                    Success = false,
                    Message = "Payment provider declined the payment."
                };
            }

            // Apply business rules in domain
            try
            {
                account.Debit(request.Amount);
            }
            catch (DomainException ex)
            {
                throw new ApiException(
                    (int)HttpStatusCode.BadRequest,
                    ErrorType.BadRequest,
                    ex.Message
                );
            }

            // Update balance (your current repo DOES save immediately)
            await _repo.UpdateBalanceAsync(account, request.Amount);

            // Record transaction (your repo ALSO saves immediately)
            var tx = new Transaction
            {
                AccountId = account.Id,
                Amount = -request.Amount,
                Description = request.Description,
                Type = TransactionType.Debit,
                BalanceAfter = account.Balance,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddTransactionAsync(tx);

            // Return final result
            return new PaymentResult
            {
                Success = true,
                TransactionId = providerResponse.TransactionId,
                NewBalance = account.Balance,
                Message = providerResponse.Message
            };
        }

    }

}
