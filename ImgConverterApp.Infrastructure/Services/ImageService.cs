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
using ImgConverterApp.Application.Images;
using Microsoft.EntityFrameworkCore;

namespace ImgConverterApp.Infrastructure.Services
{
    public class ImageService : IImageService
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

            var userImageCount = await _context.UserImages.CountAsync(x => x.UserId == userId);

            if (userImageCount >= 10)
            {
                // find the oldest image that exceeds the limit
                var oldestImages = await _context.UserImages
                    .Where(x => x.UserId == userId)
                    .OrderBy(x => x.CreatedAt)
                    .Take(userImageCount - 9) // select excess images
                    .ToListAsync();

                foreach (var img in oldestImages)
                {
                    // delete physical file
                    if (File.Exists(img.StoredPath)) File.Delete(img.StoredPath);
                    _context.UserImages.Remove(img);
                }

            }

            // save metadata to database
            _context.UserImages.Add(userImage);
            await _context.SaveChangesAsync();

            return userImage;
        }

        public async Task<FileResponseDto> GetImageAsync(Guid imageId, string userId)
        {
            // find the image in the DB
            var userImage = await _context.UserImages.FirstOrDefaultAsync(x => x.Id == imageId);

            if (userImage == null)
            {
                throw new FileNotFoundException("Image not found in databse.");
            }

            // security check - does the image belong to the user?
            if (userImage.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to access this file.");
            }

            // verifying the physical file exists
            if (!File.Exists(userImage.StoredPath))
            {
                throw new FileNotFoundException("Physical file is missing.");
            }

            // open the file stream
            // FileShare.Read is used to allow multiple people to download at once if needed
            var fileStream = new FileStream(userImage.StoredPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return new FileResponseDto
            {
                Stream = fileStream,
                ContentType = "image/png", // at least for now we only convert to PNG
                FileName = userImage.OriginalFileName
            };
        }

        public async Task<List<UserImageDto>> GetHistoryAsync(string userId)
        {
            var images = await _context.UserImages
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new UserImageDto
                {
                    Id = x.Id,
                    OriginalFileName = x.OriginalFileName,
                    Format = x.Format.ToString(),
                    SizeInBytes = x.SizeInBytes,
                    CreatedAt = x.CreatedAt
                }).Take(10) // return last 10 images
                .ToListAsync();
            return images;
        }

        // one or multiple images deletion
        public async Task DeleteImagesAsync(List<Guid> imageIds, string userId)
        {
            var imagesToDelete = await _context.UserImages
                .Where(x => imageIds.Contains(x.Id) && x.UserId == userId)
                .ToListAsync();

            foreach (var img in imagesToDelete)
            {
                // delete physical file
                if (File.Exists(img.StoredPath)) File.Delete(img.StoredPath);
            }

            // remove from database
            _context.UserImages.RemoveRange(imagesToDelete);
            await _context.SaveChangesAsync();
        }

        // delete all images for a user
        public async Task DeleteAllImagesAsync(string userId)
        {
            var imagesToDelete = await _context.UserImages
                .Where(x => x.UserId == userId)
                .ToListAsync();

            foreach (var img in imagesToDelete)
            {
                // delete all physical files
                if (File.Exists(img.StoredPath)) File.Delete(img.StoredPath);
            }

            // remove from database
            _context.UserImages.RemoveRange(imagesToDelete);
            await _context.SaveChangesAsync();
        }
    }
}
