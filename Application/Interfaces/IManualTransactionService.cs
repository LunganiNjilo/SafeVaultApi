using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IManualTransactionService
    {
        Task<UserServiceResult> CreateManualAsync(CreateManualTransactionCommand command);
        Task<UserServiceResult> UpdateManualAsync(UpdateManualTransactionCommand command);
        Task<UserServiceResult> DeleteManualAsync(Guid transactionId);
    }

}
