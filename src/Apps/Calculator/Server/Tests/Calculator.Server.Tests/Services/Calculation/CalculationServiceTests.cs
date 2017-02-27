#region

using System;
using System.Collections.Generic;
using System.Linq;
using Calculator.Common.Domain.Calculations;
using Calculator.Server.Services.Calculation;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Data;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Calculator.Server.Tests.Services.Calculation
{
    public class CalculationServiceTests
    {
        private static readonly IList<ExpressionModel> ExpressionList = new List<ExpressionModel>();
        private readonly IRepository<ExpressionModel, long> _expressionRepoMock = CreateNockRepository();

        private static IRepository<ExpressionModel, long> CreateNockRepository()
        {
            var mockRepo = new Mock<IRepository<ExpressionModel, long>>();
            mockRepo.Setup(r => r.GetAll())
                .Returns(ExpressionList);
            mockRepo.Setup(r => r.Create(It.IsAny<ExpressionModel>()))
                .Callback<ExpressionModel>(edm =>
                {
                    edm.Id = ExpressionList.Count;
                    ExpressionList.Add(edm);
                });

            return mockRepo.Object;
        }

        [Test]
        public void CalculatorController_GetAllMessages()
        {
            ExpressionList.Clear();
            //less than 10 messages
            var srv = new CalculationService(_expressionRepoMock);

            var maxIndex = 3;
            for (var i = 0; i < maxIndex; i++)
            {
                srv.AddAsync(i, i + 1).Wait();
            }
            var exps = srv.GetExpressionsAsync().Result;
            exps.Count().ShouldEqual(maxIndex);
            for (var i = 0; i < maxIndex; i++)
            {
                var expectedMessage =string.Format("Perform {0} Expression with X: {1} and Y: {2}", "Add", i, i + 1);
                exps.ElementAt(maxIndex-1-i).ShouldEqual(expectedMessage);
            }

            ExpressionList.Clear();

            maxIndex = 10;
            for (var i = 0; i < maxIndex; i++)
            {
                srv.AddAsync(i, i + 1).Wait();
            }
            exps = srv.GetExpressionsAsync().Result;
            exps.Count().ShouldEqual(maxIndex);
            for (var i = 0; i < maxIndex; i++)
            {
                var expectedMessage = string.Format("Perform {0} Expression with X: {1} and Y: {2}", "Add", i, i + 1);
                exps.ElementAt(maxIndex - 1 - i).ShouldEqual(expectedMessage);
            }


            //more than 10 messages 
            ExpressionList.Clear();

            var curIndex = 12;
            for (var i = 0; i < curIndex; i++)
            {
                srv.AddAsync(i, i + 1).Wait();
            }
            exps = srv.GetExpressionsAsync().Result;
            exps.Count().ShouldEqual(maxIndex);
            for (var i = 0; i < maxIndex; i++)
            {
                var expectedMessage =  string.Format("Perform {0} Expression with X: {1} and Y: {2}", "Add", curIndex-1-i, curIndex-  i);
                exps.ElementAt(i).ShouldEqual(expectedMessage);
            }
            ExpressionList.Clear();
        }


        [Test]
        public void CalculatorController_AddsTwoNumbers()
        {
            ExpressionList.Clear();
            var srv = new CalculationService(_expressionRepoMock);
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
            ExpressionList.Clear();
            var srv = new CalculationService(_expressionRepoMock);
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
            ExpressionList.Clear();
            var srv = new CalculationService(_expressionRepoMock);
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
            var srv = new CalculationService(_expressionRepoMock);
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
                () => new CalculationService(_expressionRepoMock).DivideAsync(-10, 0));
        }
    }
}