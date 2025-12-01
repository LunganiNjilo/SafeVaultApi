using Domain.Entities;
using SafeVaultApi.Models.Response;


namespace SafeVaultApi.Mapping
{
    public class TransactionMapper
    {
        public static TransactionResponse ToResponse(Transaction tx)
        {
            return new TransactionResponse
            {
                Id = tx.Id,
                Amount = tx.Amount,
                Fee = tx.Fee,
                BalanceAfter = tx.BalanceAfter,
                Description = tx.Description!,
                Type = tx.Type.ToString(),
                CreatedAt = tx.CreatedAt
            };
        }


        public static IEnumerable<TransactionResponse> ToResponse(IEnumerable<Transaction> transactions)
        {
            return transactions.Select(ToResponse);
        }
    }
}
