namespace SafeVaultApi.Models.Response
{
    public class AccountDashboardResponse
    {
        public AccountResponse Account { get; set; } = new();
        public List<TransactionResponse> Transactions { get; set; } = new();
    }
}
