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
        private readonly IMediaUploadValidationFactory _mediaUploadValidationFactory;
        private readonly ILogger _logger;

        public MediaUploadManager(IMediaUploadValidationFactory mediaUploadValidationFactory, ILogger logger,
            IEventPublisher eventPublisher)
        {
            _mediaUploadValidationFactory = mediaUploadValidationFactory;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public bool IsSupportedExtension(string extension)
        {
            return _mediaUploadValidationFactory.IsSupportedExtension(extension);
        }

        public MediaUploadResponse Upload(MediaUploadRequest mediaUploadRequest)
        {
            Guard.NotNull(mediaUploadRequest);

            byte[] bytes;
            if(mediaUploadRequest.Bytes.IsNull() || (bytes = mediaUploadRequest.Bytes()).IsEmptyOrNull())
                return new MediaUploadResponse(mediaUploadRequest, MediaStatusCode.Invalid, null, "Invalid upload request");

            if (!IsSupportedExtension(mediaUploadRequest.Extension))
                return new MediaUploadResponse(mediaUploadRequest, MediaStatusCode.NotSupported, null, "failed to validate");

            var fileStatusCode = _mediaUploadValidationFactory.Validate(mediaUploadRequest);
            if (fileStatusCode != MediaStatusCode.Valid)
                return new MediaUploadResponse(mediaUploadRequest, fileStatusCode, null, "failed to validate");

            _logger.Information("Start uploading file to server. File name {0}".AsFormat(mediaUploadRequest.FileName));

            var media = new MediaModel
            {
                FilePath = mediaUploadRequest.FileName,
                Bytes = bytes
            };

            //   _mediaRepository.Create(media);

            _logger.Information(
                "Finish uploading file to server. File name {0}, RecordId: {1}".AsFormat(mediaUploadRequest.FileName,
                    media));
            _eventPublisher.DomainModelCreated(media);
            return new MediaUploadResponse(mediaUploadRequest, MediaStatusCode.Uploaded, media,
                "File uploaded successfully");
        }
    }
}