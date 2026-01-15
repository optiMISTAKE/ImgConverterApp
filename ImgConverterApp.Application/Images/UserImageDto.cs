using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgConverterApp.Application.Images
{
    public class UserImageDto
    {
        public Guid Id { get; set; }
        public string OriginalFileName { get; set; }
        public string Format { get; set; }
        public long SizeInBytes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
