using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeVault.FunctionalTests.Models
{
    public class TransactionResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public decimal BalanceAfter { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
