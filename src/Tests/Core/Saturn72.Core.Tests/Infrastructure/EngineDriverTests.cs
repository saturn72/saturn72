#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;
using Saturn72.Core.Configuration;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Core.Tasks;
using Saturn72.Core.Tests.TestObjects;

#endregion

namespace Saturn72.Core.Tests.Infrastructure
{
    public class EngineDriverTests
    {
        internal static ICollection<int> DependencyIndexes = new List<int>();
        internal static ICollection<int> StartupTaskIndexes = new List<int>();

        [Test]
        public void RegisterDependencies()
        {
            DependencyIndexes.Clear();
            var config = Saturn72Config.GetConfiguration();
            new AppEngineDriver().Initialize(config);

            var indexArray = DependencyIndexes.ToArray();
            indexArray.Length.ShouldEqual(2);
            indexArray[0].ShouldEqual(1);
            indexArray[1].ShouldEqual(2);
        }

        [Test]
        public void RunStartupTasks()
        {
            StartupTaskIndexes.Clear();

            new AppEngineDriver().Initialize(Saturn72Config.GetConfiguration());

            var indexArray = StartupTaskIndexes.ToArray();
            indexArray.Length.ShouldEqual(2);
            indexArray[0].ShouldEqual(1);
            indexArray[1].ShouldEqual(2);
        }


        internal class TestDependencyRegistrar1 : IDependencyRegistrar
        {
            public int RegistrationOrder
            {
                get { return 1; }
            }

            public Action<IIocRegistrator> RegistrationLogic(ITypeFinder typeFinder, Saturn72Config config)
            {
                return registrator => DependencyIndexes.Add(RegistrationOrder);
            }
        }

        public class TestDependencyRegistrar2 : IDependencyRegistrar
        {
            public int RegistrationOrder
            {
                get { return 2; }
            }

            public Action<IIocRegistrator> RegistrationLogic(ITypeFinder typeFinder, Saturn72Config config)
            {
                return registrator => DependencyIndexes.Add(RegistrationOrder);
            }
        }


        public class TestStartupTask1 : IStartupTask
        {
            public void Execute()

            {
                StartupTaskIndexes.Add(ExecutionIndex);
            }

            public int ExecutionIndex
            {
                get { return 1; }
            }
        }

        public class TestStartupTask2 : IStartupTask
        {
            public void Execute()

            {
                StartupTaskIndexes.Add(ExecutionIndex);
            }

            public int ExecutionIndex
            {
                get { return 2; }
            }
        }
    }
}