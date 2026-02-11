using GestiónTrabajadores.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestiónTrabajadores.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagenesController : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;

    public ImagenesController(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10 * 1024 * 1024)]
    public async Task<ActionResult<ImageUploadResponse>> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "No se ha proporcionado ningún archivo" });
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(new { error = "Solo se permiten archivos JPG, JPEG o PNG" });
        }

        if (file.Length > 5 * 1024 * 1024)
        {
            return BadRequest(new { error = "El archivo no puede superar los 5MB" });
        }

        try
        {
            using var stream = file.OpenReadStream();
            var imageUrl = await _cloudinaryService.UploadImageAsync(stream, file.FileName);
            return Ok(new ImageUploadResponse { Url = imageUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error al subir la imagen: {ex.Message}" });
        }
    }

    [HttpDelete("{publicId}")]
    public async Task<ActionResult> DeleteImage(string publicId)
    {
        try
        {
            var result = await _cloudinaryService.DeleteImageAsync(publicId);

            if (result)
            {
                return Ok(new { message = "Imagen eliminada correctamente" });
            }

            return NotFound(new { error = "No se encontró la imagen" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error al eliminar la imagen: {ex.Message}" });
        }
    }
}

public class ImageUploadResponse
{
    public string Url { get; set; } = string.Empty;
}