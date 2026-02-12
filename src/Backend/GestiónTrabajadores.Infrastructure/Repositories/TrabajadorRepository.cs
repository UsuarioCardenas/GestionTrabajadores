using GestiónTrabajadores.Application.Interfaces;
using GestiónTrabajadores.Domain.Entities;
using GestiónTrabajadores.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GestiónTrabajadores.Infrastructure.Repositories;

public class TrabajadorRepository : ITrabajadorRepository
{
    private readonly TrabajadoresDbContext _context;

    public TrabajadorRepository(TrabajadoresDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Trabajador>> GetAllAsync()
    {
        if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            return await _context.Trabajadores.ToListAsync();
        }
        var trabajadores = new List<Trabajador>();

        var connectionString = _context.Database.GetConnectionString();

        using (var connection = new SqlConnection(connectionString))
        {
            using (var command = new SqlCommand("USP_ObtenerTodosLosTrabajadores", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var trabajador = new Trabajador
                        {
                            IdTrabajador = reader.GetInt32(reader.GetOrdinal("IdTrabajador")),
                            Nombres = reader.GetString(reader.GetOrdinal("Nombres")),
                            Apellidos = reader.GetString(reader.GetOrdinal("Apellidos")),
                            TipoDocumento = reader.GetString(reader.GetOrdinal("TipoDocumento")),
                            NumeroDocumento = reader.GetString(reader.GetOrdinal("NumeroDocumento")),
                            Sexo = reader.GetString(reader.GetOrdinal("Sexo"))[0],
                            FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                            Foto = reader.IsDBNull(reader.GetOrdinal("Foto")) ? null : reader.GetString(reader.GetOrdinal("Foto")),
                            Direccion = reader.GetString(reader.GetOrdinal("Direccion"))
                        };

                        trabajadores.Add(trabajador);
                    }
                }
            }
        }

        return trabajadores;
    }

    public async Task<Trabajador?> GetByIdAsync(int id)
    {
        return await _context.Trabajadores
            .FirstOrDefaultAsync(t => t.IdTrabajador == id);
    }

    public async Task<Trabajador> CreateAsync(Trabajador trabajador)
    {
        _context.Trabajadores.Add(trabajador);
        await _context.SaveChangesAsync();
        return trabajador;
    }

    public async Task<Trabajador> UpdateAsync(Trabajador trabajador)
    {
        _context.Entry(trabajador).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return trabajador;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var trabajador = await GetByIdAsync(id);
        if (trabajador == null)
            return false;

        _context.Trabajadores.Remove(trabajador);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Trabajadores.AnyAsync(t => t.IdTrabajador == id);
    }

    public async Task<bool> ExistsByDocumentoAsync(string numeroDocumento, int? excludeId = null)
    {
        var query = _context.Trabajadores.Where(t => t.NumeroDocumento == numeroDocumento);

        if (excludeId.HasValue)
            query = query.Where(t => t.IdTrabajador != excludeId.Value);

        return await query.AnyAsync();
    }
}