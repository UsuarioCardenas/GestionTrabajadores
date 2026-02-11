using FluentAssertions;
using GestiónTrabajadores.API.Controllers;
using GestiónTrabajadores.Application.DTOs;
using GestiónTrabajadores.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
namespace GestiónTrabajadores.UnitTests.Controllers;

public class TrabajadoresControllerTests
{
    private readonly Mock<ITrabajadorService> _mockService;
    private readonly Mock<ILogger<TrabajadoresController>> _mockLogger;
    private readonly TrabajadoresController _controller;
    private readonly Xunit.ITestOutputHelper _output;
    public TrabajadoresControllerTests(Xunit.ITestOutputHelper output)
    {
        _output = output;
        _mockService = new Mock<ITrabajadorService>();
        _mockLogger = new Mock<ILogger<TrabajadoresController>>();
        _controller = new TrabajadoresController(_mockService.Object, _mockLogger.Object);

        _output.WriteLine("=== Inicializando TrabajadoresControllerTests ===");
    }
    [Fact]
    public async Task GetAll_DebeRetornarOk()
    {
        _output.WriteLine("TEST: GetAll_DebeRetornarOk");
        _output.WriteLine("Ejecutando GET /api/trabajadores");
        var trabajadores = new List<TrabajadorDto>
        {
            new() { IdTrabajador = 1, Nombres = "Juan", Apellidos = "Perez" }
        };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(trabajadores);
        var result = await _controller.GetAll();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        _output.WriteLine($"RESULTADO: HTTP 200 OK - {trabajadores.Count} trabajadores");
        _output.WriteLine("TEST PASSED ✓");
    }
    [Fact]
    public async Task GetById_ConIdValido_DebeRetornarOk()
    {
        _output.WriteLine("TEST: GetById_ConIdValido_DebeRetornarOk");
        _output.WriteLine("Ejecutando GET /api/trabajadores/1");
        var trabajador = new TrabajadorDto { IdTrabajador = 1, Nombres = "Juan" };
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(trabajador);
        var result = await _controller.GetById(1);
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        _output.WriteLine($"RESULTADO: HTTP 200 OK - Trabajador: {trabajador.Nombres}");
        _output.WriteLine("TEST PASSED ✓");
    }

    [Fact]
    public async Task GetById_ConIdInexistente_DebeRetornar404()
    {
        _output.WriteLine("TEST: GetById_ConIdInexistente_DebeRetornar404");
        _output.WriteLine("Ejecutando GET /api/trabajadores/999");
        _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((TrabajadorDto?)null);
        var result = await _controller.GetById(999);
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);
        _output.WriteLine("RESULTADO: HTTP 404 Not Found");
        _output.WriteLine("TEST PASSED ✓");
    }
    [Fact]
    public async Task Delete_ConIdValido_DebeRetornarOk()
    {
        _output.WriteLine("TEST: Delete_ConIdValido_DebeRetornarOk");
        _output.WriteLine("Ejecutando DELETE /api/trabajadores/1");
        _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
        var result = await _controller.Delete(1);
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        _output.WriteLine("RESULTADO: HTTP 200 OK - Trabajador eliminado");
        _output.WriteLine("TEST PASSED ✓");
    }

    [Fact]
    public async Task Delete_ConIdInexistente_DebeRetornar404()
    {
        _output.WriteLine("TEST: Delete_ConIdInexistente_DebeRetornar404");
        _output.WriteLine("Ejecutando DELETE /api/trabajadores/999");
        _mockService.Setup(s => s.DeleteAsync(999)).ThrowsAsync(new KeyNotFoundException());
        var result = await _controller.Delete(999);
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);
        _output.WriteLine("RESULTADO: HTTP 404 Not Found");
        _output.WriteLine("TEST PASSED ✓");
    }
}
