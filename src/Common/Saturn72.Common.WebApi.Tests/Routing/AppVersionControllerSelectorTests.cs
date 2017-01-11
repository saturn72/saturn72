#region

using System.Net.Http;
using System.Web.Http;
using NUnit.Framework;
using Saturn72.Common.App;
using Saturn72.Common.WebApi.Routing;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Common.WebApi.Tests.Routing
{
    public class AppVersionControllerSelectorTests
    {
        [Test]
        [Ignore("unknown failure readon. need to sync newtonsoft version.")]
        [Category("ignored")]
        public void SelectController_ThrowsOnMissingRouteData()
        {
            var config = new HttpConfiguration();

            var avcs = new AppVersionControllerSelector(config);
            typeof (HttpResponseException).ShouldBeThrownBy(() => avcs.SelectController(new HttpRequestMessage()));
        }

        //    [Test]
        //    public void SelectController_ThrowsOnMissingControllerName()
        //    {
        //        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/test");
        //        var config = new HttpConfiguration();
        //        var actionSelector = config.Services.GetActionSelector();
        //        var controllerSelector = config.Services.GetHttpControllerSelector();
        //        config.EnsureInitialized();
        //        var routeData = config.Routes.GetRouteData(request);
        //        request.SetRouteData(routeData);

        //        var avcs = new AppVersionControllerSelector(config);

        //        typeof(HttpResponseException).ShouldBeThrownBy(() => avcs.SelectController(request));
        //    }

        //    [Test]
        //    public void SelectController_ControllerWasAttributesWithAppVersion()
        //    {
        //        var config = new HttpConfiguration();


        //        var avcs = new AppVersionControllerSelector(config);
        //        var request = new HttpRequestMessage
        //        {
        //            RequestUri = new Uri("http://localhost:8080/v1/test"),
        //        };


        //      //  var controllerContext = new HttpControllerContext(config, )


        //        var actual = avcs.SelectController(request);
        //        actual.ShouldBe<TestController>();
        //    }
    }
}

namespace Saturn72.Common.WebApi.Tests.Routing.NS1
{
    [AppVersion("v1")]
    public class TestController : ApiController
    {
        public string[] Get()
        {
            return new[] {"a1", "b1"};
        }
    }
}

namespace Saturn72.Common.WebApi.Tests.Routing.NS2
{
    [AppVersion("v2")]
    public class TestController : ApiController
    {
        public string[] Get()
        {
            return new[] {"a2", "b2"};
        }
    }
}