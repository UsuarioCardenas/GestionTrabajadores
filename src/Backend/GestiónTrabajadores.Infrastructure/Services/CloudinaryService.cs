using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GestiónTrabajadores.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GestiónTrabajadores.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        var cloudName = configuration["Cloudinary:CloudName"];
        var apiKey = configuration["Cloudinary:ApiKey"];
        var apiSecret = configuration["Cloudinary:ApiSecret"];

        if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
        {
            throw new ArgumentException("Las credenciales de Cloudinary no están configuradas correctamente en appsettings.json");
        }

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = "trabajadores",
            Transformation = new Transformation()
                .Width(400)
                .Height(400)
                .Crop("fill")
                .Gravity("face")
        };

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        try
        {
            var uploadResult = await _cloudinary.UploadAsync(uploadParams, cts.Token);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Error al subir imagen a Cloudinary: {uploadResult.Error.Message}");
            }

            return uploadResult.SecureUrl.ToString();
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException("Cloudinary no respondió en el tiempo esperado.");
        }
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        if (string.IsNullOrEmpty(publicId))
            return false;

        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);

        return result.Result == "ok";
    }
}