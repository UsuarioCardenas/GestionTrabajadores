using GestiónTrabajadores.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestiónTrabajadores.Infrastructure.Data.Configurations;

public class TrabajadorConfiguration : IEntityTypeConfiguration<Trabajador>
{
    public void Configure(EntityTypeBuilder<Trabajador> builder)
    {
        builder.ToTable("Trabajadores");

        builder.HasKey(t => t.IdTrabajador);

        builder.Property(t => t.IdTrabajador)
            .HasColumnName("IdTrabajador")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.Nombres)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Apellidos)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.TipoDocumento)
            .IsRequired()
            .HasMaxLength(35);

        builder.Property(t => t.NumeroDocumento)
            .IsRequired()
            .HasMaxLength(35);

        builder.HasIndex(t => t.NumeroDocumento)
            .IsUnique()
            .HasDatabaseName("IX_Trabajadores_NumeroDocumento");

        builder.Property(t => t.Sexo)
            .IsRequired()
            .HasMaxLength(1)
            .HasColumnType("CHAR(1)");

        builder.Property(t => t.FechaNacimiento)
            .IsRequired()
            .HasColumnType("DATE");

        builder.Property(t => t.Foto)
            .HasMaxLength(500);

        builder.Property(t => t.Direccion)
            .IsRequired()
            .HasMaxLength(175);

        builder.Ignore(t => t.NombreCompleto);
    }
}