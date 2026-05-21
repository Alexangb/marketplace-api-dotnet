using Microsoft.AspNetCore.Http;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace MarketplaceApi.Shared.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary? _cloudinary;
        private readonly bool _isConfigured;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            if (!string.IsNullOrEmpty(cloudName) && !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(apiSecret))
            {
                var account = new Account(cloudName, apiKey, apiSecret);
                _cloudinary = new Cloudinary(account);
                _isConfigured = true;
            }
            else
            {
                _isConfigured = false;
                Console.WriteLine("⚠️ Cloudinary no configurado.");
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder = "perfiles")
        {
            if (!_isConfigured || _cloudinary == null)
                throw new Exception("Cloudinary no está configurado. Agrega las credenciales.");

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

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (!_isConfigured || _cloudinary == null) return true;
            if (string.IsNullOrEmpty(imageUrl)) return true;

            try
            {
                var publicId = ExtractPublicIdFromUrl(imageUrl);
                if (string.IsNullOrEmpty(publicId)) return true;

                var deletionParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deletionParams);
                return result.Result == "ok";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar imagen: {ex.Message}");
                return false;
            }
        }

        private string? ExtractPublicIdFromUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/');
                var fileName = segments.Last();
                return Path.GetFileNameWithoutExtension(fileName);
            }
            catch
            {
                return null;
            }
        }
    }
}