namespace InvoiceAPI.ModelViews
{
    public class SearchInvoiceCriteriaModel
    {
        public int PageIndex {get;set;}
        public int PageSize {get;set;}
        public string Currency {get;set;}
        public string Status {get;set;}
        public DateTime StartDate {get;set;}
        public DateTime EndDate { get; set; }
    }
}
