#region

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Impl.Tasks;
using Saturn72.Core.Services.Tasks;
using Saturn72.Core.Tasks;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Services.Impl.Tests.Tasks
{
    public class BackgroundTaskServiceTests
    {
        private readonly BackgroundTaskDomainModel task1 = new BackgroundTaskDomainModel
        {
            Active = false,
            TaskTypeUniqueIdentifier = "Task1_TaskTypeUniqueIdentifier_Code",
            AdminComment = "Task1 admin comment",
            Cron = "*/2 * * * *",
            Description = "Task1 Description",
            DisplayName = "Task1 Disaply name",
            Name = "Task1 name",
            StopOnError = false
        };

        private readonly BackgroundTaskDomainModel task2 = new BackgroundTaskDomainModel
        {
            Active = true,
            TaskTypeUniqueIdentifier = "Task2_TaskTypeUniqueIdentifier_Code",
            AdminComment = "Task1 admin comment",
            Cron = "*/2 * * * *",
            Description = "Task2 Description",
            DisplayName = "Task2 Disaply name",
            Name = "Task2 name",
            StopOnError = true
        };

        private IBackgroundTaskService _backgroundTaskService;

        [Test]
        [Category("sanity")]
        public void AddsBackgroundTasks_CrudToDatabase()
        {
            var btList = new List<BackgroundTaskDomainModel>();

            var insertIndex = 0;
            var btRepository = new Mock<IBackgroundTaskRepository>();
            btRepository.Setup(r => r.CreateAsync(It.IsAny<BackgroundTaskDomainModel>()))
                .Callback<BackgroundTaskDomainModel>(t =>
                {
                    t.Id = ++insertIndex;
                    btList.Add(t);
                })
                .Returns<BackgroundTaskDomainModel>(Task.FromResult);

            btRepository.Setup(r => r.Update(It.IsAny<BackgroundTaskDomainModel>()))
                .Callback<BackgroundTaskDomainModel>(t =>
                {
                    var originalTask = btList.FirstOrDefault(b => b.Id == t.Id);
                    btList.Remove(originalTask);
                    btList.Add(t);
                })
                .Returns<BackgroundTaskDomainModel>(t => t);

            btRepository.Setup(r => r.GetById(It.IsAny<long>()))
                .Returns<long>(id => btList.FirstOrDefault(t=>t.Id == id));

            var btSettings = new BackgroundTaskSettings
            {
                CompressAllAttachtmentsToPackage = false,
                SaveToDatabase = true
            };

            var eventPublisher = new Mock<IEventPublisher>();
            var typeFinder = new Mock<ITypeFinder>();
            var taskManager = new Mock<ITaskManager>();
            var cacheManager = new Mock<ICacheManager>();

            _backgroundTaskService = new BackgroundTaskService(btRepository.Object, eventPublisher.Object,
                typeFinder.Object,
                taskManager.Object, cacheManager.Object, btSettings, null);

            //create task
            var t1 = CrateTaskAndAssert(task1);

            //----- UPDATE TASK
            ModifyParametersAndAssert(t1);
            ModifyAttachtmentsAndAssert(t1);

            var t2 = CrateTaskAndAssert(task2);
        }

        private void ModifyAttachtmentsAndAssert(BackgroundTaskDomainModel task)
        {
            //Create 
            task.Attachtments = new List<BackgroundTaskAttachtmentDomainModel>
            {
                new BackgroundTaskAttachtmentDomainModel {FilePath = "FilePath_1", IsPrimary = true},
                new BackgroundTaskAttachtmentDomainModel {FilePath = "FilePath_2", IsPrimary = false},
                new BackgroundTaskAttachtmentDomainModel {FilePath = "FilePath_3", IsPrimary = false},
                new BackgroundTaskAttachtmentDomainModel {FilePath = "FilePath_4", IsPrimary = true}
            };

            UpdateAndAssert(task);
            //Delete
            task.Attachtments.Remove(task.Attachtments.FirstOrDefault(x => x.FilePath.Equals("FilePath_3")));
            UpdateAndAssert(task);
            //Update

            Thread.Sleep(5000);
            task.Attachtments.FirstOrDefault(x => x.FilePath.Equals("FilePath_2")).FilePath = "new FileName";
            UpdateAndAssert(task);
        }

        private void UpdateAndAssert(BackgroundTaskDomainModel task)
        {
            _backgroundTaskService.UpdateScehduleTaskAsync(task).Wait();
            var updated = _backgroundTaskService.GetTaskByIdAsync(task.Id).Result;
            updated.PropertyValuesAreEquals(task, new[] {"Id", "UpdatedOnUtc", "CreatedOnUtc", "Attachtments"});

            task.Attachtments.ForEach(x => x.BackgroundTaskId = task.Id);

            task.Attachtments.Count.ShouldEqual(updated.Attachtments.Count);
            var expectedArray = task.Attachtments.ToArray();
            var actualArray = updated.Attachtments.ToArray();

            for (var i = 0; i < expectedArray.Length; i++)
                actualArray[i].PropertyValuesAreEquals(expectedArray[i], new[] {"Id", "CreatedOnUtc", "UpdatedOnUtc"});
        }

        private void ModifyParametersAndAssert(BackgroundTaskDomainModel task)
        {
//add parameters
            task.Parameters = new Dictionary<string, string>
            {
                {"t1_key1", "t1_value1"},
                {"t1_key2", "t1_value2"},
                {"t1_key3", "t1_value3"},
                {"t1_key4", "t1_value4"}
            };
            UpdateAndAssert(task);

            //remove parameter
            task.Parameters.Remove("t1_key1");
            UpdateAndAssert(task);

            //modify parameter
            task.Parameters["t1_key2"] = "new Value";
            UpdateAndAssert(task);

            //add to collection
            task.Parameters.Add("new_key", "new Value");
            UpdateAndAssert(task);
        }

        private BackgroundTaskDomainModel CrateTaskAndAssert(BackgroundTaskDomainModel task)
        {
            var t1 = _backgroundTaskService.CreateTaskAsync(task).Result;
            t1.PropertyValuesAreEquals(task, new[] {"id"});

            return t1;
        }
    }
}