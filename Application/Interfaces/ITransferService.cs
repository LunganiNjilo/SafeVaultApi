using Application.Models;

namespace Application.Interfaces
{
    public interface ITransferService
    {
        Task<TransferAccountResult> TransferAsync(TransferFundsCommand request);
    }
}
