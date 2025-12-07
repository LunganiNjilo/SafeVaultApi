using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeVault.FunctionalTests.Models
{
    public class TransferAccountResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;

        public decimal FromAccountBalance { get; set; }

        public decimal ToAccountBalance { get; set; }

        public decimal Amount { get; set; }
    }
}
