namespace InvoiceAPI.Helpers
{
    public static class InvoiceStatusMapper
    {
        public static string Get(string status)
        {
            string invoiceStatus = "";
            switch (status.ToLower())
            {
                case "approved":
                    invoiceStatus = "A";
                    break;
                case "failed":
                case "rejected":
                    invoiceStatus = "R";
                    break;
                case "finished":
                case "done":
                    invoiceStatus = "D";
                    break;
            }

            return invoiceStatus;
        }
    }
}
