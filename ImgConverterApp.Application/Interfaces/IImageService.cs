using ImgConverterApp.Application.Images;
using ImgConverterApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgConverterApp.Application.Interfaces
{
    public interface IImageService
    {
        Task<UserImage> ProcessImageAsync (Stream fileStream, string originalFileName, string userId, long fileSize);
        // method that retrieves an image
        Task<FileResponseDto> GetImageAsync(Guid imageId, string userId); // userId to 
    }
}
