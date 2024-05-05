namespace InvoiceAPI.ModelViews
{
    public class InvoiceViewModel
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Status { get; set; }
    }
}
