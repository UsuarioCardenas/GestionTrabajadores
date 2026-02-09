using GestiónTrabajadores.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestiónTrabajadores.Infrastructure.Data;

public class TrabajadoresDbContext : DbContext
{
    public TrabajadoresDbContext(DbContextOptions<TrabajadoresDbContext> options)
        : base(options)
    {
    }

    public DbSet<Trabajador> Trabajadores { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrabajadoresDbContext).Assembly);
    }
}
