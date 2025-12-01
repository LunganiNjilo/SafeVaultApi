namespace Application.Models
{
    public  class TransferAccountResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;

        public decimal FromAccountBalance { get; set; }

        public decimal ToAccountBalance { get; set; }

        public decimal Amount { get; set; }
    }
}
