using InvoiceAPI.ModelViews;

namespace InvoiceAPI.Helpers
{
    public static class InvoiceCsvValidator
    {
        public static List<string> Validate(List<InvoiceCsvRecord> records)
        {
            List<string> errors = new List<string>();

            for (int i = 1; i < records.Count; i++) // Start from index 1 to skip the header record
            {
                InvoiceCsvRecord record = records[i];
                int recordNumber = i + 1; // Record number is index + 1

                // Validate record fields
                List<string> recordErrors = ValidateCsvRecord(record, recordNumber);
                errors.AddRange(recordErrors);
            }

            return errors;
        }

        private static List<string> ValidateCsvRecord(InvoiceCsvRecord record, int recordNumber)
        {
            List<string> errors = new List<string>();

            // Validate TransactionIdentificator length
            if (record.TransactionIdentificator == null)
            {
                errors.Add($"Record {recordNumber}: TransactionIdentificator must be specified");
            }
            if (!string.IsNullOrEmpty(record.TransactionIdentificator) && record.TransactionIdentificator.Length > 50)
            {
                errors.Add($"Record {recordNumber}: TransactionIdentificator length must be less than or equal to 50");
            }

            // Validate Amount type
            if(record.Amount == null)
            {
                errors.Add($"Record {recordNumber}: Amount must be specified");
            }
            if (!string.IsNullOrEmpty(record.Amount) && !decimal.TryParse(record.Amount, out _))
            {
                errors.Add($"Record {recordNumber}: Amount must be a number");
            }

            // Validate currencyCode length
            if (record.CurrencyCode == null)
            {
                errors.Add($"Record {recordNumber}: CurrencyCode must be specified");
            }
            if (!string.IsNullOrEmpty(record.CurrencyCode) && record.CurrencyCode.Length != 3)
            {
                errors.Add($"Record {recordNumber}: CurrencyCode length must be 3 characters");
            }

            // Validate transaction date format
            DateTime? transactionDate;
            if (record.TransactionDate == null)
            {
                errors.Add($"Record {recordNumber}: TransactionDate must be specified");
            }
            if (!string.IsNullOrEmpty(record.TransactionDate) && !DateTime.TryParseExact(record.TransactionDate, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                errors.Add($"Record {recordNumber}: TransactionDate must be in format dd/MM/yyyy HH:mm:ss");
            }

            // Validate status
            if (record.Status == null)
            {
                errors.Add($"Record {recordNumber}: Status must be specified");
            }
            string[] validStatuses = { "Approved", "Failed", "Finished" };
            if (!string.IsNullOrEmpty(record.Status) && Array.IndexOf(validStatuses, record.Status) == -1)
            {
                errors.Add($"Record {recordNumber}: Status must be one of Approved, Failed, or Finished");
            }

            return errors;
        }
    }
}
