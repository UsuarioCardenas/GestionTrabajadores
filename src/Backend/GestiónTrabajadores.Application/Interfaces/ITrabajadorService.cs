using GestiónTrabajadores.Application.DTOs;

namespace GestiónTrabajadores.Application.Interfaces;

public interface ITrabajadorService
{
    Task<IEnumerable<TrabajadorDto>> GetAllAsync();
    Task<TrabajadorDto?> GetByIdAsync(int id);
    Task<TrabajadorDto> CreateAsync(CreateTrabajadorDto createDto);
    Task<TrabajadorDto> UpdateAsync(UpdateTrabajadorDto updateDto);
    Task<bool> DeleteAsync(int id);
}