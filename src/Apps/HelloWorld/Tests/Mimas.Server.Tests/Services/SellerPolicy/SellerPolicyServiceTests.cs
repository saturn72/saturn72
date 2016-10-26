using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mimas.Common.Data.Repositories;
using Mimas.Common.Domain.SellerPolicy;
using Mimas.Common.Domain.Users;
using Mimas.Server.Services.SellerPolicy;
using Moq;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

namespace Mimas.Server.Tests.Services.SellerPolicy
{
    public class SellerPolicyServiceTests
    {
        //[Test]
        //public void SellerPolicyService_GetSellerShippingPolicy_ThrowsOnIllegalSellerId()
        //{
        //    var srv = new SellerPolicyService(null, null);

        //    typeof(ArgumentException).ShouldBeThrownBy(()=> srv.GetSellerShippingPoliciesAsync(0));
        //    typeof(ArgumentException).ShouldBeThrownBy(()=> srv.GetSellerShippingPoliciesAsync(-10));
        //}

        [Test]
        public void SellerPolicyService_GetSellerShippingPolicy_ThrowsOnNonExistSellerId()
        {
            var sellerRepository = new Mock<ISellerRepository>();
            sellerRepository.Setup(r => r.GetSellerByIdAsync(It.IsAny<long>()))
                .Returns(()=> null);
            var srv = new SellerPolicyService(sellerRepository.Object, null);
            var task = new Task(()=>srv.GetSellerShippingPoliciesAsync(long.MaxValue));
            task.Start();
            task.Wait();
            task.Exception.InnerException.GetType().ShouldBeType<ArgumentException>();
        }

        [Test]
        public void SellerPolicyService_GetSellerShippingPolicy_ReturnsNullCollection()
        {
            var seller = new SellerDomainModel {Id = 2000};
            var sellerRepos = new Mock<ISellerRepository>();
            sellerRepos.Setup(r => r.GetSellerByIdAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(seller));

            var sellerPolicyRepo = new Mock<ISellerPolicyRepository>();
            sellerPolicyRepo.Setup(r => r.GetSellerShippingPoliciesAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(new List<SellerShippingPolicy>() as IEnumerable<SellerShippingPolicy>));

            var srv = new SellerPolicyService(sellerRepos.Object, sellerPolicyRepo.Object);
            var actual = srv.GetSellerShippingPoliciesAsync(long.MaxValue).Result;
            actual.Count().ShouldEqual(0);
        }
    }
}