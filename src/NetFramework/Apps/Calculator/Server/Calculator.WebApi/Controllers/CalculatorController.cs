#region

using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Calculator.Server.Services.Calculation;
using Saturn72.Common.WebApi;

#endregion

namespace Calculator.WebApi.Controllers
{
    [RoutePrefix("api/calc")]
    public class CalculatorController : Saturn72ApiControllerBase
    {
        private readonly ICalculationService _calculationService;

        public CalculatorController(ICalculationService calculationService)
        {
            _calculationService = calculationService;
        }

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(HttpRequestMessage request)
        {
            var result = await _calculationService.GetExpressionsAsync();
            return Ok(result);
        }

        [Route("add/{x}/{y}")]
        [HttpGet]
        public async Task<IHttpActionResult> Add(HttpRequestMessage request, int x, int y)
        {
            var result = await _calculationService.AddAsync(x, y);
            return Ok(result);
        }

        [Route("sub/{x}/{y}")]
        [HttpGet]
        public async Task<IHttpActionResult> Subtract(HttpRequestMessage request, int x, int y)
        {
            var result = await _calculationService.SubtractAsync(x, y);
            return Ok(result);
        }

        [Route("mul/{x}/{y}")]
        [HttpGet]
        public async Task<IHttpActionResult> Multiple(HttpRequestMessage request, int x, int y)
        {
            var result = await _calculationService.MultipleAsync(x, y);
            return Ok(result);
        }

        [Route("div/{x}/{y}")]
        [HttpGet]
        public async Task<IHttpActionResult> Divide(HttpRequestMessage request, long x, long y)
        {
            if (y == 0)
                return BadRequest();

            var result = await _calculationService.DivideAsync(x, y);
            return Ok(result);
        }
    }
}