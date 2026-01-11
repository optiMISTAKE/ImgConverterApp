using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ImgConverterApp.Domain.Entities;
using ImgConverterApp.Application.Interfaces;
using ImgConverterApp.Application.Auth;

namespace ImgConverterApp.Application.Commands
{
    public class RegisterCommandHandler: IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        public RegisterCommandHandler(UserManager<AppUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // check if user with the same email already exists
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                throw new Exception("User with this email already exists");
            }

            var user = new AppUser
            {
                UserName = request.Username,
                Email = request.Email,
                // CreatedAt will be set in the constructor, no need to set it here
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var token = _tokenService.CreateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Username = user.UserName
            };
        }
    }
}
