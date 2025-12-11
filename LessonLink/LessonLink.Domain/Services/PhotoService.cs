using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LessonLink.BusinessLogic.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LessonLink.BusinessLogic.Services;

public class PhotoService
{
    private readonly Cloudinary? _cloudinary;
    private readonly bool _isConfigured;

    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var settings = config.Value;

        // Only initialize Cloudinary if all required settings are provided
        if (!string.IsNullOrEmpty(settings.CloudName) &&
            !string.IsNullOrEmpty(settings.ApiKey) &&
            !string.IsNullOrEmpty(settings.ApiSecret))
        {
            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
            _isConfigured = true;
        }
        else
        {
            _isConfigured = false;
        }
    }

    public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file)
    {
        if (!_isConfigured || _cloudinary == null)
        {
            throw new InvalidOperationException("A Cloudinary nincs konfigurálva. Adja meg a CloudName, ApiKey és ApiSecret értékeket az appsettings.json fájlban, hogy használhassa a képfeltöltés funkciót!");
        }

        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "LessonLink"
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }
}
