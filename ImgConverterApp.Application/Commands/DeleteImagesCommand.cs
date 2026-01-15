using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgConverterApp.Application.Commands
{
    public record DeleteImagesCommand(List<Guid> ImageIds, string UserId) : IRequest;
}
