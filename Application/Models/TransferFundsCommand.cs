namespace Application.Models
{
    public class TransferFundsCommand
    {
        public required string FromAccountId { get; set; }
        public required string ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
