using System.Web.Http.Results;
using NUnit.Framework;
using  Calculator.WebApi.Controllers;
using Saturn72.UnitTesting.Framework;

namespace Calculator.WebApi.Tests.Controllers
{
    public class CalculatorControllerTests
    {
        [Test]
        public void CalculatorController_AddsTwoNumbers()
        {
            var ctrl = new CalculatorController();
            //2 positives
            (ctrl.Add(null, 2, 3) as OkNegotiatedContentResult<int>).Content.ShouldEqual(5);

            //positive + negative
            (ctrl.Add(null, -1, 2) as OkNegotiatedContentResult<int>).Content.ShouldEqual(1);

            //2 negatives
            (ctrl.Add(null, -2, -3) as OkNegotiatedContentResult<int>).Content.ShouldEqual(-5);
        }
    }
}
