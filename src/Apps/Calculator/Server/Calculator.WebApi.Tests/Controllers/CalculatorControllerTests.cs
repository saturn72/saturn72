#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Calculator.Server.Services.Calculation;
using Calculator.WebApi.Controllers;
using Moq;
using NUnit.Framework;
using Shouldly;

#endregion

namespace Calculator.WebApi.Tests.Controllers
{
    public class CalculatorControllerTests
    {
        private readonly ICalculationService _calcSrv = CreateCAlculationServiceMock();
        private static readonly IEnumerable<string> expectedExpressions = new []{"1", "2", "3", "4", "5"};

        private static ICalculationService CreateCAlculationServiceMock()
        {
            var res = new Mock<ICalculationService>();
            res.Setup(s => s.GetExpressionsAsync())
                .Returns(() => Task.FromResult(expectedExpressions));

            res.Setup(s => s.AddAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, int>((x, y) => Task.FromResult(x + y));

            res.Setup(s => s.SubtractAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, int>((x, y) => Task.FromResult(x - y));

            res.Setup(s => s.MultipleAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, int>((x, y) => Task.FromResult(x*y));

            res.Setup(s => s.DivideAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns<long, long>((x, y) => Task.Run(() =>
                {
                    if (y == 0)
                        throw new ArgumentException();
                    return x/y;
                }));

            return res.Object;
        }

        [Test]
        public void CalculatorController_GetsExpressions()
        {
            var ctrl = new CalculatorController(_calcSrv);
            (ctrl.Get(null).Result as OkNegotiatedContentResult<IEnumerable<string>>).Content.ShouldBe(expectedExpressions);
        }

        [Test]
        public void CalculatorController_AddsTwoNumbers()
        {
            var ctrl = new CalculatorController(_calcSrv);
            //2 positives
            (ctrl.Add(null, 2, 3).Result as OkNegotiatedContentResult<int>).Content.ShouldBe(5);

            //positive + negative
            (ctrl.Add(null, -1, 2).Result as OkNegotiatedContentResult<int>).Content.ShouldBe(1);

            //2 negatives
            (ctrl.Add(null, -2, -3).Result as OkNegotiatedContentResult<int>).Content.ShouldBe(-5);
        }

        [Test]
        public void CalculatorController_SubtractTwoNumbers()
        {
            var ctrl = new CalculatorController(_calcSrv);
            //2 positives
            (ctrl.Subtract(null, 2, 3).Result as OkNegotiatedContentResult<int>).Content.ShouldBe(-1);
            (ctrl.Subtract(null, 3, 2).Result as OkNegotiatedContentResult<int>).Content.ShouldBe(1);

            //positive + negative
            (ctrl.Subtract(null, -1, 2).Result as OkNegotiatedContentResult<int>).Content.ShouldBe(-3);

            //2 negatives
            (ctrl.Subtract(null, -2, -3).Result as OkNegotiatedContentResult<int>).Content.ShouldBe(1);
        }

        [Test]
        public void CalculatorController_MultipleTwoNumbers()
        {
            var ctrl = new CalculatorController(_calcSrv);
            (ctrl.Multiple(null, 0, 3).Result as OkNegotiatedContentResult<int>).Content.ShouldBe(0);
            //2 positives
            (ctrl.Multiple(null, 2, 3).Result as OkNegotiatedContentResult<int>).Content.ShouldBe(6);

            //positive + negative
            (ctrl.Multiple(null, -1, 2).Result as OkNegotiatedContentResult<int>).Content.ShouldBe(-2);

            //2 negatives
            (ctrl.Multiple(null, -2, -3).Result as OkNegotiatedContentResult<int>).Content.ShouldBe(6);
        }

        [Test]
        public void CalculatorController_DivideTwoNumbers()
        {
            var ctrl = new CalculatorController(_calcSrv);
            (ctrl.Divide(null, 0, 3).Result as OkNegotiatedContentResult<long>).Content.ShouldBe(0);
            (ctrl.Divide(null, 6, 2).Result as OkNegotiatedContentResult<long>).Content.ShouldBe(3);
            (ctrl.Divide(null, 2, 3).Result as OkNegotiatedContentResult<long>).Content.ShouldBe(2/3);


            (ctrl.Divide(null, -1, 2).Result as OkNegotiatedContentResult<long>).Content.ShouldBe(-1/2);
            (ctrl.Divide(null, -10, 2).Result as OkNegotiatedContentResult<long>).Content.ShouldBe(-5);


            (ctrl.Divide(null, -2, -3).Result as OkNegotiatedContentResult<long>).Content.ShouldBe(2/3);

            (ctrl.Divide(null, -10, 0).Result as BadRequestResult).ShouldNotBeNull();
        }
    }
}