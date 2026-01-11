using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ImgConverterApp.Domain.Entities
{
    public class AppUser: IdentityUser
    {
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // navigation property for related UserImages
        public ICollection<UserImage> UserImages { get; private set; } = new List<UserImage>();
    }
}
