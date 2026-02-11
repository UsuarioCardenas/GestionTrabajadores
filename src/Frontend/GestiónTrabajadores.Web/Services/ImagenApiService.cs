using System.Net.Http.Headers;

namespace GestiónTrabajadores.Web.Services;

public class ImagenApiService : IImagenApiService
{
    private readonly HttpClient _httpClient;

    public ImagenApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> UploadImageAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            if (fileStream.CanSeek)
            {
                fileStream.Position = 0;
            }

            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(25));

            var result = await Task.Run(async () =>
            {
                using var uploadStream = new MemoryStream(fileBytes);
                using var content = new MultipartFormDataContent();
                using var streamContent = new StreamContent(uploadStream);

                streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Add(streamContent, "file", fileName);

                var response = await _httpClient.PostAsync("api/imagenes/upload", content, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var uploadResult = await response.Content.ReadFromJsonAsync<ImageUploadResponse>(cts.Token);
                    return uploadResult?.Url;
                }
                else
                {
                    return null;
                }
            }, cts.Token);

            return result;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}

public class ImageUploadResponse
{
    public string Url { get; set; } = string.Empty;
}