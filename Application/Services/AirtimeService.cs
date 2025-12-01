using Application.Common.Exceptions;
using Application.Enums;
using Application.Interfaces;
using System.Net;

namespace Application.Services
{
    public class AirtimeService : IAirtimeService
    {
        private readonly IAccountService _accounts;

        public AirtimeService(IAccountService accounts)
        {
            _accounts = accounts;
        }

        public async Task<bool> PurchaseAirtimeAsync(
            string accountNumber,
            string phoneNumber,
            decimal amount)
        {
            try
            {
                // Debit the amount from the customer account
                await _accounts.DebitAsync(
                    accountNumber,
                    amount,
                    $"Airtime purchase for {phoneNumber}"
                );

                return true;
            }
            catch (Exception ex)
            {
                // Centralized error reporting
                throw new ApiException((int)HttpStatusCode.InternalServerError, ErrorType.InternalServerError, $"Fail to purchase airtime: {ex.Message}");
            }
        }
    }
}


