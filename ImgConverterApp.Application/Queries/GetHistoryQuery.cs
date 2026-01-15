using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImgConverterApp.Application.Images;

namespace ImgConverterApp.Application.Queries
{
    public record GetHistoryQuery(string UserId) : IRequest<List<UserImageDto>>;
}
