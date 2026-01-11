using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ImgConverterApp.Application.Commands;
using Microsoft.AspNetCore.Authorization;

namespace ImgConverterApp.Controllers
{
    [Authorize]
    public class ImageController : BaseApiController
    {
        private readonly IMediator _mediator;
        public ImageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("convert")]
        public async Task<IActionResult> ConvertImage(IFormFile file)
        {
            try
            {
                // extract user ID from token claims
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var command = new ConvertImageCommand
                {
                    File = file,
                    UserId = userId
                };

                var result = await _mediator.Send(command);

                return Ok(result);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // log exception (not implemented here) [TO-DO] 
                return StatusCode(500, "An error occurred while processing the image.");
            }
        }
    }
}
