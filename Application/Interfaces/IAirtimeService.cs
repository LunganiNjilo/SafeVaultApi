namespace Application.Interfaces
{
    public interface IAirtimeService
    {
        Task<bool> PurchaseAirtimeAsync(
            string accountNumber,
            string msisdn,
            decimal amount
        );
    }
}
