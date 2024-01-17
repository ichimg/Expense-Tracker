namespace ExpenseTracker.Domain.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public decimal EurExchangeRate { get; set; }
        public decimal UsdExchangeRate { get; set; }
        public DateTime RequestCallDate { get; set; }
    }
}
