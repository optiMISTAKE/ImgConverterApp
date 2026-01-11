using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImgConverterApp.Application.Images;
using MediatR;

namespace ImgConverterApp.Application.Queries
{
    public class GetImageQuery: IRequest<FileResponseDto>
    {
        public Guid ImageId { get; set; }
        public string UserId { get; set; }

        public GetImageQuery(Guid imageId, string userId)
        {
            ImageId = imageId;
            UserId = userId;
        }
    }
}
