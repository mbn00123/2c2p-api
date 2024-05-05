using CsvHelper;
using InvoiceAPI.DAL;
using InvoiceAPI.Helpers;
using InvoiceAPI.Models;
using InvoiceAPI.ModelViews;
using Microsoft.AspNetCore.Mvc;
using System;

namespace InvoiceAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InvoiceController : Controller
    {
        private readonly InvoiceDAL _invoiceDAL;

        public InvoiceController(InvoiceDAL invoiceDAL)
        {
            _invoiceDAL = invoiceDAL;
        }

        [HttpPost(Name = "SearchByStatus")]
        public ActionResult<IEnumerable<InvoiceModel>> SearchByStatus([FromBody] SearchInvoiceCriteriaModel request)
        {
            try
            {
                var results = _invoiceDAL.Search(new SearchInvoiceCriteriaModel()
                {
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Status = request.Status,
                });
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost(Name = "SearchByCurrency")]
        public ActionResult<IEnumerable<InvoiceModel>> SearchByCurrency([FromBody] SearchInvoiceCriteriaModel request)
        {
            try
            {
                var results = _invoiceDAL.Search(new SearchInvoiceCriteriaModel()
                {
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Currency = request.Currency,
                });
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost(Name = "SearchByTransactionDate")]
        public ActionResult<IEnumerable<InvoiceModel>> SearchByTransactionDate([FromBody] SearchInvoiceCriteriaModel request)
        {
            try
            {
                var results = _invoiceDAL.Search(new SearchInvoiceCriteriaModel()
                {
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                });
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost(Name = "upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Check file format
            var allowedFormats = new[] { ".csv", ".xml" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedFormats.Contains(fileExtension))
            {
                return BadRequest("Unknown Format");
            }

            // Check maximum file size (about 1MB)
            const int maxFileSizeInBytes = 1 * 1024 * 1024; // 1MB
            if (file.Length > maxFileSizeInBytes)
            {
                return BadRequest("File size exceeds the maximum limit of 1MB.");
            }

            // Process the file
            try
            {
                // Read the file content
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var fileContent = await reader.ReadToEndAsync();
                    string uuid = Guid.NewGuid().ToString();
                    List<InvoiceTempModel> uploadInvoices = new List<InvoiceTempModel>();

                    if (fileExtension == ".csv")
                    {
                        var csvMapper = new InvoiceCsvMapper();
                        var csvRecords = csvMapper.MapCsvContentToClass(fileContent);
                        //return Ok(csvRecords);
                        // *** validate ***

                        uploadInvoices = csvRecords.Select(x => new InvoiceTempModel()
                        {
                            uuid = uuid,
                            TransactionId = x.TransactionIdentificator,
                            Amount = (decimal)x.Amount,
                            CurrencyCode = x.CurrencyCode,
                            TransactionDate = (DateTime)x.TransactionDate,
                            Status = InvoiceStatusMapper.Get(x.Status)
                        }).ToList();
                    }
                    else
                    {
                        var xmlMapper = new InvoiceXMLMapper();
                        var xmlRecords = xmlMapper.MapXmlContentToClass(fileContent);
                        //return Ok(transactionList);
                        // *** validate ***

                        uploadInvoices = xmlRecords.Transactions.Select(x => new InvoiceTempModel()
                        {
                            uuid = uuid,
                            TransactionId = x.Id,
                            Amount = (decimal)x.PaymentDetails.Amount,
                            CurrencyCode = x.PaymentDetails.CurrencyCode,
                            TransactionDate = (DateTime)x.TransactionDate,
                            Status = InvoiceStatusMapper.Get(x.Status)
                        }).ToList();
                    }

                    _invoiceDAL.BulkInsertIntoInvoiceTemp(uploadInvoices);
                    _invoiceDAL.InsertInvoiceFromTempTalbe(uuid);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
