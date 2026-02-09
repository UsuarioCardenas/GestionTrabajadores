using FluentValidation;
using GestiónTrabajadores.Application.DTOs;

namespace GestiónTrabajadores.Application.Validators;

public class CreateTrabajadorDtoValidator : AbstractValidator<CreateTrabajadorDto>
{
    public CreateTrabajadorDtoValidator()
    {
        RuleFor(x => x.Nombres)
            .NotEmpty().WithMessage("Los nombres son obligatorios")
            .MaximumLength(50).WithMessage("Los nombres no pueden exceder 50 caracteres");

        RuleFor(x => x.Apellidos)
            .NotEmpty().WithMessage("Los apellidos son obligatorios")
            .MaximumLength(50).WithMessage("Los apellidos no pueden exceder 50 caracteres");

        RuleFor(x => x.TipoDocumento)
            .NotEmpty().WithMessage("El tipo de documento es obligatorio")
            .MaximumLength(35).WithMessage("El tipo de documento no puede exceder 35 caracteres");

        RuleFor(x => x.NumeroDocumento)
            .NotEmpty().WithMessage("El número de documento es obligatorio")
            .MaximumLength(35).WithMessage("El número de documento no puede exceder 35 caracteres")
            .Matches(@"^[0-9]+$").WithMessage("El número de documento solo puede contener números");

        RuleFor(x => x.Sexo)
            .Must(s => s == 'M' || s == 'F')
            .WithMessage("El sexo debe ser 'M' (Masculino) o 'F' (Femenino)");

        RuleFor(x => x.FechaNacimiento)
            .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria")
            .LessThan(DateTime.Now).WithMessage("La fecha de nacimiento debe ser anterior a hoy")
            .GreaterThan(DateTime.Now.AddYears(-100)).WithMessage("La fecha de nacimiento no puede ser mayor a 100 años");

        RuleFor(x => x.Foto)
            .MaximumLength(500).WithMessage("La URL de la foto no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Foto));

        RuleFor(x => x.Direccion)
            .NotEmpty().WithMessage("La dirección es obligatoria")
            .MaximumLength(175).WithMessage("La dirección no puede exceder 175 caracteres");
    }
}