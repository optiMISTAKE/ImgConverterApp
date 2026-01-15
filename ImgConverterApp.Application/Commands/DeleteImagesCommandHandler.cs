using ImgConverterApp.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgConverterApp.Application.Commands
{
    public class DeleteImagesCommandHandler : IRequestHandler<DeleteImagesCommand>
    {
        private readonly IImageService _imageService;
        public DeleteImagesCommandHandler(IImageService imageService) => _imageService = imageService;

        public async Task Handle(DeleteImagesCommand request, CancellationToken ct)
        {
            // if the method gets "null" for imageIds - we read it as a request
            // for deleting all images from that user
            if (request.ImageIds == null)
            {
                await _imageService.DeleteAllImagesAsync(request.UserId);
            }
            // otherwise delete the image with the given imageId
            else
            {
                await _imageService.DeleteImagesAsync(request.ImageIds, request.UserId);
            }
        }

    }
}
