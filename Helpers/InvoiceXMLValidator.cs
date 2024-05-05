using InvoiceAPI.ModelViews;

namespace InvoiceAPI.Helpers
{
    public static class InvoiceXMLValidator
    {
        public static List<string> Validate(InvoiceXmlData.TransactionList transactionList)
        {
            List<string> errors = new List<string>();

            for (int i = 1; i < transactionList.Transactions.Count; i++) // Start from index 1 to skip the header record
            {
                InvoiceXmlData.Transaction record = transactionList.Transactions[i];
                int recordNumber = i + 1; // Record number is index + 1

                // Validate record fields
                List<string> recordErrors = ValidateCsvRecord(record, recordNumber);
                errors.AddRange(recordErrors);
            }

            return errors;
        }

        private static List<string> ValidateCsvRecord(InvoiceXmlData.Transaction record, int recordNumber)
        {
            List<string> errors = new List<string>();

            // Validate Transaction ID length
            if (record.Id == null)
            {
                errors.Add($"Record {recordNumber}: Transaction ID must be specified");
            }
            if (!string.IsNullOrEmpty(record.Id) && record.Id.Length > 50)
            {
                errors.Add($"Record {recordNumber}: Transaction ID length must be less than or equal to 50");
            }

            // Validate Amount type
            if (record.PaymentDetails.Amount == null)
            {
                errors.Add($"Record {recordNumber}: Amount must be specified");
            }
            if (!string.IsNullOrEmpty(record.PaymentDetails.Amount) && !decimal.TryParse(record.PaymentDetails.Amount, out _))
            {
                errors.Add($"Record {recordNumber}: Amount must be a number");
            }

            // Validate currencyCode length
            if (record.PaymentDetails.CurrencyCode == null)
            {
                errors.Add($"Record {recordNumber}: CurrencyCode must be specified");
            }
            if (!string.IsNullOrEmpty(record.PaymentDetails.CurrencyCode) && record.PaymentDetails.CurrencyCode.Length != 3)
            {
                errors.Add($"Record {recordNumber}: CurrencyCode length must be 3 characters");
            }

            // Validate transaction date format
            DateTime? transactionDate;
            if (record.TransactionDate == null)
            {
                errors.Add($"Record {recordNumber}: TransactionDate must be specified");
            }
            if (!string.IsNullOrEmpty(record.TransactionDate) && !DateTime.TryParseExact(record.TransactionDate, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                errors.Add($"Record {recordNumber}: TransactionDate must be in format yyyy-MM-ddTHH:mm:ss");
            }

            // Validate status
            if (record.Status == null)
            {
                errors.Add($"Record {recordNumber}: Status must be specified");
            }
            string[] validStatuses = { "Approved", "Rejected", "Done" };
            if (!string.IsNullOrEmpty(record.Status) && Array.IndexOf(validStatuses, record.Status) == -1)
            {
                errors.Add($"Record {recordNumber}: Status must be one of Approved, Rejected, or Done");
            }

            return errors;
        }
    }
}
