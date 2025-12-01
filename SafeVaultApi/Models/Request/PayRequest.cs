namespace SafeVaultApi.Models.Request
{
    public class PayRequest
    {
        public Guid FromAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = "Payment";
    }
}
