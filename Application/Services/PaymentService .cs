using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;

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
            var account = await _repo.GetAccountAsync(request.FromAccountId);

            if (account == null)
                return new PaymentResult { Success = false, Message = "Account not found" };

            if (request.Amount <= 0)
                return new PaymentResult { Success = false, Message = "Invalid amount" };

            if (account.Balance < request.Amount)
                return new PaymentResult { Success = false, Message = "Insufficient balance" };

            // Call mock provider
            var providerResponse = await _provider.SendPaymentAsync();

            if (providerResponse == null || providerResponse.Success == false)
                return new PaymentResult { Success = false, Message = "Provider declined payment" };

            // Update balance
            await _repo.UpdateBalanceAsync(account, request.Amount);

            // Add transaction
            await _repo.AddTransactionAsync(new Transaction
            {
                AccountId = account.Id,
                Amount = -request.Amount,
                Description = request.Description,
                Type = TransactionType.Debit,
                BalanceAfter = account.Balance,
                CreatedAt = DateTime.UtcNow
            });

            return new PaymentResult
            {
                Success = true,
                Message = providerResponse.Message,
                TransactionId = providerResponse.TransactionId,
                NewBalance = account.Balance
            };
        }
    }

}
