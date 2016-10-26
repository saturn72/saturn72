#region

using NUnit.Framework;
using Saturn72.UnitTesting.Framework;
using Saturn72.Core.Infrastructure.AppDomainManagement;

#endregion

namespace Saturn72.Core.Tests.Infrastructure
{
    public class TypeFinderExtensionsTests
    {
        [Test]
        public void FindClassesOfTypeAndRunAction_RunsMethod()
        {
            var i = 0;
            var typeFinder = new AppDomainTypeFinder();
            typeFinder.FindClassesOfTypeAndRunMethod<IMyInterface>(t => t.Do(ref i));

            1.ShouldEqual(i);
        }


        internal interface IMyInterface

        {
            int Do(ref int i);
        }

        internal class MyImpl1 : IMyInterface
        {
            public int Do(ref int i)
            {
                return i++;
            }
        }
    }
}