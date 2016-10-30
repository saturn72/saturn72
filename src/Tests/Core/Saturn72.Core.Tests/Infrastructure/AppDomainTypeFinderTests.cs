#region

using System;
using System.Linq;
using NUnit.Framework;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Tests.Infrastructure
{
    public class AppDomainTypeFinderTests
    {
        [Test]
        public void AppDomainTypeFinder_FindClassesOfType_GetsAllTypes()
        {
            var typeFinder = new AppDomainTypeFinder();
            typeFinder.FindClassesOfType<IMyInterface>(true).Count().ShouldEqual(2);
            typeFinder.FindClassesOfType<IMyInterface>(false).Count().ShouldEqual(3);

            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            typeFinder.FindClassesOfType(typeof(IMyInterface), allAssemblies, true).Count().ShouldEqual(2);
            typeFinder.FindClassesOfType(typeof(IMyInterface), allAssemblies, false).Count().ShouldEqual(3);
        }

        [Test]
        public void AppDomainTypeFinder_FindClassesOfAttribute_GetsAllTypes()
        {
            var typeFinder = new AppDomainTypeFinder();
            typeFinder.FindClassesOfAttribute<MyAttribute>(true).Count().ShouldEqual(1);
            typeFinder.FindClassesOfType<IMyInterface>(false).Count().ShouldEqual(3);

            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            typeFinder.FindClassesOfAttribute<MyAttribute>(allAssemblies, true).Count().ShouldEqual(1);
            typeFinder.FindClassesOfAttribute<MyAttribute>(allAssemblies, false).Count().ShouldEqual(2);
        }

        [Test]
        public void AppDomainTypeFinder_FindMethodsOfReturnType_GetsAllTypes()
        {
            var typeFinder = new AppDomainTypeFinder();
            typeFinder.FindMethodsOfReturnType<IMyInterface>(true).Count().ShouldEqual(2);
            typeFinder.FindMethodsOfReturnType<IMyInterface>(false).Count().ShouldEqual(3);

            
            typeFinder.FindMethodsOfReturnType(typeof(IMyInterface), true).Count().ShouldEqual(2);
            typeFinder.FindMethodsOfReturnType(typeof(IMyInterface), false).Count().ShouldEqual(3);

            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            typeFinder.FindMethodsOfReturnType(typeof(IMyInterface),allAssemblies,  true).Count().ShouldEqual(2);
            typeFinder.FindMethodsOfReturnType(typeof(IMyInterface), allAssemblies, false).Count().ShouldEqual(3);
        }

        [Test]
        public void AppDomainTypeFinder_GetAllTypes()
        {
            var typeFinder = new AppDomainTypeFinder();
            typeFinder.FindClassesOfType<IMyInterface>().Count().ShouldEqual(2);
        }

        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
        internal sealed class MyAttribute : Attribute
        {
        }

        [My]
        internal interface IMyInterface

        {
            IMyInterface Do(ref int i);
        }

        [My]
        internal class MyImpl1 : IMyInterface
        {
            public IMyInterface Do(ref int i)
            {
                throw new NotImplementedException();
            }
        }

        internal class MyImpl2 : IMyInterface
        {
            public IMyInterface Do(ref int i)
            {
                throw new NotImplementedException();
            }
        }
    }
}