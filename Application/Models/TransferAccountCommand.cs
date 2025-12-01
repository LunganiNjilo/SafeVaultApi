namespace Application.Models
{
    public class TransferAccountCommand
    {
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
