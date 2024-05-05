using InvoiceAPI.DAL;
using InvoiceAPI.Models;
using InvoiceAPI.ModelViews;
using Microsoft.AspNetCore.Mvc;

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

                    // Do whatever you need with the file content
                    // For example, you can save it to a file or process it further
                    // Here, we simply return the content as a response
                    return Ok(fileContent);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
