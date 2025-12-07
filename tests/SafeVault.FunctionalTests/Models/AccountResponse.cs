using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeVault.FunctionalTests.Models
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}
