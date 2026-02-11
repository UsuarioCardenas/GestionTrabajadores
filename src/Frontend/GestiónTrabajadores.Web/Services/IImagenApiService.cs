namespace GestiónTrabajadores.Web.Services;

public interface IImagenApiService
{
    Task<string?> UploadImageAsync(Stream fileStream, string fileName, string contentType);
}