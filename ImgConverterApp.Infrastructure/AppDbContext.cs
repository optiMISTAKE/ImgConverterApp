using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImgConverterApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace ImgConverterApp.Infrastructure
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet for UserImage entities
        public DbSet<UserImage> UserImages { get; set; }

        // configure the model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.OriginalFileName).IsRequired();
                entity.Property(e => e.StoredFileName).IsRequired();
                entity.Property(e => e.StoredPath).IsRequired();
                entity.Property(e => e.Format).IsRequired();
                entity.Property(e => e.SizeInBytes).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                // we ignore ExpiresAt as it's a computed property
                entity.Ignore(e => e.ExpiresAt);
            });
            // call the base method
            base.OnModelCreating(modelBuilder);
        }
    }
}
