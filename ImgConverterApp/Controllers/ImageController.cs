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

        // ENDPOINT - for converting an image from ".webp" to ".png"
        // for a user with a given userId (the img will be tied to that user)
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

        // ENDPOINT - for downloading a converted image from the database
        // with the provided imageId of the png image
        [HttpGet("download/{imageId}")]
        public async Task<IActionResult> DownloadImage(Guid imageId)
        {
            try
            {
                // extract user ID from token claims
                // (the image must be tied to the user)
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

        // ENDPOINT - for getting the 10 last converted images of the user
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
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
                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                // log exception (not implemented here) [TO-DO]
                return StatusCode(500, "An error occurred while retrieving image history.");
            }
        }

        // ENDPOINT - for deleting singular or multiple images from their account
        // and the database
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteImages([FromBody] List<Guid> ids)
        {
            try
            {
                // extract user ID from token claims
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                await _mediator.Send(new DeleteImagesCommand(ids, userId));

                return NoContent();
            }
            catch (Exception ex)
            {
                // log exception (not implemented here) [TO-DO]
                return StatusCode(500, "An error occurred while retrieving image history.");
            }
        }

        // ENDPOINT - for deleting all of the images tied to the specific user in the database
        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                // extract user ID from token claims
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                await _mediator.Send(new DeleteImagesCommand(null, userId));

                return NoContent();
            }
            catch (Exception ex)
            {
                // log exception (not implemented here) [TO-DO]
                return StatusCode(500, "An error occurred while retrieving image history.");
            }
        }
    }
}
