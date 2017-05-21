#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Saturn72.Core.Configuration;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Core.Tasks;
using Shouldly;

#endregion

namespace Saturn72.Core.Tests.Infrastructure
{
    public class EngineDriverTests
    {
        internal static ICollection<int> DependencyIndexes = new List<int>();
        internal static ICollection<int> StartupTaskIndexes = new List<int>();

        [Test]
        public void EngineDriver_RegisterDependencies()
        {
            DependencyIndexes.Clear();
            var config = Saturn72Config.GetConfiguration();
            new AppEngineDriver().Initialize(config);

            var indexArray = DependencyIndexes.ToArray();
            indexArray.Length.ShouldBe(2);
            indexArray[0].ShouldBe(1);
            indexArray[1].ShouldBe(2);
        }

        [Test]
        public void EngineDriver_RunStartupTasks()
        {
            StartupTaskIndexes.Clear();

            var config = Saturn72Config.GetConfiguration();
            new AppEngineDriver().Initialize(config);

            var indexArray = StartupTaskIndexes.ToArray();
            indexArray.Length.ShouldBe(2);
            indexArray[0].ShouldBe(1);
            indexArray[1].ShouldBe(2);
        }
        internal class TestDependencyRegistrar1 : IDependencyRegistrar
        {
            public int RegistrationOrder => 1;

            public Action<IIocRegistrator> RegistrationLogic(ITypeFinder typeFinder, Saturn72Config config)
            {
                return registrator => DependencyIndexes.Add(RegistrationOrder);
            }
        }

        public class TestDependencyRegistrar2 : IDependencyRegistrar
        {
            public int RegistrationOrder => 2;

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

            public int ExecutionIndex => 1;
        }

        public class TestStartupTask2 : IStartupTask
        {
            public void Execute()

            {
                StartupTaskIndexes.Add(ExecutionIndex);
            }

            public int ExecutionIndex => 2;
        }
    }
}