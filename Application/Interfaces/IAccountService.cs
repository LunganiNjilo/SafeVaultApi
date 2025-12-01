using Domain.Entities;
namespace Application.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId);
        Task<decimal> GetBalanceAsync(string accountNumber);
        Task<Transaction> CreditAsync(string accountNumber, decimal amount, string description);
        Task<Transaction> DebitAsync(string accountNumber, decimal amount, string description);
    }
}
