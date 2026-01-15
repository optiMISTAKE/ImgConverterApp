using MediatR;
using Microsoft.AspNetCore.Mvc;
using ImgConverterApp.Application.Auth;
using ImgConverterApp.Application.Commands;
using ImgConverterApp.Application.Queries;

namespace ImgConverterApp.Controllers
{
    public class AuthController: BaseApiController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ENDPOINT - for registering new user in the system
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            try
            {
                var command = new RegisterCommand
                {
                    Email = registerDto.Email,
                    Username = registerDto.Username,
                    Password = registerDto.Password
                };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ENDPOINT - for logging in
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            try
            {
                var query = new LoginQuery
                {
                    Email = loginDto.Email,
                    Password = loginDto.Password
                };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
