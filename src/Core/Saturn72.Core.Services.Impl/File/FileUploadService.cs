using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.FileUpload;
using Saturn72.Core.Logging;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.File;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Impl.File
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger _logger;
        private readonly IFileUploadRecordRepository _fileUploadRecordRepository;
        private readonly IFileUploadValidationFactory _mediaUploadValidationFactory;
        private readonly IFileUploadSessionRepository _fileUploadSessionRepository;
        private readonly ICacheManager _cacheManager;

        public FileUploadService(IFileUploadValidationFactory mediaUploadValidationFactory, ILogger logger, IEventPublisher eventPublisher, IFileUploadRecordRepository fileUploadRecordRepository, IFileUploadSessionRepository fileUploadSessionRepository, ICacheManager cacheManager)
        {
            _mediaUploadValidationFactory = mediaUploadValidationFactory;
            _logger = logger;
            _eventPublisher = eventPublisher;
            _fileUploadRecordRepository = fileUploadRecordRepository;
            _fileUploadSessionRepository = fileUploadSessionRepository;
            _cacheManager = cacheManager;
        }

        public bool IsSupportedExtension(string extension)
        {
            return _mediaUploadValidationFactory.IsSupportedExtension(extension);
        }

        public async Task<IEnumerable<FileUploadResponse>> UploadAsync(IEnumerable<FileUploadRequest> requests)
        {
            Guard.NotNull(requests);
            var fileUploadResponses = new List<FileUploadResponse>();
            if (requests.IsEmptyOrNull())
                return fileUploadResponses;

            var us = new FileUploadSessionModel
            {
                SessionId = Guid.NewGuid()
            };
            await Task.Run(() =>
            {
                _fileUploadSessionRepository.Create(us);
                requests.ForEachItem(req => fileUploadResponses.Add(Upload(req, us)));
            });
            return fileUploadResponses;
        }

        public async Task<IEnumerable<FileUploadRecordModel>> GetFileUploadRecordByUploadSessionIdAsync(long uploadSessionId)
        {
            Guard.GreaterThan(uploadSessionId, (long)0);

            return await _cacheManager.Get(uploadSessionId.ToString(), 6000,
                ()=> Task.FromResult(_fileUploadRecordRepository.GetByUploadSessionId(uploadSessionId)));
        }

        private FileUploadResponse Upload(FileUploadRequest request, FileUploadSessionModel session)
        {
            if (request.Bytes.IsEmptyOrNull())
                return new FileUploadResponse(request, FileStatusCode.Invalid, null, "Invalid upload request");

            if (!IsSupportedExtension(request.Extension))
                return new FileUploadResponse(request, FileStatusCode.Unsupported, null, "Extension not supportted");

            var fileStatusCode = _mediaUploadValidationFactory.Validate(request);
            if (fileStatusCode != FileStatusCode.Valid)
                return new FileUploadResponse(request, fileStatusCode, null, "failed to validate");

            _logger.Information("Start uploading file to server. File name {0}".AsFormat(request.FileName));

            var fur = new FileUploadRecordModel
            {
                FileName = request.FileName,
                Bytes = request.Bytes,
                UploadId = Guid.NewGuid(),
                UploadSession = session,
                UploadSessionId = session.Id
            };

            _fileUploadRecordRepository.Create(fur);
            
            if (fur.Id == default(long))
            {
                _logger.Information(
                    "Failed to upload file to server. File name {0}, RecordId: {1}, UploadSession: {2}".AsFormat(request.FileName, fur, session));
                return new FileUploadResponse(request, FileStatusCode.FailedToUpload, fur,
                    "File failed to upload");
            }

            _logger.Information(
                "Finish uploading file to server. File name {0}, RecordId: {1}, UploadSession: {2}".AsFormat(request.FileName, fur, session));
            _eventPublisher.DomainModelCreated(fur);
            return new FileUploadResponse(request, FileStatusCode.Uploaded, fur,
                "File uploaded successfully");
        }
    }
}