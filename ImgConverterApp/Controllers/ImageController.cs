using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ImgConverterApp.Application.Commands;
using Microsoft.AspNetCore.Authorization;
using ImgConverterApp.Application.Queries;

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

        [HttpGet("download/{imageId}")]
        public async Task<IActionResult> DownloadImage(Guid imageId)
        {
            try
            {
                // extract user ID from token claims
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                // send query to get the image
                var query = new GetImageQuery(imageId, userId);
                var fileResponse = await _mediator.Send(query);

                // return the file
                return File(fileResponse.Stream, fileResponse.ContentType, fileResponse.FileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound("Image not found.");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You do not have access to this image."); // 403 Forbidden
            }
            catch (Exception)
            {
                // log exception (not implemented here) [TO-DO] 
                return StatusCode(500, "An error occurred while retrieving the image.");
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetConversionHistory()
        {
            try
            {
                // extract user ID from token claims
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }
                var query = new GetHistoryQuery(userId);
                var history = await _mediator.Send(query);
                return Ok(history);
            }
            catch (Exception)
            {
                // log exception (not implemented here) [TO-DO] 
                return StatusCode(500, "An error occurred while retrieving conversion history.");
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteImages([FromBody] List<Guid> imageIds)
        {
            try
            {
                // extract user ID from token claims
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }
                var command = new DeleteImagesCommand(imageIds, userId);
                await _mediator.Send(command);
                return NoContent(); // 204 No Content
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You do not have permission to delete one or more of the specified images.");
            }
            catch (Exception)
            {
                // log exception (not implemented here) [TO-DO] 
                return StatusCode(500, "An error occurred while deleting images.");
            }
        }

        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteAllImages()
        {
            try
            {
                // extract user ID from token claims
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }
                var command = new DeleteImagesCommand(null, userId); // TO-DO: interpret null as delete all in handler
                await _mediator.Send(command);
                return NoContent(); // 204 No Content
            }
            catch (Exception)
            {
                // log exception (not implemented here) [TO-DO] 
                return StatusCode(500, "An error occurred while deleting all images.");
            }
        }
    }
}
