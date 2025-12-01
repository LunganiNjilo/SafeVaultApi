namespace SafeVaultApi.Models.Response
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "ZAR";
    }
}
