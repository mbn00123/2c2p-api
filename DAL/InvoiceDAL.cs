using InvoiceAPI.Models;
using InvoiceAPI.ModelViews;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InvoiceAPI.DAL
{
    public class InvoiceDAL
    {
        private readonly IConfiguration _config;
        private string ConnectionString;

        public InvoiceDAL(IConfiguration config)
        {
            _config = config;
            ConnectionString = _config["ConnectionString"];
        }

        public IEnumerable<InvoiceViewModel> Search(SearchInvoiceCriteriaModel model)
        {
            List<InvoiceViewModel> results = new List<InvoiceViewModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM Invoice WHERE 1=1";

                    if (model.Currency != null)
                    {
                        sql += " AND CurrencyCode = @pCurrency";
                    }

                    if (model.Status != null)
                    {
                        sql += " AND Status = @pStatus";
                    }

                    if (model.StartDate != null)
                    {
                        model.StartDate = (DateTime)(model.StartDate?.Date); //remove time
                        sql += " AND TransactionDate >= @pStartDate";
                    }

                    if (model.EndDate != null)
                    {
                        model.EndDate = (DateTime)(model.EndDate?.Date); //remove time
                        model.EndDate?.AddDays(1);
                        sql += " AND TransactionDate < @pEndDate";
                    }

                    sql += " ORDER BY TransactionId ASC";
                    sql += " OFFSET @pOffset ROWS";
                    sql += " FETCH NEXT @pPageSize ROWS ONLY";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@pOffset", model.PageSize * model.PageIndex);
                        command.Parameters.AddWithValue("@pPageSize", model.PageSize);
                        command.Parameters.AddWithValue("@pCurrency", (object)model.Currency ?? DBNull.Value);
                        command.Parameters.AddWithValue("@pStatus", (object)model.Status ?? DBNull.Value);
                        command.Parameters.AddWithValue("@pStartDate", (object)model.StartDate ?? DBNull.Value);
                        command.Parameters.AddWithValue("@pEndDate", (object)model.EndDate ?? DBNull.Value);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new InvoiceViewModel
                                {
                                    TransactionId = reader["TransactionId"].ToString(),
                                    Amount = decimal.Parse(reader["Amount"].ToString()),
                                    CurrencyCode = reader["CurrencyCode"].ToString(),
                                    Status = reader["Status"].ToString(),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }


            return results;
        }

        public void BulkInsertIntoInvoiceTemp(List<InvoiceTempModel> invoices)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                // Create a SqlBulkCopy object
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    // Set the destination table name
                    bulkCopy.DestinationTableName = "Invoice_temp";

                    // Map the columns from the source data to the destination table
                    bulkCopy.ColumnMappings.Add("uuid", "uuid");
                    bulkCopy.ColumnMappings.Add("TransactionId", "TransactionId");
                    bulkCopy.ColumnMappings.Add("Amount", "Amount");
                    bulkCopy.ColumnMappings.Add("CurrencyCode", "CurrencyCode");
                    bulkCopy.ColumnMappings.Add("TransactionDate", "TransactionDate");
                    bulkCopy.ColumnMappings.Add("Status", "Status");

                    // Set the batch size (optional)
                    bulkCopy.BatchSize = 1000;

                    // Set the timeout (optional)
                    bulkCopy.BulkCopyTimeout = 600; // in seconds

                    // Write the data to the SQL Server table
                    bulkCopy.WriteToServer(CreateDataTable(invoices));
                }
            }
        }

        private DataTable CreateDataTable(List<InvoiceTempModel> invoices)
        {
            DataTable dataTable = new DataTable();

            // Add columns to the DataTable
            dataTable.Columns.Add("uuid", typeof(string));
            dataTable.Columns.Add("TransactionId", typeof(string));
            dataTable.Columns.Add("Amount", typeof(decimal));
            dataTable.Columns.Add("CurrencyCode", typeof(string));
            dataTable.Columns.Add("TransactionDate", typeof(DateTime));
            dataTable.Columns.Add("Status", typeof(string));

            // Add rows to the DataTable
            foreach (var invoice in invoices)
            {
                dataTable.Rows.Add(invoice.uuid, invoice.TransactionId, invoice.Amount, invoice.CurrencyCode, invoice.TransactionDate, invoice.Status);
            }

            return dataTable;
        }
    }
}
