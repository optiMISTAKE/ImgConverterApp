using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImgConverterApp.Application.Interfaces;

namespace ImgConverterApp.Application.Commands
{
    public record DeleteImagesCommand(List<Guid> ImageIds, string UserId): IRequest;
    public class DeleteImagesCommandHandler: IRequestHandler<DeleteImagesCommand>
    {
        private readonly IImageService _imageService;
        public DeleteImagesCommandHandler(IImageService imageService)
        {
            _imageService = imageService;
        }
        public async Task Handle(DeleteImagesCommand request, CancellationToken cancellationToken)
        {
            await _imageService.DeleteImagesAsync(request.ImageIds, request.UserId);
        }
    }
}
