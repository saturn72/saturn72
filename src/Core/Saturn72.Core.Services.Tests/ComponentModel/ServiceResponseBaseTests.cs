
using NUnit.Framework;
using Saturn72.Core.Services.ComponentModel;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Services.Tests.ComponentModel
{
    public class ServiceResponseBaseTests
    {
        [Test]
        public void ServiceResponseBase_IsValid()
        {
            //should return true
            var response1 = new TestResponse();
            response1.HasErrors.ShouldBeFalse();

            //should return false ==> due to error messsages
            var response2 = new TestResponse();
            response2.AddErrorMessage("This is error message");
            response2.HasErrors.ShouldBeTrue();

            //should return false ==> due to error messsages
            var response3 = new TestResponse();
            response3.AddErrorMessage("This is error message");
            response3.HasErrors = true;
            response3.HasErrors.ShouldBeTrue();

            //should return false ==> due to error messsages
            var response4 = new TestResponse();
            response4.AddErrorMessage("This is error message");
            response4.HasErrors = false;
            response4.HasErrors.ShouldBeTrue();

            //should return false due to is valid set to false
            var response5 = new TestResponse();
            response5.HasErrors = false;
            response5.HasErrors.ShouldBeFalse();
        }

        [Test]
        public void ServiceResponseBase_Autorized()
        {
            //should return true
            var response1 = new TestResponse();
            response1.HasErrors.ShouldBeFalse();
            response1.Authorized.ShouldBeTrue();


            //should return false
            var response2 = new TestResponse();
            response2.Authorized = false;
            response2.Authorized.ShouldBeFalse();
            response2.HasErrors.ShouldBeTrue();

            //should return false
            var response3 = new TestResponse();
            response3.Authorized = true;
            response3.Authorized.ShouldBeTrue();
            response3.HasErrors.ShouldBeFalse();
        }


        internal class TestResponse : ServiceResponseBase
        {

        }
    }
}
