using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgConverterApp.Application.Images
{
    // data transfer model for storing the data regarding the image
    // user has uploaded
    public class UserImageDto
    {
        public Guid Id { get; set; }
        public string OriginalFileName { get; set; }
        public string Format { get; set; }
        public long SizeInBytes { get; set; }
        public long ConvertedSizeInBytes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
