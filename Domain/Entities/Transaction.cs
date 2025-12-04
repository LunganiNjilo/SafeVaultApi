using Domain.Enums;

namespace Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal BalanceAfter { get; set; }

        public bool IsManual { get; set; } = false;
    }
}
