namespace InvoiceAPI.ModelViews
{
    public class ResponsePaggingModel<T>
    {
        public int PageIndex { get; set; }
        public int TotalRecord { get; set; }
        public int TotalPage { get; set; }
        public IEnumerable<T> data { get; set; }
    }
}
