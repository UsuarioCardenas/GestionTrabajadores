using GestiónTrabajadores.Application.DTOs;

namespace GestiónTrabajadores.Web.Services;

public interface ITrabajadorApiService
{
    Task<List<TrabajadorDto>> GetAllAsync();
    Task<TrabajadorDto?> GetByIdAsync(int id);
    Task<TrabajadorDto> CreateAsync(CreateTrabajadorDto createDto);
    Task<TrabajadorDto> UpdateAsync(int id, UpdateTrabajadorDto updateDto);
    Task<bool> DeleteAsync(int id);
}