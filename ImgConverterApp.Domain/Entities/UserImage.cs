using ImgConverterApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgConverterApp.Domain.Entities
{
    public class UserImage
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string UserId { get; private set; }
        public string OriginalFileName { get; private set; }
        public string StoredFileName { get; private set; }
        public string StoredPath { get; private set; }
        public ImageFormat Format { get; private set; }
        public long SizeInBytes { get; private set; }
        public long ConvertedSizeInBytes { get; private set; }
        
        // date and time when the image was created
        public DateTime CreatedAt { get; private set; }
        // after 30 days the image will be deleted
        public DateTime ExpiresAt => CreatedAt.AddDays(30);

        // constructor
        public UserImage(string userId, string originalFileName, string storedFileName,
            string storedPath, long sizeInBytes, long convertedSizeInBytes, ImageFormat format, DateTime createdAt)
        {
            // Simple validation (Domain Guardrails)
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required");
            if (sizeInBytes <= 0) throw new ArgumentException("File cannot be empty");

            UserId = userId;
            OriginalFileName = originalFileName;
            StoredFileName = storedFileName;
            StoredPath = storedPath;
            SizeInBytes = sizeInBytes;
            ConvertedSizeInBytes = convertedSizeInBytes;
            Format = format;
            CreatedAt = createdAt;
        }
    }
}
