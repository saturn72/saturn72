using NUnit.Framework;
using Saturn72.Core.Domain;
using Saturn72.Core.Services.ComponentModel;
using Shouldly;

namespace Saturn72.Core.Services.Tests.ComponentModel
{
    public class ServiceModelResponseTests
    {
        [Test]
        public void ServiceModelRespone_IsValid_HasErrors()
        {
            //on null model
            var response1 = new ServiceModelResponse<DummyModel> {Model = null};
            response1.HasErrors.ShouldBeTrue();

            //on error message
            var mdel2 = new DummyModel();
            var response2 = new ServiceModelResponse<DummyModel> { Model = mdel2};
            response2.AddErrorMessage("RRR");
            response2.HasErrors.ShouldBeTrue();

            var model3 = new DummyModel();
            var response3 = new ServiceModelResponse<DummyModel> { Model = model3};
            response3.Authorized = false;
            response3.HasErrors.ShouldBeTrue();
        }

        [Test]
        public void ServiceModelRespone_IsValid_DoesNotHasErrors()
        {
            //on error message
            var mdel2 = new DummyModel();
            var response2 = new ServiceModelResponse<DummyModel> { Model = mdel2 };
            response2.HasErrors.ShouldBeFalse();
        }

        internal class DummyModel : DomainModelBase
        {

        }
    }
}
