using AutoMapper;
using FluentValidation;
using GestiónTrabajadores.Application.DTOs;
using GestiónTrabajadores.Application.Interfaces;
using GestiónTrabajadores.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace GestiónTrabajadores.Application.Services;

public class TrabajadorService : ITrabajadorService
{
    private readonly ITrabajadorRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateTrabajadorDto> _createValidator;
    private readonly IValidator<UpdateTrabajadorDto> _updateValidator;

    public TrabajadorService(
        ITrabajadorRepository repository,
        IMapper mapper,
        IValidator<CreateTrabajadorDto> createValidator,
        IValidator<UpdateTrabajadorDto> updateValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<TrabajadorDto>> GetAllAsync()
    {
        var trabajadores = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<TrabajadorDto>>(trabajadores);
    }

    public async Task<TrabajadorDto?> GetByIdAsync(int id)
    {
        var trabajador = await _repository.GetByIdAsync(id);
        return trabajador != null ? _mapper.Map<TrabajadorDto>(trabajador) : null;
    }

    public async Task<TrabajadorDto> CreateAsync(CreateTrabajadorDto createDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new System.ComponentModel.DataAnnotations.ValidationException(errors);
        }

        if (await _repository.ExistsByDocumentoAsync(createDto.NumeroDocumento))
            throw new InvalidOperationException("Ya existe un trabajador con ese número de documento");

        var trabajador = _mapper.Map<Trabajador>(createDto);
        var created = await _repository.CreateAsync(trabajador);

        return _mapper.Map<TrabajadorDto>(created);
    }

    public async Task<TrabajadorDto> UpdateAsync(UpdateTrabajadorDto updateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new System.ComponentModel.DataAnnotations.ValidationException(errors);
        }

        var existing = await _repository.GetByIdAsync(updateDto.IdTrabajador);
        if (existing == null)
            throw new KeyNotFoundException($"No se encontró el trabajador con ID {updateDto.IdTrabajador}");

        if (await _repository.ExistsByDocumentoAsync(updateDto.NumeroDocumento, updateDto.IdTrabajador))
            throw new InvalidOperationException("Ya existe otro trabajador con ese número de documento");

        _mapper.Map(updateDto, existing);
        var updated = await _repository.UpdateAsync(existing);

        return _mapper.Map<TrabajadorDto>(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
            throw new KeyNotFoundException($"No se encontró el trabajador con ID {id}");

        return await _repository.DeleteAsync(id);
    }
}