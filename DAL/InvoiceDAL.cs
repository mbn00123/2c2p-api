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
    }
}
