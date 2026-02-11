using GestiónTrabajadores.Application.DTOs;

namespace GestiónTrabajadores.Web.Services;

public class TrabajadorApiService : ITrabajadorApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TrabajadorApiService> _logger;

    public TrabajadorApiService(HttpClient httpClient, ILogger<TrabajadorApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<TrabajadorDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<TrabajadorDto>>>("api/trabajadores");
            return response?.Data ?? new List<TrabajadorDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener trabajadores");
            return new List<TrabajadorDto>();
        }
    }

    public async Task<TrabajadorDto?> GetByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<TrabajadorDto>>($"api/trabajadores/{id}");
            return response?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener trabajador {Id}", id);
            return null;
        }
    }

    public async Task<TrabajadorDto> CreateAsync(CreateTrabajadorDto createDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/trabajadores", createDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error al crear trabajador: {StatusCode} - {Content}", response.StatusCode, errorContent);
                throw new HttpRequestException($"Error del servidor: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<TrabajadorDto>>();

            if (result?.Data == null)
            {
                throw new Exception("La respuesta del servidor no contiene datos válidos");
            }

            return result.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear trabajador");
            throw;
        }
    }

    public async Task<TrabajadorDto> UpdateAsync(int id, UpdateTrabajadorDto updateDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/trabajadores/{id}", updateDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error al actualizar trabajador: {StatusCode} - {Content}", response.StatusCode, errorContent);
                throw new HttpRequestException($"Error del servidor: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<TrabajadorDto>>();

            if (result?.Data == null)
            {
                throw new Exception("La respuesta del servidor no contiene datos válidos");
            }

            return result.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar trabajador {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/trabajadores/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar trabajador {Id}", id);
            return false;
        }
    }

    private class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; } = default!;
        public string? Message { get; set; }
    }
}