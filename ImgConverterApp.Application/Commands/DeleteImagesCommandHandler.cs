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
            if (request.ImageIds == null)
            {
                await _imageService.DeleteAllImagesAsync(request.UserId);
            }
            else
            {
                await _imageService.DeleteImagesAsync(request.ImageIds, request.UserId);
            }
        }

    }
}
