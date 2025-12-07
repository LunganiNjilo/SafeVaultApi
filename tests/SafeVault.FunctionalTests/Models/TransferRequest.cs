namespace SafeVault.FunctionalTests.Models
{
    public class TransferRequest
    {
        public required string FromAccountId { get; set; }
        public required string ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
