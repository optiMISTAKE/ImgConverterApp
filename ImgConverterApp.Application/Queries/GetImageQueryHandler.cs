using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImgConverterApp.Application.Images;
using ImgConverterApp.Application.Interfaces;

namespace ImgConverterApp.Application.Queries
{
    public class GetImageQueryHandler: IRequestHandler<GetImageQuery, FileResponseDto>
    {
        private readonly IImageService _imageService;

        public GetImageQueryHandler(IImageService imageService)
        {
            _imageService = imageService;
        }

        public async Task<FileResponseDto> Handle(GetImageQuery request, CancellationToken cancellationToken)
        {
            var fileResponse = await _imageService.GetImageAsync(request.ImageId, request.UserId);
            return fileResponse;
        }
    }
}
