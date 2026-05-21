using Microsoft.AspNetCore.Http;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace MarketplaceApi.Shared.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder = "perfiles")
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                Transformation = new Transformation().Width(500).Height(500).Crop("fill").Quality("auto")
            };
            
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);
            return result.Result == "ok";
        }

        public string GetPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return Guid.NewGuid().ToString();
            
            // Extraer el public_id de la URL de Cloudinary
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/');
            var fileName = segments.Last();
            return Path.GetFileNameWithoutExtension(fileName);
        }
    }
}