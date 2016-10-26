#region

using System.Net.Http;
using System.Web.Http;
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
    }
}