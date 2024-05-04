using InvoiceAPI.Models;
using InvoiceAPI.ModelViews;
using System.Data.SqlClient;
using System.Transactions;

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

        public IEnumerable<InvoiceModel> Search(SearchInvoiceCriteriaModel model)
        {
            List< InvoiceModel > results = new List<InvoiceModel> ();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();

                    String sql = "SELECT * FROM Invoice";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //results.Add(new InvoiceModel
                                //{
                                //    TransactionId
                                //    Amount
                                //    CurrencyCode
                                //    TransactionDate
                                //    Status
                                //});
                                var id = reader["TransactionId"].ToString();
                            }
                        }
                    }
                }
                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
