using Application.Models;
using SafeVaultApi.Models.Request;

namespace SafeVaultApi.Mapping
{
    public static class TransferMapper
    {
        public static TransferFundsCommand FromTransferFundsRequest(TransferRequest request)
        {
           return new TransferFundsCommand
            {
                FromAccountId = request.FromAccountId,
                ToAccountId = request.ToAccountId,
                Amount = request.Amount,
            };

        }
    }
}
