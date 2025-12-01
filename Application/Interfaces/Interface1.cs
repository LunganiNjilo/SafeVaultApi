using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Account?> GetAccountAsync(Guid accountId);
        Task UpdateBalanceAsync(Account account, decimal amount);
        Task AddTransactionAsync(Transaction tx);
    }
}
