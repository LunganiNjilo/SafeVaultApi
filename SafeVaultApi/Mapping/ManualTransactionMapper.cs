using Application.Models;
using SafeVaultApi.Models.Request;

namespace SafeVaultApi.Mapping
{
    public static class ManualTransactionMapper
    {
        public static CreateManualTransactionCommand ToCommand(CreateManualTransactionRequest request)
        {
            return new CreateManualTransactionCommand
            {
                AccountId = request.AccountId,
                Amount = request.Amount,
                Type = request.Type,
                Description = request.Description,
                TransactionDate = request.TransactionDate
            };
        }

        public static UpdateManualTransactionCommand ToCommand(UpdateManualTransactionRequest request)
        {
            return new UpdateManualTransactionCommand
            {
                TransactionId = request.TransactionId,
                Amount = request.Amount,
                Type = request.Type,
                Description = request.Description,
                TransactionDate = request.TransactionDate
            };
        }
    }

}
