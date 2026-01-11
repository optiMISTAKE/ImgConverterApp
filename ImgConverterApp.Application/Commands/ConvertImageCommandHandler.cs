using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ImgConverterApp.Application.Images;
using ImgConverterApp.Application.Interfaces;

namespace ImgConverterApp.Application.Commands
{
    public class ConvertImageCommandHandler: IRequestHandler<ConvertImageCommand, ImageResponseDto>
    {
        private readonly IImageService _imageService;

        public ConvertImageCommandHandler(IImageService imageService)
        {
            _imageService = imageService;
        }

        public async Task<ImageResponseDto> Handle(ConvertImageCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                throw new ArgumentException("File is null or empty");
            }

            var ext = Path.GetExtension(request.File.FileName).ToLower();
            if (ext != ".webp")
            {
                throw new ArgumentException("Only .webp files are supported");
            }

            using var stream = request.File.OpenReadStream();

            // calling infrastructure service to process image
            var resultImage = await _imageService.ProcessImageAsync(
                fileStream: stream,
                originalFileName: request.File.FileName,
                userId: request.UserId,
                fileSize: request.File.Length
            );

            return new ImageResponseDto
            {
                Id = resultImage.Id,
                OriginalFileName = resultImage.OriginalFileName,
                StoredName = resultImage.StoredFileName,
                // TO-DO: construct actual download URL based on routing, for now we construct a path
                DownloadUrl = $"/images/download/{resultImage.Id}" // example URL
            };
        }
    }
}
