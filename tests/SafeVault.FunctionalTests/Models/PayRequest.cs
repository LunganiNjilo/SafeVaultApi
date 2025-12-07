using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeVault.FunctionalTests.Models
{
    public class PayRequest
    {
        public Guid FromAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = "Payment";
    }
}
