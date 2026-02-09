using GestiónTrabajadores.Domain.Entities;

namespace GestiónTrabajadores.Application.Interfaces;

public interface ITrabajadorRepository
{
    Task<IEnumerable<Trabajador>> GetAllAsync();
    Task<Trabajador?> GetByIdAsync(int id);
    Task<Trabajador> CreateAsync(Trabajador trabajador);
    Task<Trabajador> UpdateAsync(Trabajador trabajador);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByDocumentoAsync(string numeroDocumento, int? excludeId = null);
}