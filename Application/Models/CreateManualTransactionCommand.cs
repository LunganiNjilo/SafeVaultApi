using Domain.Enums;

namespace Application.Models
{
    public class CreateManualTransactionCommand
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; } // Debit/Credit
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
