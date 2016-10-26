#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Saturn72.Core.Tests.TestObjects;
using Saturn72.Extensions;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Tests
{
    public class CommonHelperTests
    {
        [Test]
        public void IsValidEmail_AllTests()
        {
            var validEmails = new[] {"rrr@rmail.com", "123@essss.com"};
            var invalidEmails = new[] {"rrr@w.com", "@e.com"};

            foreach (var email in validEmails)
            {
                CommonHelper.IsValidEmail(email).ShouldBeTrue();
            }

            //foreach (var email in invalidEmails)
            //{
            //    CommonHelper.IsValidEmail(email).ShouldBeFalse();
            //}
        }
        [Test]
        public void GetCompatibleTypeName()
        {
            const string stringCompatibleName = "System.String, mscorlib";

            CommonHelper.GetCompatibleTypeName<string>().ShouldEqual(stringCompatibleName);
            CommonHelper.GetCompatibleTypeName(typeof (string)).ShouldEqual(stringCompatibleName);
        }

        [Test]
        public void TryGetTypeFromAppComain_ReturnsaType()
        {
            CommonHelper.TryGetTypeFromAppDomain("string").ShouldBeType<string>();
            CommonHelper.TryGetTypeFromAppDomain("System.String, mscorlib").ShouldBeType<string>();
        }

        [Test]
        public void TryGetTypeFromAppComain_ReturnsaNull()
        {
            CommonHelper.TryGetTypeFromAppDomain("dddd").ShouldBeNull();
            CommonHelper.TryGetTypeFromAppDomain("notExsistTypeName, NotExistAssembly").ShouldBeNull();
            CommonHelper.TryGetTypeFromAppDomain("notExsistTypeName, NotExistAssembly, whatever").ShouldBeNull();
        }

        [Test]
        public void GetTypeFromAppDomain_FromTypeName_ThrowsOnBadTypeName()
        {
            var typeName = "BlaBla, BlaBla";
            typeof(ArgumentException).ShouldBeThrownBy(() => CommonHelper.GetTypeFromAppDomain(typeName));

            typeName = "RRR";
            typeof (ArgumentException).ShouldBeThrownBy(() => CommonHelper.GetTypeFromAppDomain(typeName));

           
        }

        [Test]
        public void GetTypeFromAppDomain_FromTypeName_ReturnsNull()
        {
            var typeName = "Saturn72.Core.Tests.BlaBla, Saturn72.Core.Tests";
            CommonHelper.GetTypeFromAppDomain(typeName).ShouldBeNull();
        }

        [Test]
        public void GetTypeFromAppDomain_FromTypeName_GetsType()
        {
            var typeName = "Saturn72.Core.Tests.TestObjects.TestObject, Saturn72.Core.Tests";
            var t = CommonHelper.GetTypeFromAppDomain(typeName);
            t.ShouldBeType<TestObject>();
        }

        [Test]
        public void GetTypeFromAppDomain_FromTypeAndAssemblyNames_ThrowsOnBadTypeName()
        {
            var typeName = "RRR";
            typeof (ArgumentException).ShouldBeThrownBy(() => CommonHelper.GetTypeFromAppDomain("", typeName));
        }

        [Test]
        public void GetTypeFromAppDomain_FromTypeAndAssemblyNames_ReturnsNull()
        {
            var typeName = "Saturn72.Core.Tests.BlaBla, Saturn72.Core.Tests";
            CommonHelper.GetTypeFromAppDomain(typeName).ShouldBeNull();
        }

        [Test]
        public void GetTypeFromAppDomain_FromTypeAndAssemblyNames_GetsType()
        {
            CommonHelper.GetTypeFromAppDomain("Saturn72.Core.Tests.TestObjects.TestObject", "Saturn72.Core.Tests")
                .ShouldBeType<TestObject>();
        }

        [Test]
        public void CreateInstnce_CreatesInstanceFromTypeName()
        {
            var type = typeof (List);
            var typeName = type.FullName + ", " + type.Assembly.GetName().Name;
            var instance = CommonHelper.CreateInstance<List>(typeName);

            Assert.IsInstanceOf<List>(instance);
        }

        [Test]
        public void CreateInstnce_CreatesInstanceFromTypeNameWithParams()
        {
            var type = typeof (TestObject);
            var typeName = type.FullName + ", " + type.Assembly.GetName().Name;
            var instance = CommonHelper.CreateInstance<TestObject>(typeName, "objName", new List<string> {"1", "2", "3"});

            Assert.IsInstanceOf<TestObject>(instance);
            instance.Name.ShouldEqual("objName");
            instance.List.ToList().ShouldCount(3);
        }


        [Test]
        public void CreateInstnce_CreatesInstanceFromType()
        {
            var type = typeof (List);
            var instance = CommonHelper.CreateInstance<List>(type);

            Assert.IsInstanceOf<List>(instance);
        }

        [Test]
        public void CreateInstnce_CreatesInstanceFromTypeWithParams()
        {
            var type = typeof (TestObject);
            var instance = CommonHelper.CreateInstance<TestObject>(type, "objName", new List<string> {"1", "2", "3"});

            Assert.IsInstanceOf<TestObject>(instance);
            instance.Name.ShouldEqual("objName");
            instance.List.ToList().ShouldCount(3);
        }

        [Test]
        public void CreateInstnce_ThrowsOnBadTypeName()
        {
            Assert.Throws<ArgumentException>(() => CommonHelper.CreateInstance<List>("TTT"));

            Assert.Throws<ArgumentException>(() => CommonHelper.CreateInstance<List>("TTT, TTT"));
            Assert.Throws<ArgumentException>(() => CommonHelper.CreateInstance<List>("TTT,System"));
        }

        [Test]
        public void CreateInstnce_NullOnBadServiceRequested()
        {
            var instance = CommonHelper.CreateInstance<String>(typeof (List));

            Assert.IsNull(instance);
        }

        [Test]
        public void ToInt_returnsInteger()
        {
            var res = CommonHelper.To<int>("100");
            res.ShouldEqual(100);
        }

        [Test]
        public void ToInt_ReturnsZeroResult()
        {
            var res = CommonHelper.ToInt("");
            res.ShouldEqual(0);

            res = CommonHelper.ToInt("dwwd");
            res.ShouldEqual(0);

            res = CommonHelper.ToInt(null);
            res.ShouldEqual(0);
        }
    }
}