using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ImgConverterApp.Application.Images;
using Microsoft.EntityFrameworkCore;
using ImgConverterApp.Application.Interfaces;

namespace ImgConverterApp.Application.Queries
{
    public class GetHistoryQueryHandler : IRequestHandler<GetHistoryQuery, List<UserImageDto>>
    {
        private readonly IImageService _imageService;

        public GetHistoryQueryHandler(IImageService imageService)
        {
            _imageService = imageService;
        }

        public async Task<List<UserImageDto>> Handle(GetHistoryQuery request, CancellationToken cancellationToken)
        {
            var images = await _imageService.GetHistoryAsync(request.UserId);
            return images;
        }
    }
}
