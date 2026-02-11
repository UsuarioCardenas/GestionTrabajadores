using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GestiónTrabajadores.Application.DTOs;
using GestiónTrabajadores.Application.Interfaces;
using GestiónTrabajadores.Application.Services;
using GestiónTrabajadores.Domain.Entities;
using Moq;
namespace GestiónTrabajadores.UnitTests.Services;

public class TrabajadorServiceTests
{
    private readonly Mock<ITrabajadorRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<CreateTrabajadorDto>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateTrabajadorDto>> _mockUpdateValidator;
    private readonly TrabajadorService _service;
    private readonly ITestOutputHelper _output;
    public TrabajadorServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _mockRepository = new Mock<ITrabajadorRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockCreateValidator = new Mock<IValidator<CreateTrabajadorDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateTrabajadorDto>>();

        _service = new TrabajadorService(
            _mockRepository.Object,
            _mockMapper.Object,
            _mockCreateValidator.Object,
            _mockUpdateValidator.Object
        );

        _output.WriteLine("=== Inicializando TrabajadorServiceTests ===");
    }
    [Fact]
    public async Task GetAllAsync_DebeRetornarListaDeTrabajadores()
    {
        _output.WriteLine("TEST: GetAllAsync_DebeRetornarListaDeTrabajadores");
        _output.WriteLine("Preparando datos de prueba...");
        var trabajadores = new List<Trabajador>
        {
            new() { IdTrabajador = 1, Nombres = "Juan", Apellidos = "Perez", Sexo = 'M' },
            new() { IdTrabajador = 2, Nombres = "Maria", Apellidos = "Lopez", Sexo = 'F' }
        };

        var trabajadoresDto = new List<TrabajadorDto>
        {
            new() { IdTrabajador = 1, Nombres = "Juan", Apellidos = "Perez", Sexo = 'M' },
            new() { IdTrabajador = 2, Nombres = "Maria", Apellidos = "Lopez", Sexo = 'F' }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(trabajadores);
        _mockMapper.Setup(m => m.Map<IEnumerable<TrabajadorDto>>(trabajadores)).Returns(trabajadoresDto);
        _output.WriteLine("Ejecutando GetAllAsync...");
        var result = await _service.GetAllAsync();
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _output.WriteLine($"RESULTADO: Se obtuvieron {result.Count()} trabajadores");
        _output.WriteLine("TEST PASSED ✓");
    }
    [Fact]
    public async Task GetAllAsync_SinTrabajadores_DebeRetornarListaVacia()
    {
        _output.WriteLine("TEST: GetAllAsync_SinTrabajadores_DebeRetornarListaVacia");
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Trabajador>());
        _mockMapper.Setup(m => m.Map<IEnumerable<TrabajadorDto>>(It.IsAny<IEnumerable<Trabajador>>()))
            .Returns(new List<TrabajadorDto>());
        var result = await _service.GetAllAsync();
        result.Should().BeEmpty();
        _output.WriteLine("RESULTADO: Lista vacía retornada correctamente");
        _output.WriteLine("TEST PASSED ✓");
    }
    [Fact]
    public async Task GetByIdAsync_ConIdValido_DebeRetornarTrabajador()
    {
        _output.WriteLine("TEST: GetByIdAsync_ConIdValido_DebeRetornarTrabajador");
        _output.WriteLine("Buscando trabajador con ID: 1");
        var trabajador = new Trabajador { IdTrabajador = 1, Nombres = "Juan", Apellidos = "Perez" };
        var trabajadorDto = new TrabajadorDto { IdTrabajador = 1, Nombres = "Juan", Apellidos = "Perez" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(trabajador);
        _mockMapper.Setup(m => m.Map<TrabajadorDto>(trabajador)).Returns(trabajadorDto);
        var result = await _service.GetByIdAsync(1);
        result.Should().NotBeNull();
        result!.IdTrabajador.Should().Be(1);
        _output.WriteLine($"RESULTADO: Trabajador encontrado - {result.Nombres} {result.Apellidos}");
        _output.WriteLine("TEST PASSED ✓");
    }
    [Fact]
    public async Task GetByIdAsync_ConIdInexistente_DebeRetornarNull()
    {
        _output.WriteLine("TEST: GetByIdAsync_ConIdInexistente_DebeRetornarNull");
        _output.WriteLine("Buscando trabajador con ID inexistente: 999");
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Trabajador?)null);
        var result = await _service.GetByIdAsync(999);
        result.Should().BeNull();
        _output.WriteLine("RESULTADO: Null retornado correctamente (trabajador no existe)");
        _output.WriteLine("TEST PASSED ✓");
    }
    [Fact]
    public async Task CreateAsync_ConDatosValidos_DebeCrearTrabajador()
    {
        _output.WriteLine("TEST: CreateAsync_ConDatosValidos_DebeCrearTrabajador");
        _output.WriteLine("Creando nuevo trabajador...");
        var createDto = new CreateTrabajadorDto
        {
            Nombres = "Juan",
            Apellidos = "Perez",
            TipoDocumento = "DNI",
            NumeroDocumento = "12345678",
            Sexo = 'M',
            FechaNacimiento = new DateTime(1990, 1, 1),
            Direccion = "Calle 123"
        };
        var trabajador = new Trabajador { IdTrabajador = 1, Nombres = "Juan", Apellidos = "Perez" };
        var trabajadorDto = new TrabajadorDto { IdTrabajador = 1, Nombres = "Juan", Apellidos = "Perez" };

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(new ValidationResult());
        _mockRepository.Setup(r => r.ExistsByDocumentoAsync(createDto.NumeroDocumento, null))
            .ReturnsAsync(false);
        _mockMapper.Setup(m => m.Map<Trabajador>(createDto)).Returns(trabajador);
        _mockRepository.Setup(r => r.CreateAsync(trabajador)).ReturnsAsync(trabajador);
        _mockMapper.Setup(m => m.Map<TrabajadorDto>(trabajador)).Returns(trabajadorDto);
        var result = await _service.CreateAsync(createDto);
        result.Should().NotBeNull();
        result.IdTrabajador.Should().Be(1);
        _output.WriteLine($"RESULTADO: Trabajador creado con ID: {result.IdTrabajador}");
        _output.WriteLine("TEST PASSED ✓");
    }
    [Fact]
    public async Task CreateAsync_ConDocumentoDuplicado_DebeLanzarExcepcion()
    {
        _output.WriteLine("TEST: CreateAsync_ConDocumentoDuplicado_DebeLanzarExcepcion");
        _output.WriteLine("Intentando crear trabajador con documento duplicado...");
        var createDto = new CreateTrabajadorDto
        {
            Nombres = "Juan",
            Apellidos = "Perez",
            TipoDocumento = "DNI",
            NumeroDocumento = "12345678",
            Sexo = 'M',
            FechaNacimiento = new DateTime(1990, 1, 1),
            Direccion = "Calle 123"
        };
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(new ValidationResult());
        _mockRepository.Setup(r => r.ExistsByDocumentoAsync(createDto.NumeroDocumento, null))
            .ReturnsAsync(true);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));

        _output.WriteLine("RESULTADO: InvalidOperationException lanzada correctamente");
        _output.WriteLine("TEST PASSED ✓");
    }
    [Fact]
    public async Task DeleteAsync_ConIdValido_DebeEliminar()
    {
        _output.WriteLine("TEST: DeleteAsync_ConIdValido_DebeEliminar");
        _output.WriteLine("Eliminando trabajador con ID: 1");
        _mockRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);
        var result = await _service.DeleteAsync(1);
        result.Should().BeTrue();
        _output.WriteLine("RESULTADO: Trabajador eliminado exitosamente");
        _output.WriteLine("TEST PASSED ✓");
    }
    [Fact]
    public async Task DeleteAsync_ConIdInexistente_DebeLanzarExcepcion()
    {
        _output.WriteLine("TEST: DeleteAsync_ConIdInexistente_DebeLanzarExcepcion");
        _output.WriteLine("Intentando eliminar trabajador inexistente con ID: 999");
        _mockRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(999));
        _output.WriteLine("RESULTADO: KeyNotFoundException lanzada correctamente");
        _output.WriteLine("TEST PASSED ✓");
    }
}