using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgConverterApp.Application.Images
{
    public class ImageResponseDto
    {
        public Guid Id { get; set; }
        public string OriginalFileName { get; set; }
        public string StoredName { get; set; }
        public string DownloadUrl { get; set; }

    }
}
