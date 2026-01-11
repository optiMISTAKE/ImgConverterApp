using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImgConverterApp.Application.Auth;
using MediatR;

namespace ImgConverterApp.Application.Commands
{
    // Command for registering a new user, returns an AuthResponseDto upon success
    public class RegisterCommand: IRequest<AuthResponseDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
