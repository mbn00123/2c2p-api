﻿using InvoiceAPI.Models;
using InvoiceAPI.ModelViews;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
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

        public int Count(SearchInvoiceCriteriaModel model)
        {
            int totalRecord = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string sql = "SELECT COUNT(TransactionId) AS TotalRecord FROM Invoice WHERE 1=1";

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
                        sql += " AND TransactionDate >= @pStartDate";
                    }

                    if (model.EndDate != null)
                    {
                        model.EndDate = model.EndDate?.AddDays(1);
                        sql += " AND TransactionDate < @pEndDate";
                    }


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@pCurrency", (object)model.Currency ?? DBNull.Value);
                        command.Parameters.AddWithValue("@pStatus", (object)model.Status ?? DBNull.Value);
                        command.Parameters.AddWithValue("@pStartDate", (object)model.StartDate?.Date ?? DBNull.Value);
                        command.Parameters.AddWithValue("@pEndDate", (object)model.EndDate?.Date ?? DBNull.Value);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                totalRecord = int.Parse(reader["TotalRecord"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }


            return totalRecord;
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
                        sql += " AND TransactionDate >= @pStartDate";
                    }

                    if (model.EndDate != null)
                    {
                        model.EndDate = model.EndDate?.AddDays(1);
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
                        command.Parameters.AddWithValue("@pStartDate", (object)model.StartDate?.Date ?? DBNull.Value);
                        command.Parameters.AddWithValue("@pEndDate", (object)model.EndDate?.Date ?? DBNull.Value);

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

        public void InsertInvoiceFromTempTalbe(string uuid)
        {
            try
            {
                //WHEN MATCHED ==> Update
                //WHEN NOT MATCHED ==> Insert
                string sql = @"
                MERGE INTO invoice AS target
                USING ( SELECT * FROM invoice_temp WHERE uuid = @pUUID ) AS source
                ON target.TransactionId = source.TransactionId
                
                WHEN MATCHED THEN
                    UPDATE SET
                        target.Amount = source.Amount,
                        target.CurrencyCode = source.CurrencyCode,
                        target.TransactionDate = source.TransactionDate,
                        target.Status = source.Status
                
                WHEN NOT MATCHED BY TARGET THEN
                    INSERT (TransactionId, Amount, CurrencyCode, TransactionDate, Status)
                    VALUES (source.TransactionId, source.Amount, source.CurrencyCode, source.TransactionDate, source.Status);
                ";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@pUUID", uuid);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                throw;
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
