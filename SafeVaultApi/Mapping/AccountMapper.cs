using Domain.Entities;
using SafeVaultApi.Models.Response;

namespace SafeVaultApi.Mapping
{
    public class AccountMapper
    {
        public static AccountResponse ToResponse(Account account)
        {
            return new AccountResponse
            {
                Id = account.Id,
                Name = account.Name,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                Currency = account.Currency
            };
        }

        public static IEnumerable<AccountResponse> ToResponse(IEnumerable<Account> accounts)
        {
            return accounts.Select(ToResponse);
        }
    }
}
