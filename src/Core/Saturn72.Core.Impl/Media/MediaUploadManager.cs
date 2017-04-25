using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Domain.FileUpload;
using Saturn72.Core.Logging;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Media;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Impl.Media
{
    public class MediaUploadManager : IMediaUploadManager
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger _logger;
        private readonly IMediaRepository _mediaRepository;
        private readonly IMediaUploadValidationFactory _mediaUploadValidationFactory;

        public MediaUploadManager(IMediaUploadValidationFactory mediaUploadValidationFactory, ILogger logger,
            IEventPublisher eventPublisher, IMediaRepository mediaRepository)
        {
            _mediaUploadValidationFactory = mediaUploadValidationFactory;
            _logger = logger;
            _eventPublisher = eventPublisher;
            _mediaRepository = mediaRepository;
        }

        public bool IsSupportedExtension(string extension)
        {
            return _mediaUploadValidationFactory.IsSupportedExtension(extension);
        }

        public async Task<IEnumerable<MediaUploadResponse>> UploadAsync(IEnumerable<MediaUploadRequest> requests)
        {
            Guard.NotNull(requests);
            var fileUploadResponses = new List<MediaUploadResponse>();
            if (requests.IsEmptyOrNull())
                return fileUploadResponses;

            var uploadSessionId = new Guid();
            await Task.Run(() =>
                requests.ForEachItem(req => fileUploadResponses.Add(Upload(req,uploadSessionId))));
            return fileUploadResponses;
        }

        private MediaUploadResponse Upload(MediaUploadRequest request, Guid uploadSessionId)
        {
            if (request.Bytes.IsEmptyOrNull())
                return new MediaUploadResponse(request, MediaStatusCode.Invalid, null, "Invalid upload request");

            if (!IsSupportedExtension(request.Extension))
                return new MediaUploadResponse(request, MediaStatusCode.NotSupported, null, "failed to validate");

            var fileStatusCode = _mediaUploadValidationFactory.Validate(request);
            if (fileStatusCode != MediaStatusCode.Valid)
                return new MediaUploadResponse(request, fileStatusCode, null, "failed to validate");

            _logger.Information("Start uploading file to server. File name {0}".AsFormat(request.FileName));

            var media = new MediaModel
            {
                FileName = request.FileName,
                Bytes = request.Bytes,
                UploadSessionId = uploadSessionId
            };
            _mediaRepository.Create(media);
            if (media.Id == default(long))
            {
                _logger.Information(
                    "Failed to upload file to server. File name {0}, RecordId: {1}, UploadSessionId: {2}".AsFormat(request.FileName, media, uploadSessionId));
                return new MediaUploadResponse(request, MediaStatusCode.FailedToUpload, media,
                    "File failed to upload");
            }

            _logger.Information(
                "Finish uploading file to server. File name {0}, RecordId: {1}, UploadSessionId: {2}".AsFormat(request.FileName, media, uploadSessionId));
            _eventPublisher.DomainModelCreated(media);
            return new MediaUploadResponse(request, MediaStatusCode.Uploaded, media,
                "File uploaded successfully");
        }
    }
}