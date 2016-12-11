using Castle.DynamicProxy;

namespace Saturn72.Module.Ioc.Autofac.Tests.TestObjects
{
    public class TestInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            (invocation.Arguments[0] as TestObject).Value = "Set by interceptor";
        }
    }
}