using ImgConverterApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using ImgConverterApp.Domain.Entities;
using ImgConverterApp.Domain.Enums;

namespace ImgConverterApp.Infrastructure.Services
{
    public class ImageService: IImageService
    {
        private readonly AppDbContext _context;
        // folder where images will be stored
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "ImageStorage");

        public ImageService(AppDbContext context)
        {
            _context = context;
            // ensure storage directory exists
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task<UserImage> ProcessImageAsync(Stream fileStream, string originalFileName, string userId, long fileSize)
        {
            // generate unique file names
            var fileId = Guid.NewGuid();
            var storedFileName = $"{fileId}.png";
            var fullPath = Path.Combine(_storagePath, storedFileName);

            // load and convert using ImageSharp
            fileStream.Position = 0; // reset stream position

            using (var image = await Image.LoadAsync(fileStream))
            {
                // save as PNG
                await image.SaveAsync(fullPath, new PngEncoder());
            }

            // create UserImage entity
            var userImage = new UserImage(
                userId: userId,
                originalFileName: originalFileName,
                storedFileName: storedFileName,
                storedPath: fullPath,
                sizeInBytes: fileSize, // input size, TO-DO: consider actual saved size
                format: ImageFormat.Png,
                createdAt: DateTime.UtcNow
            );

            // save metadata to database
            _context.UserImages.Add(userImage);
            await _context.SaveChangesAsync();

            return userImage;
        }
    }
}
