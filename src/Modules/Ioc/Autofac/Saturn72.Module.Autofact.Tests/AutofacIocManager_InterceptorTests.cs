using NUnit.Framework;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Module.Ioc.Autofac.Tests.TestObjects;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Module.Ioc.Autofac.Tests
{
    public class AutofacIocManager_InterceptorTests
    {
        [Test]
        public void RegisterType_WithInterceptor()
        {
            var testInterceptor = new TestInterceptor();


            var cm = new AutofacIocContainerManager();
            cm.RegisterInstance(testInterceptor);
            cm.RegisterInstance<ITestService1>(new TestService(), interceptorType: typeof(TestInterceptor));

            ResolveAndAssert< ITestService1>(cm);

            cm = new AutofacIocContainerManager();
            cm.RegisterInstance(testInterceptor);
            cm.RegisterType<TestService, ITestService1>(LifeCycle.SingleInstance,
                interceptorType: typeof(TestInterceptor));
            ResolveAndAssert< ITestService1>(cm);

            cm = new AutofacIocContainerManager();
            cm.RegisterInstance(testInterceptor);
            cm.RegisterType(typeof(TestService), LifeCycle.SingleInstance,interceptorType: typeof(TestInterceptor));
            ResolveAndAssert< TestService>(cm);


            cm = new AutofacIocContainerManager();
            cm.RegisterInstance(testInterceptor);
            cm.RegisterType(typeof(TestService), typeof(ITestService1), LifeCycle.SingleInstance, interceptorType: typeof(TestInterceptor));
            ResolveAndAssert<ITestService1>(cm);

            cm = new AutofacIocContainerManager();
            cm.RegisterInstance(testInterceptor);
            cm.RegisterType(typeof(TestService), new [] { typeof(ITestService1), typeof(ITestService2)}, LifeCycle.SingleInstance, interceptorType: typeof(TestInterceptor));
            ResolveAndAssert<ITestService1>(cm);
            ResolveAndAssert<ITestService2>(cm);

            cm = new AutofacIocContainerManager();
            cm.RegisterInstance(testInterceptor);
            cm.Register<ITestService1>(()=> new TestService() ,LifeCycle.SingleInstance, interceptorType: typeof(TestInterceptor));
            ResolveAndAssert<ITestService1>(cm);

            cm = new AutofacIocContainerManager();
            cm.RegisterInstance(testInterceptor);
            cm.RegisterInstance(new TestService());
            cm.RegisterDelegate<ITestService1>(res => res.Resolve<TestService>(), LifeCycle.SingleInstance, interceptorType: typeof(TestInterceptor));
            ResolveAndAssert<ITestService1>(cm);

            
        }

        private static void ResolveAndAssert<TService>(AutofacIocContainerManager cm)
        {
            var to = new TestObject();
            var srv = cm.Resolve<TService>() as ITestService1;
            srv.Do(to);
            to.Value.Length.ShouldBeGreaterThan(0);
        }
    }
}