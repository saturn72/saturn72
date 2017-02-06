
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
            response1.Commited.ShouldBeTrue();

            //should return false ==> due to error messsages
            var response2 = new TestResponse();
            response2.AddErrorMessage("This is error message");
            response2.Commited.ShouldBeFalse();

            //should return false ==> due to error messsages
            var response3 = new TestResponse();
            response3.AddErrorMessage("This is error message");
            response3.Commited = true;
            response3.Commited.ShouldBeFalse();

            //should return false ==> due to error messsages
            var response4 = new TestResponse();
            response4.AddErrorMessage("This is error message");
            response4.Commited = false;
            response4.Commited.ShouldBeFalse();

            //should return false due to is valid set to false
            var response5 = new TestResponse();
            response5.Commited = false;
            response5.Commited.ShouldBeFalse();
        }

        [Test]
        public void ServiceResponseBase_Autorized()
        {
            //should return true
            var response1 = new TestResponse();
            response1.Authorized.ShouldBeTrue();
            response1.Commited.ShouldBeTrue();

            //should return false
            var response2 = new TestResponse();
            response2.Authorized = false;
            response2.Authorized.ShouldBeFalse();
            response2.Commited.ShouldBeFalse();
        }


        internal class TestResponse : ServiceResponseBase
        {

        }
    }
}
