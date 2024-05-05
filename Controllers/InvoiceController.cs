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
    }
}
