using System.Linq;
using NUnit.Framework;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Tests.Infrastructure.AppDomainManagements.TestObjects;
using Shouldly;

namespace Saturn72.Core.Tests.Infrastructure.AppDomainManagements
{
    public class AppDomainTypeFinderTests
    {
        [Test]
        public void AppDomainTypeFinder_FindClassesOfType()
        {
            var tf = new AppDomainTypeFinder();

            var mis = tf.FindClassesOfType<ITestObject2>();
            mis.Count().ShouldBe(2);

            mis = tf.FindClassesOfType<TestObject1>();
            mis.Count().ShouldBe(1);

        }

        [Test]
        public void AppDomainTypeFinder_FindMethodsOfReturnType()
        {
            var tf = new AppDomainTypeFinder();
            var mis = tf.FindMethodsOfReturnType<ReturnType1>();
            mis.Count().ShouldBe(2);
            mis.Select(m => m.MemberType).Distinct().Count().ShouldBe(1);
        }
    }
}