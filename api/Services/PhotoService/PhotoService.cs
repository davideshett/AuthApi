using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace api.Services.PhotoService
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly DataContext _context;

        public PhotoService(IOptions<CloudinaryConfig> config, DataContext context)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
            _context = context;
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var fileStream = file.OpenReadStream();
            var fileName = file.FileName;
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = "Auth-Api"
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult;
        }
        public async Task<DeletionResult> DeletePhotoAsync(string publicUrl)
        {
            var publicId = publicUrl.Split('/').Last().Split('.')[0];
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}