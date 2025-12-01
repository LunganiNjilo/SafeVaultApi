using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Account
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public string AccountNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Balance { get; set; } = 0M;
        public string Currency { get; set; } = "ZAR";
        public AccountType AccountType { get; set; } = AccountType.Current;

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public void Debit(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Amount must be greater than zero");

            if (Balance < amount)
                throw new InvalidOperationException("Insufficient funds");

            Balance -= amount;
        }

        public void Credit(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Amount must be greater than zero");

            Balance += amount;
        }

    }
}
