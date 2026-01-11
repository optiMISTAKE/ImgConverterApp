using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImgConverterApp.Application.Images;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ImgConverterApp.Application.Commands
{
    public class ConvertImageCommand: IRequest<ImageResponseDto>
    {
        public IFormFile File { get; set; }
        public string UserId { get; set; } // this will be grabbed from token
    }
}
