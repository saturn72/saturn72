#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services.Data.Repositories;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Tasks;
using Saturn72.Core.Tasks;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.Tasks
{
    public class BackgroundTaskService : DomainModelCrudServiceBase<BackgroundTaskDomainModel, long>,
        IBackgroundTaskService
    {
        private readonly IBackgroundTaskRepository _backgroundTaskRepository;
        private readonly BackgroundTaskSettings _settings;
        private readonly ITaskManager _taskManager;

        #region ctor

        public BackgroundTaskService(IBackgroundTaskRepository backgroundTaskRepository, IEventPublisher eventPublisher,
            ITypeFinder typeFinder, ITaskManager taskManager,
            ICacheManager cacheManager, BackgroundTaskSettings settings)
            : base(backgroundTaskRepository, eventPublisher, cacheManager, typeFinder)
        {
            _taskManager = taskManager;
            _backgroundTaskRepository = backgroundTaskRepository;
            _settings = settings;
        }

        #endregion

        public async Task UpdateScehduleTaskAsync(BackgroundTaskDomainModel task)
        {
            await Task.Run(() =>
            {
                HandleAttachtments(task);
                _backgroundTaskRepository.Update(task);
                _taskManager.UpdateTask(task);
                EventPublisher.DomainModelUpdated<BackgroundTaskDomainModel, long>(task);
            });
        }


        public Task<IEnumerable<BackgroundTaskDomainModel>> GetAllTasksAsync()
        {
            return Task.Run(() => GetAllTasks());
        }

        public IEnumerable<BackgroundTaskDomainModel> GetAllTasks()
        {
            return _backgroundTaskRepository.GetAll();
        }

        public async Task<BackgroundTaskDomainModel> CreateTaskAsync(BackgroundTaskDomainModel task)
        {
            var attachtments = task.Attachtments;

            task = await _backgroundTaskRepository.CreateAsync(task);
            task.Attachtments = attachtments;

            HandleAttachtments(task);
            _backgroundTaskRepository.Update(task);

            EventPublisher.DomainModelCreated<BackgroundTaskDomainModel, long>(task);
            if (task.Active)
                _taskManager.Schedule(task);

            return task;
        }

        public Task<IEnumerable<IBackgroundTaskType>> GetAllTaskTypesAsync()
        {
            return Task.Run(() => GetAllTaskTypes());
        }

        public IEnumerable<IBackgroundTaskType> GetAllTaskTypes()
        {
            return CacheManager.Get(AllTaskTypesKey, 1440, () =>
            {
                var result = new List<IBackgroundTaskType>();
                TypeFinder.FindClassesOfTypeAndRunMethod<IBackgroundTaskType>(result.Add);
                return result;
            });
        }

        public async Task<IEnumerable<string>> GetAllFullTrustBackgroundTaskTypeNames()
        {
            return await Task.Run(() =>
                CacheManager.Get(FullTrustCacheKey,
                    () => TypeFinder.FindClassesOfType<BackgroundTaskBase>()
                        .Select(t => t.FullName + ", " + t.Assembly.GetName().Name)));
        }

        public async Task<BackgroundTaskDomainModel> GetTaskByIdAsync(long id)
        {
            return await Task.Run(() => _backgroundTaskRepository.GetById(id));
        }

        public async Task DeleteBackgroundTask(long id)
        {
            var task = await GetTaskByIdAsync(id);
            _taskManager.Remove(task);
            await Task.Run(() => _backgroundTaskRepository.Delete(id));
            EventPublisher.DomainModelDeleted<BackgroundTaskDomainModel, long>(task);
        }

        public async Task RunNow(long taskId)
        {
            await Task.Run(() => _taskManager.RunNow(taskId));
        }

        #region Utitlities

        private void HandleAttachtments(BackgroundTaskDomainModel task)
        {
            if (task.Attachtments.IsEmptyOrNull())
                return;

            task.Attachtments.ForEachItem(att => att.BackgroundTaskId = task.Id);

            if (_settings.SaveToDatabase)
            {
                SaveAttachtmentsToDatabaseWithAttachtmentsAsByteArray(task);
            }
            else
            {
                DeleteFileSystemObject(task);
                SaveAttachtmentToLocalPath(task);
            }
        }

        private void DeleteFileSystemObject(BackgroundTaskDomainModel task)
        {
            if (_settings.CompressAllAttachtmentsToPackage)
            {
                throw new NotImplementedException();
            }


            var notNewAttachtments = task.Attachtments.Where(a => a.Id != default(long)).ToArray();
            var taskLocalFolder = GetTaskLocalPath(task);
            var files = Directory.GetFiles(taskLocalFolder);
            var todelete = files.Where(f => !notNewAttachtments.Select(n => n.FilePath).Contains(f));
            todelete.ForEachItem(FileSystemUtil.DeleteFile);
        }


        private void SaveAttachtmentToLocalPath(BackgroundTaskDomainModel task)
        {
            var taskFolder = GetTaskLocalPath(task);

            if (_settings.CompressAllAttachtmentsToPackage)
            {
                throw new NotImplementedException();
            }

            foreach (var attachtment in task.Attachtments)
            {
                if (attachtment.Id != default(long))
                    continue;
                var fileLocalPath = Path.Combine(taskFolder, attachtment.FilePath);

                File.WriteAllBytes(fileLocalPath, attachtment.Bytes);
                attachtment.FilePath = fileLocalPath;
            }
        }


        private void SaveAttachtmentsToDatabaseWithAttachtmentsAsByteArray(BackgroundTaskDomainModel task)
        {
            foreach (var attachtment in task.Attachtments)
            {
                Guard.HasValue(attachtment.FilePath);
                using (var stream = File.OpenRead(attachtment.FilePath))
                {
                    var newFile = stream.ToByteArray();
                    if (attachtment.Bytes.SequenceEqual(newFile))
                        continue;

                    attachtment.Bytes = newFile;

                    attachtment.FilePath = null;
                    stream.Close();
                }
            }
        }

        private string GetTaskLocalPath(BackgroundTaskDomainModel task)
        {
            Guard.NotNull(task);
            Guard.MustFollow(task.Id != default(long));
            var taskId = task.Id;

            var taskFolder = FileSystemUtil.RelativePathToAbsolutePath(_settings.RootSavePath ??
                                                                       "BackgroundTaskAttachtments" + "\\" + taskId);
            FileSystemUtil.CreateDirectoryIfNotExists(taskFolder);
            return taskFolder;
        }

        #endregion

        #region consts

        private const string AllTaskTypesKey = "TaskTypes.";
        private const string FullTrustCacheKey = "FullTrustCacheKey";

        #endregion
    }
}