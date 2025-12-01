namespace Application.Models
{
    public class PayCommand
    {
        public Guid FromAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = "Payment";
    }
}
