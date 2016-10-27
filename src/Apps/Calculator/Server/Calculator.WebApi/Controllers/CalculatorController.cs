#region

using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Saturn72.Common.WebApi;

#endregion

namespace Calculator.WebApi.Controllers
{
    [RoutePrefix("api/calc")]
    public class CalculatorController : Saturn72ApiControllerBase
    {
        [Route("add/{x}/{y}")]
        [HttpGet]
        public IHttpActionResult Add(HttpRequestMessage request, int x, int y)
        {
            return Ok(x + y);
        }

      [Route("sub/{x}/{y}")]
        [HttpGet]
        public IHttpActionResult Subtract(HttpRequestMessage request, int x, int y)
        {
            return Ok(x - y);
        }

        [Route("mul/{x}/{y}")]
        [HttpGet]
        public IHttpActionResult Multiple(HttpRequestMessage request, int x, int y)
        {
            return Ok(x * y);
        }

        [Route("div/{x}/{y}")]
        [HttpGet]
        public IHttpActionResult Divide(HttpRequestMessage request, long x, long y)
        {
            if (y == 0)
                return BadRequest();

            return Ok(x / y);
        }
    }
}