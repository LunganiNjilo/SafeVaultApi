namespace SafeVaultApi.Models.Request
{
    public class TransferRequest
    {
        public required string FromAccountId { get; set; }
        public required string ToAccountId { get; set; }
        public  decimal Amount { get; set; }
    }
}
