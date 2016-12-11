using System;

namespace Saturn72.Module.Ioc.Autofac.Tests.TestObjects
{
    public class TestService : ITestService1, ITestService2
    {
        public virtual void Do(TestObject to)
        {
            throw new NotImplementedException();
        }
    }

    public class TestService2 : ITestService2
    {
        
    }

    public class TestService3 : ITestService3
    {

    }

    public interface ITestService1
    {
        void Do(TestObject to);
    }

    public interface ITestService2
    {
    }

    public interface ITestService3
    {
    }
}