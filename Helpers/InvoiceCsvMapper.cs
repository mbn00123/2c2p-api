using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using InvoiceAPI.ModelViews;

namespace InvoiceAPI.Helpers
{
    public class InvoiceCsvMapper
    {
        public List<InvoiceCsvRecord> MapCsvContentToClass(string csvContent)
        {
            using (var reader = new StringReader(csvContent))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Configure CSV reader settings
                HasHeaderRecord = true, // Assumes the first row contains column headers
            }))
            {
                csv.Context.RegisterClassMap<InvoiceCsvRecordMap>(); // Optional: Register a class map for custom mappings
                return csv.GetRecords<InvoiceCsvRecord>().ToList();
            }
        }
    }
}
