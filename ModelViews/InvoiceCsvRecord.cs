using CsvHelper.Configuration;
using CsvHelper;
using System.Formats.Asn1;
using System.Globalization;

namespace InvoiceAPI.ModelViews
{
    public class InvoiceCsvRecord
    {
        public string? TransactionIdentificator { get; set; }
        public decimal? Amount { get; set; }
        public string? CurrencyCode { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? Status { get; set; }
    }

    public class InvoiceCsvRecordMap : ClassMap<InvoiceCsvRecord>
    {
        public InvoiceCsvRecordMap()
        {
            // Define mappings between CSV columns and model properties
            Map(m => m.TransactionIdentificator).Name("Transaction Identificator");
            Map(m => m.Amount).Name("Amount");
            Map(m => m.CurrencyCode).Name("Currency Code");
            Map(m => m.TransactionDate).Name("Transaction Date");
            Map(m => m.Status).Name("Status");
        }
    }
        
    public class InvoiceCsvValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }
    }


    //public class InvoiceCsvValidator
    //{
    //    public InvoiceCsvValidationResult ValidateCsv(string filePath)
    //    {
    //        var validationResult = new InvoiceCsvValidationResult();
    //        validationResult.Errors = new List<string>();

    //        //using (var reader = new StreamReader(filePath))
    //        //using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
    //        //{
    //        //    // Configure CSV reader settings
    //        //}))
    //        {
    //            csv.Context.RegisterClassMap<CsvRecordMap>();

    //            while (csv.Read())
    //            {
    //                try
    //                {
    //                    var record = csv.GetRecord<CsvRecord>();
    //                    // Perform validation on 'record'
    //                    if (string.IsNullOrEmpty(record.TransactionIdentificator) || record.TransactionIdentificator!.Length > 50)
    //                    {
    //                        validationResult.IsValid = false;
    //                        validationResult.Errors.Add("Transaction Identificator is invalid.");
    //                    }
    //                    if (!IsValidAmount(record.Amount))
    //                    {
    //                        validationResult.IsValid = false;
    //                        validationResult.Errors.Add("Amount is invalid.");
    //                    }
    //                    if (string.IsNullOrEmpty(record.CurrencyCode) || record.CurrencyCode!.Length != 3)
    //                    {
    //                        validationResult.IsValid = false;
    //                        validationResult.Errors.Add("Currency Code is invalid.");
    //                    }
    //                    if (!IsValidDateTimeFormat(record.TransactionDate, "dd/MM/yyyy hh:mm:ss"))
    //                    {
    //                        validationResult.IsValid = false;
    //                        validationResult.Errors.Add("Transaction Date is invalid.");
    //                    }
    //                    if (!IsValidStatus(record.Status))
    //                    {
    //                        validationResult.IsValid = false;
    //                        validationResult.Errors.Add("Status is invalid.");
    //                    }
    //                }
    //                catch (HeaderValidationException ex)
    //                {
    //                    validationResult.IsValid = false;
    //                    validationResult.Errors.Add($"CSV file header is invalid: {ex.Message}");
    //                }
    //                catch (TypeConverterException ex)
    //                {
    //                    validationResult.IsValid = false;
    //                    validationResult.Errors.Add($"CSV data conversion error: {ex.Message}");
    //                }
    //                catch (Exception ex)
    //                {
    //                    validationResult.IsValid = false;
    //                    validationResult.Errors.Add($"An error occurred: {ex.Message}");
    //                }
    //            }
    //        }

    //        if (validationResult.Errors.Count == 0)
    //        {
    //            validationResult.IsValid = true;
    //        }

    //        return validationResult;
    //    }

    //    private bool IsValidDateTimeFormat(DateTime? dateTime, string format)
    //    {
    //        try
    //        {
    //            if (dateTime != null)
    //            {
    //                DateTime parsedDateTime;
    //                return DateTime.TryParseExact(dateTime.Value.ToString(format), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);
    //            }
    //            return true; // Return true for null DateTime
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }

    //    private bool IsValidAmount(decimal? amount)
    //    {
    //        // Validate amount only if it's not null
    //        if (amount != null)
    //        {
    //            // Validate additional rules for the amount if needed
    //            return amount > 0; // Example: Amount should be greater than zero
    //        }
    //        return true; // Return true for null amount
    //    }

    //    private bool IsValidStatus(string? status)
    //    {
    //        if (status != null)
    //        {
    //            return new List<string> { "Approved", "Failed", "Finished" }.Contains(status);
    //        }
    //        return true; // Return true for null status
    //    }
    //}

}
