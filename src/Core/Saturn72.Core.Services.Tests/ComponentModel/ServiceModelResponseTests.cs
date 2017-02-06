using NUnit.Framework;
using Saturn72.Core.Domain;
using Saturn72.Core.Services.ComponentModel;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Services.Tests.ComponentModel
{
    public class ServiceModelResponseTests
    {
        [Test]
        public void ServiceModelRespone_IsValid_returnsFalse()
        {
            //on null model
            var response1 = new ServiceModelResponse<DummyModel>(null);
            response1.IsValid.ShouldBeFalse();
            //on error message
            var mdel2 = new DummyModel();
            var response2 = new ServiceModelResponse<DummyModel>(mdel2);
            response2.AddErrorMessage("RRR");
            response2.IsValid.ShouldBeFalse();

            var model3 = new DummyModel();
            var response3 = new ServiceModelResponse<DummyModel>(model3);
            response3.Authorized = false;
            response3.IsValid.ShouldBeFalse();
        }

        internal class DummyModel : DomainModelBase
        {

        }
    }
}
