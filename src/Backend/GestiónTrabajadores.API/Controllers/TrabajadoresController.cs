using FluentValidation;
using GestiónTrabajadores.Application.DTOs;
using GestiónTrabajadores.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestiónTrabajadores.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrabajadoresController : ControllerBase
{
    private readonly ITrabajadorService _trabajadorService;
    private readonly ILogger<TrabajadoresController> _logger;

    public TrabajadoresController(
        ITrabajadorService trabajadorService,
        ILogger<TrabajadoresController> logger)
    {
        _trabajadorService = trabajadorService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TrabajadorDto>>> GetAll()
    {
        try
        {
            var trabajadores = await _trabajadorService.GetAllAsync();
            return Ok(new
            {
                success = true,
                data = trabajadores,
                total = trabajadores.Count()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los trabajadores");
            return StatusCode(500, new
            {
                success = false,
                message = "Error al obtener los trabajadores",
                error = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TrabajadorDto>> GetById(int id)
    {
        try
        {
            var trabajador = await _trabajadorService.GetByIdAsync(id);

            if (trabajador == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No se encontró el trabajador con ID {id}"
                });
            }

            return Ok(new
            {
                success = true,
                data = trabajador
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el trabajador con ID {Id}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "Error al obtener el trabajador",
                error = ex.Message
            });
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TrabajadorDto>> Create([FromBody] CreateTrabajadorDto createDto)
    {
        try
        {
            var trabajador = await _trabajadorService.CreateAsync(createDto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = trabajador.IdTrabajador },
                new
                {
                    success = true,
                    message = "Trabajador creado exitosamente",
                    data = trabajador
                });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Error de validación",
                errors = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el trabajador");
            return StatusCode(500, new
            {
                success = false,
                message = "Error al crear el trabajador",
                error = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TrabajadorDto>> Update(int id, [FromBody] UpdateTrabajadorDto updateDto)
    {
        try
        {
            if (id != updateDto.IdTrabajador)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "El ID de la URL no coincide con el ID del cuerpo de la solicitud"
                });
            }

            var trabajador = await _trabajadorService.UpdateAsync(updateDto);

            return Ok(new
            {
                success = true,
                message = "Trabajador actualizado exitosamente",
                data = trabajador
            });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = "Error de validación",
                errors = ex.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el trabajador con ID {Id}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "Error al actualizar el trabajador",
                error = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _trabajadorService.DeleteAsync(id);

            return Ok(new
            {
                success = true,
                message = "Trabajador eliminado exitosamente"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el trabajador con ID {Id}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "Error al eliminar el trabajador",
                error = ex.Message
            });
        }
    }
}