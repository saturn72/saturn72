#region

using System;
using System.Linq;
using Calculator.Server.Services.Calculation;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Calculator.Server.Tests.Services.Calculation
{
    public class CalculationServiceTests
    {
        [Test]
        public void CalculatorController_GetAllMessages()
        {
            //less than 10 messages
            var srv = new CalculationService();

            for (var i = 0; i < 3; i++)
            {
                srv.AddAsync(i, i + 1).Wait();
            }
            var exps = srv.GetExpressionsAsync().Result;
            for (var i = 0; i < 10; i++)
            {
                var expectedMessage = i < 3
                    ? string.Format("Perform {0} Expression with X: {1} and Y: {2}", "Add", i, i + 1)
                    : null;
                exps.ElementAt(i).ShouldEqual(expectedMessage);
            }
            CalculationService.CreateOrResetMessageCollection();
            srv = new CalculationService();

            //exactly 10 messages 

            for (var i = 0; i < 10; i++)
            {
                srv.AddAsync(i, i + 1).Wait();
            }
            exps = srv.GetExpressionsAsync().Result;
            for (var i = 0; i < 10; i++)
            {
                var expectedMessage = string.Format("Perform {0} Expression with X: {1} and Y: {2}", "Add", i, i + 1);
                exps.ElementAt(i).ShouldEqual(expectedMessage);
            }

            //more than 10 messages 
            CalculationService.CreateOrResetMessageCollection();
            srv = new CalculationService();
            for (var i = 0; i < 12; i++)
            {
                srv.AddAsync(i, i + 1).Wait();
            }
            exps = srv.GetExpressionsAsync().Result;
            for (var i = 0; i < 10; i++)
            {
                var expectedMessage = i < 2
                    ? string.Format("Perform {0} Expression with X: {1} and Y: {2}", "Add", 10 + i, 11 + i)
                    : string.Format("Perform {0} Expression with X: {1} and Y: {2}", "Add", i, i + 1);

                exps.ElementAt(i).ShouldEqual(expectedMessage);
            }
        }


        [Test]
        public void CalculatorController_AddsTwoNumbers()
        {
            var srv = new CalculationService();
            //2 positives
            srv.AddAsync(2, 3).Result.ShouldEqual(5);

            //positive + negative
            srv.AddAsync(-1, 2).Result.ShouldEqual(1);

            //2 negatives
            srv.AddAsync(-2, -3).Result.ShouldEqual(-5);
        }

        [Test]
        public void CalculatorController_SubtractTwoNumbers()
        {
            var srv = new CalculationService();
            //2 positives
            srv.SubtractAsync(2, 3).Result.ShouldEqual(-1);
            srv.SubtractAsync(3, 2).Result.ShouldEqual(1);

            //positive + negative
            srv.SubtractAsync(-1, 2).Result.ShouldEqual(-3);

            //2 negatives
            srv.SubtractAsync(-2, -3).Result.ShouldEqual(1);
        }

        [Test]
        public void CalculatorController_MultileAsyncTwoNumbers()
        {
            var srv = new CalculationService();
            srv.MultipleAsync(0, 3).Result.ShouldEqual(0);
            //2 positives
            srv.MultipleAsync(2, 3).Result.ShouldEqual(6);

            //positive + negative
            srv.MultipleAsync(-1, 2).Result.ShouldEqual(-2);

            //2 negatives
            srv.MultipleAsync(-2, -3).Result.ShouldEqual(6);
        }

        [Test]
        public void CalculatorController_DivideTwoNumbers()
        {
            var srv = new CalculationService();
            srv.DivideAsync(0, 3).Result.ShouldEqual(0);
            srv.DivideAsync(6, 2).Result.ShouldEqual(3);
            srv.DivideAsync(2, 3).Result.ShouldEqual(2/3);


            srv.DivideAsync(-1, 2).Result.ShouldEqual(-1/2);
            srv.DivideAsync(-10, 2).Result.ShouldEqual(-5);


            srv.DivideAsync(-2, -3).Result.ShouldEqual(2/3);
        }

        [Test]
        public void Divide_Throws_OnZeroAsY()
        {
            typeof(ArgumentException).ShouldBeThrownBy(
                () => new CalculationService().DivideAsync(-10, 0));
        }
    }
}