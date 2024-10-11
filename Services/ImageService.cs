using Entities.Models;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Services.Contracts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ImageService : IImageService
    {
        private readonly IBackgroundJobClient hfClient;
        private readonly IWebHostEnvironment env;
        private readonly UserManager<User> userManager;

        public ImageService(IBackgroundJobClient hfClient, IWebHostEnvironment env, UserManager<User> userManager)
        {
            this.hfClient = hfClient;
            this.env = env;
            this.userManager = userManager;
        }

        public async Task ProcessImages(string UserId, List<IFormFile> files)
        {
            var user = await userManager.FindByEmailAsync(UserId);
            string HIResPath = Path.Combine(env.WebRootPath, "HIResImages");

            var imageId = Guid.NewGuid();

            foreach (var file in files)
            {
                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                long fileSize;
                var storageName = $"{Path.GetFileNameWithoutExtension(file.FileName)}-{imageId.ToString().Substring(0, 8)}{Path.GetExtension(file.FileName)}";
                using (var fileStream = File.Create(Path.Combine(HIResPath, storageName)))
                {
                    using (var stream = file.OpenReadStream())
                    {
                        await stream.CopyToAsync(fileStream);
                        stream.Seek(0, SeekOrigin.Begin);

                        fileSize = fileStream.Length;
                        GenerateThumbnail(stream, storageName);
                    }
                }
            }
        }

        private string GenerateThumbnail(Stream stream, string storageName)
        {
            string ThumbPath = Path.Combine(env.WebRootPath, "ThumbnailImages", storageName);

            using (var image = SixLabors.ImageSharp.Image.Load(stream))
            {
                var exif = GetExifMetadata(image.Metadata.ExifProfile);

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(96, 0),
                    Mode = ResizeMode.Max
                }));

                image.Save(ThumbPath);
            }

            return ThumbPath;
        }

        private ExifMetadata GetExifMetadata(ExifProfile exif)
        {
            var metadata = new ExifMetadata();

            metadata.Manufacturer = exif.TryGetValue(ExifTag.Make, out var maker) ? maker.Value : null;
            metadata.Model = exif.TryGetValue(ExifTag.Model, out var model) ? model.Value : null;
            metadata.DateTaken = exif.TryGetValue(ExifTag.DateTimeOriginal, out var date) ? DateTime.ParseExact(date.Value!, "yyyy:MM:dd hh:mm:ss", null) : null;

            return metadata;
        }
    }
}
