namespace GestiónTrabajadores.Application.DTOs;

public class UpdateTrabajadorDto
{
    public int IdTrabajador { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public char Sexo { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string? Foto { get; set; }
    public string? Direccion { get; set; }
}