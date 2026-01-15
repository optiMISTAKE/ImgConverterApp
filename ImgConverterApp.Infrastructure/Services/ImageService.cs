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
using Microsoft.Extensions.DependencyInjection;

namespace ImgConverterApp.Infrastructure.Services
{
    public class ImageService: IImageService
    {
        private readonly AppDbContext _context;
        private readonly IServiceScopeFactory _scopeFactory;
        // folder where images will be stored
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "ImageStorage");

        public ImageService(AppDbContext context, IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _scopeFactory = scopeFactory;
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
            var newName = originalFileName.Replace(".webp", "");
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
                originalFileName: newName,
                storedFileName: storedFileName,
                storedPath: fullPath,
                sizeInBytes: fileSize, // input size, TO-DO: consider actual saved size
                format: ImageFormat.Png,
                createdAt: DateTime.UtcNow
            );

            // save metadata to database
            _context.UserImages.Add(userImage);
            await _context.SaveChangesAsync();

            // check and cleanup old images (async, don't await if you want it even faster)
            _ = Task.Run(async () =>
            {
                // Create a NEW scope for the background thread
                using (var scope = _scopeFactory.CreateScope())
                {
                    // Resolve a NEW instance of the DbContext
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    try
                    {
                        var userImageCount = await db.UserImages.CountAsync(x => x.UserId == userId);

                        if (userImageCount >= 10)
                        {
                            var oldestImages = await db.UserImages
                                .Where(x => x.UserId == userId)
                                .OrderBy(x => x.CreatedAt)
                                .Take(userImageCount - 10)
                                .ToListAsync();

                            foreach (var oldImg in oldestImages)
                            {
                                if (File.Exists(oldImg.StoredPath))
                                    File.Delete(oldImg.StoredPath);
                            }

                            db.UserImages.RemoveRange(oldestImages); // Use RemoveRange for efficiency
                            await db.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Background cleanup failed: {ex.Message}");
                    }
                }
            });

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

        // we'll always return 10 images
        public async Task<List<UserImageDto>> GetHistoryAsync(string userId)
        {
            return await _context.UserImages
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new UserImageDto
                {
                    Id = x.Id,
                    OriginalFileName = x.OriginalFileName,
                    Format = x.Format.ToString(),
                    SizeInBytes = x.SizeInBytes,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();
        }

        public async Task DeleteImagesAsync(List<Guid> imageIds, string userId)
        {
            var images = await _context.UserImages
                .Where(x => imageIds.Contains(x.Id) && x.UserId == userId)
                .ToListAsync();

            foreach (var img in images)
            {
                if (File.Exists(img.StoredPath)) File.Delete(img.StoredPath);
            }

            _context.UserImages.RemoveRange(images);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAllImagesAsync(string userId)
        {
            var images = await _context.UserImages
                .Where(x => x.UserId == userId)
                .ToListAsync();

            foreach (var img in images)
            {
                if (File.Exists(img.StoredPath)) File.Delete(img.StoredPath);
            }

            _context.UserImages.RemoveRange(images);
            await _context.SaveChangesAsync();
        }
    }
}
