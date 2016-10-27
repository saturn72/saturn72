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

        [Test]
        public void CalculatorController_SubtractTwoNumbers()
        {
            var ctrl = new CalculatorController();
            //2 positives
            (ctrl.Subtract(null, 2, 3) as OkNegotiatedContentResult<int>).Content.ShouldEqual(-1);
            (ctrl.Subtract(null, 3, 2) as OkNegotiatedContentResult<int>).Content.ShouldEqual(1);

            //positive + negative
            (ctrl.Subtract(null, -1, 2) as OkNegotiatedContentResult<int>).Content.ShouldEqual(-3);

            //2 negatives
            (ctrl.Subtract(null, -2, -3) as OkNegotiatedContentResult<int>).Content.ShouldEqual(1);
        }

        [Test]
        public void CalculatorController_MultipleTwoNumbers()
        {
            var ctrl = new CalculatorController();
            (ctrl.Multiple(null, 0, 3) as OkNegotiatedContentResult<int>).Content.ShouldEqual(0);
            //2 positives
            (ctrl.Multiple(null, 2, 3) as OkNegotiatedContentResult<int>).Content.ShouldEqual(6);

            //positive + negative
            (ctrl.Multiple(null, -1, 2) as OkNegotiatedContentResult<int>).Content.ShouldEqual(-2);

            //2 negatives
            (ctrl.Multiple(null, -2, -3) as OkNegotiatedContentResult<int>).Content.ShouldEqual(6);
        }

        [Test]
        public void CalculatorController_DivideTwoNumbers()
        {
            var ctrl = new CalculatorController();
            (ctrl.Divide(null, 0, 3) as OkNegotiatedContentResult<long>).Content.ShouldEqual(0);
            (ctrl.Divide(null, 6, 2) as OkNegotiatedContentResult<long>).Content.ShouldEqual(3);
            (ctrl.Divide(null, 2, 3) as OkNegotiatedContentResult<long>).Content.ShouldEqual(2/3);

            
            (ctrl.Divide(null, -1, 2) as OkNegotiatedContentResult<long>).Content.ShouldEqual(-1/2);
            (ctrl.Divide(null, -10, 2) as OkNegotiatedContentResult<long>).Content.ShouldEqual(-5);

            
            (ctrl.Divide(null, -2, -3) as OkNegotiatedContentResult<long>).Content.ShouldEqual(2/3);

            (ctrl.Divide(null, -10, 0) as BadRequestResult).ShouldNotBeNull();

        }
    }
}
