using Domain.Enums;

namespace SafeVaultApi.Models.Request
{
    public class CreateManualTransactionRequest
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
