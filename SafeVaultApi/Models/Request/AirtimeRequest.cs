namespace SafeVaultApi.Models.Request
{
    public class AirtimeRequest
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
