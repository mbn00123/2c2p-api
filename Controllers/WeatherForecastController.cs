using InvoiceAPI.DAL;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _config;
        private readonly InvoiceDAL _invoiceDAL;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration config, InvoiceDAL invoiceDAL)
        {
            _logger = logger;
            _config = config;
            _invoiceDAL = invoiceDAL;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public ActionResult Get()
        {
            var userId = _invoiceDAL.Search(new ModelViews.SearchInvoiceCriteriaModel());
            return Ok();
        }
    }
}
