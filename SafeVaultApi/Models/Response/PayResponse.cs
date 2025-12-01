namespace SafeVaultApi.Models.Response
{
    public class PayResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public decimal? NewBalance { get; set; }
        public string? TransactionId { get; set; }
    }
}
