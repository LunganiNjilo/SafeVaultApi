namespace SafeVaultApi.Models.Response
{
    public class TransactionResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public decimal BalanceAfter { get; set; }
        public string Description { get; set; } = "";
        public string Type { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
