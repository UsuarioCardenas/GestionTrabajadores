using FluentAssertions;
using GestiónTrabajadores.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace GestiónTrabajadores.IntegrationTests.Controllers;

public class TrabajadoresControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly Xunit.ITestOutputHelper _output;
    public TrabajadoresControllerIntegrationTests(
        CustomWebApplicationFactory<Program> factory,
        Xunit.ITestOutputHelper output)
    {
        _output = output;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        _output.WriteLine("=== Inicializando TrabajadoresControllerIntegrationTests ===");
        _output.WriteLine($"Base URL: {_client.BaseAddress}");
    }
    #region GET Tests
    [Fact]
    public async Task GetAll_DebeRetornar200OK()
    {
        _output.WriteLine("========================================");
        _output.WriteLine("TEST: GetAll_DebeRetornar200OK");
        _output.WriteLine("Ejecutando: GET /api/trabajadores");
        _output.WriteLine("========================================");
        var response = await _client.GetAsync("/api/trabajadores", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine($"Response: {content}");
        _output.WriteLine("✓ TEST PASSED");
    }
    [Fact]
    public async Task GetById_ConIdInexistente_DebeRetornar404()
    {
        _output.WriteLine("========================================");
        _output.WriteLine("TEST: GetById_ConIdInexistente_DebeRetornar404");
        _output.WriteLine("Ejecutando: GET /api/trabajadores/99999");
        _output.WriteLine("========================================");
        var response = await _client.GetAsync("/api/trabajadores/99999", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine($"Response: {content}");
        _output.WriteLine("✓ TEST PASSED");
    }
    #endregion
    #region POST Tests
    [Fact]
    public async Task Create_ConDatosValidos_DebeRetornar201Created()
    {
        _output.WriteLine("========================================");
        _output.WriteLine("TEST: Create_ConDatosValidos_DebeRetornar201Created");
        _output.WriteLine("Ejecutando: POST /api/trabajadores");
        _output.WriteLine("========================================");
        var nuevoTrabajador = new CreateTrabajadorDto
        {
            Nombres = "Test",
            Apellidos = "Usuario",
            TipoDocumento = "DNI",
            NumeroDocumento = GenerarDocumentoUnico(),
            Sexo = 'M',
            FechaNacimiento = new DateTime(1990, 1, 1),
            Direccion = "Calle Test 123"
        };
        _output.WriteLine($"Request Body: Nombres={nuevoTrabajador.Nombres}, Apellidos={nuevoTrabajador.Apellidos}");
        _output.WriteLine($"             Documento={nuevoTrabajador.NumeroDocumento}");
        var response = await _client.PostAsJsonAsync("/api/trabajadores", nuevoTrabajador, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine($"Response: {content}");
        _output.WriteLine("✓ TEST PASSED");
    }
    [Fact]
    public async Task Create_ConDatosInvalidos_DebeRetornar400BadRequest()
    {
        _output.WriteLine("========================================");
        _output.WriteLine("TEST: Create_ConDatosInvalidos_DebeRetornar400BadRequest");
        _output.WriteLine("Ejecutando: POST /api/trabajadores (datos inválidos)");
        _output.WriteLine("========================================");
        var trabajadorInvalido = new CreateTrabajadorDto
        {
            Nombres = "",
            Apellidos = "",
            TipoDocumento = "",
            NumeroDocumento = "",
            Sexo = ' ',
            FechaNacimiento = DateTime.MinValue,
            Direccion = ""
        };

        _output.WriteLine("Request Body: Datos vacíos/inválidos");
        var response = await _client.PostAsJsonAsync("/api/trabajadores", trabajadorInvalido, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine($"Response: {content}");
        _output.WriteLine("✓ TEST PASSED");
    }
    [Fact]
    public async Task Create_ConDocumentoDuplicado_DebeRetornar400BadRequest()
    {
        _output.WriteLine("========================================");
        _output.WriteLine("TEST: Create_ConDocumentoDuplicado_DebeRetornar400BadRequest");
        _output.WriteLine("Ejecutando: POST /api/trabajadores (documento duplicado)");
        _output.WriteLine("========================================");
        var documentoUnico = GenerarDocumentoUnico();

        var trabajador1 = new CreateTrabajadorDto
        {
            Nombres = "Primero",
            Apellidos = "Test",
            TipoDocumento = "DNI",
            NumeroDocumento = documentoUnico,
            Sexo = 'M',
            FechaNacimiento = new DateTime(1990, 1, 1),
            Direccion = "Calle 1"
        };

        var trabajador2 = new CreateTrabajadorDto
        {
            Nombres = "Segundo",
            Apellidos = "Test",
            TipoDocumento = "DNI",
            NumeroDocumento = documentoUnico,
            Sexo = 'F',
            FechaNacimiento = new DateTime(1992, 5, 15),
            Direccion = "Calle 2"
        };

        _output.WriteLine($"Creando primer trabajador con documento: {documentoUnico}");
        var response1 = await _client.PostAsJsonAsync("/api/trabajadores", trabajador1, TestContext.Current.CancellationToken);
        _output.WriteLine($"Primer POST - Status: {(int)response1.StatusCode} {response1.StatusCode}");
        _output.WriteLine($"Intentando crear segundo trabajador con mismo documento: {documentoUnico}");
        var response2 = await _client.PostAsJsonAsync("/api/trabajadores", trabajador2, TestContext.Current.CancellationToken);
        var content = await response2.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _output.WriteLine($"Segundo POST - Status Code: {(int)response2.StatusCode} {response2.StatusCode}");
        _output.WriteLine($"Response: {content}");
        _output.WriteLine("✓ TEST PASSED");
    }
    #endregion
    #region PUT Tests
    [Fact]
    public async Task Update_ConIdInexistente_DebeRetornar404()
    {
        _output.WriteLine("========================================");
        _output.WriteLine("TEST: Update_ConIdInexistente_DebeRetornar404");
        _output.WriteLine("Ejecutando: PUT /api/trabajadores/99999");
        _output.WriteLine("========================================");
        var updateDto = new UpdateTrabajadorDto
        {
            IdTrabajador = 99999,
            Nombres = "Test",
            Apellidos = "Update",
            TipoDocumento = "DNI",
            NumeroDocumento = GenerarDocumentoUnico(),
            Sexo = 'M',
            FechaNacimiento = new DateTime(1990, 1, 1),
            Direccion = "Calle Update"
        };
        _output.WriteLine($"Request Body: ID={updateDto.IdTrabajador} (inexistente)");
        var response = await _client.PutAsJsonAsync("/api/trabajadores/99999", updateDto, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine($"Response: {content}");
        _output.WriteLine("✓ TEST PASSED");
    }
    [Fact]
    public async Task Update_ConIdsDiferentes_DebeRetornar400()
    {
        _output.WriteLine("========================================");
        _output.WriteLine("TEST: Update_ConIdsDiferentes_DebeRetornar400");
        _output.WriteLine("Ejecutando: PUT /api/trabajadores/999 (con ID diferente en body)");
        _output.WriteLine("========================================");
        var updateDto = new UpdateTrabajadorDto
        {
            IdTrabajador = 1,
            Nombres = "Test",
            Apellidos = "Update",
            TipoDocumento = "DNI",
            NumeroDocumento = GenerarDocumentoUnico(),
            Sexo = 'M',
            FechaNacimiento = new DateTime(1990, 1, 1),
            Direccion = "Calle Update"
        };
        _output.WriteLine($"URL ID: 999, Body ID: {updateDto.IdTrabajador}");
        var response = await _client.PutAsJsonAsync("/api/trabajadores/999", updateDto, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine($"Response: {content}");
        _output.WriteLine("✓ TEST PASSED");
    }
    #endregion
    #region DELETE Tests
    [Fact]
    public async Task Delete_ConIdInexistente_DebeRetornar404()
    {
        _output.WriteLine("========================================");
        _output.WriteLine("TEST: Delete_ConIdInexistente_DebeRetornar404");
        _output.WriteLine("Ejecutando: DELETE /api/trabajadores/99999");
        _output.WriteLine("========================================");
        var response = await _client.DeleteAsync("/api/trabajadores/99999", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _output.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        _output.WriteLine($"Response: {content}");
        _output.WriteLine("✓ TEST PASSED");
    }
    #endregion
    #region CRUD Completo Test
    [Fact]
    public async Task CRUD_Completo_DebeCompletarTodoElCiclo()
    {
        _output.WriteLine("========================================");
        _output.WriteLine("TEST: CRUD_Completo_DebeCompletarTodoElCiclo");
        _output.WriteLine("Ejecutando ciclo completo: CREATE -> READ -> UPDATE -> DELETE");
        _output.WriteLine("========================================");
        var documentoUnico = GenerarDocumentoUnico();
        _output.WriteLine("\n--- PASO 1: CREATE ---");
        var createDto = new CreateTrabajadorDto
        {
            Nombres = "CRUD",
            Apellidos = "Test",
            TipoDocumento = "DNI",
            NumeroDocumento = documentoUnico,
            Sexo = 'M',
            FechaNacimiento = new DateTime(1990, 1, 1),
            Direccion = "Calle CRUD"
        };
        _output.WriteLine($"POST /api/trabajadores - Documento: {documentoUnico}");
        var createResponse = await _client.PostAsJsonAsync("/api/trabajadores", createDto, TestContext.Current.CancellationToken);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<TrabajadorDto>>(TestContext.Current.CancellationToken);
        var trabajadorId = createResult!.Data!.IdTrabajador;
        _output.WriteLine($"Status: {(int)createResponse.StatusCode} - Trabajador creado con ID: {trabajadorId}");
        _output.WriteLine("\n--- PASO 2: READ ---");
        _output.WriteLine($"GET /api/trabajadores/{trabajadorId}");
        var getResponse = await _client.GetAsync($"/api/trabajadores/{trabajadorId}", TestContext.Current.CancellationToken);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResponse<TrabajadorDto>>(TestContext.Current.CancellationToken);
        _output.WriteLine($"Status: {(int)getResponse.StatusCode} - Trabajador: {getResult!.Data!.Nombres} {getResult.Data.Apellidos}");
        _output.WriteLine("\n--- PASO 3: UPDATE ---");
        var updateDto = new UpdateTrabajadorDto
        {
            IdTrabajador = trabajadorId,
            Nombres = "CRUD Actualizado",
            Apellidos = "Test Modificado",
            TipoDocumento = "DNI",
            NumeroDocumento = documentoUnico,
            Sexo = 'M',
            FechaNacimiento = new DateTime(1990, 1, 1),
            Direccion = "Calle CRUD Actualizada"
        };
        _output.WriteLine($"PUT /api/trabajadores/{trabajadorId}");
        var updateResponse = await _client.PutAsJsonAsync($"/api/trabajadores/{trabajadorId}", updateDto, TestContext.Current.CancellationToken);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateResult = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<TrabajadorDto>>(TestContext.Current.CancellationToken);
        _output.WriteLine($"Status: {(int)updateResponse.StatusCode} - Actualizado a: {updateResult!.Data!.Nombres}");
        _output.WriteLine("\n--- PASO 4: DELETE ---");
        _output.WriteLine($"DELETE /api/trabajadores/{trabajadorId}");
        var deleteResponse = await _client.DeleteAsync($"/api/trabajadores/{trabajadorId}", TestContext.Current.CancellationToken);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        _output.WriteLine($"Status: {(int)deleteResponse.StatusCode} - Trabajador eliminado");
        _output.WriteLine("\n--- PASO 5: VERIFY DELETE ---");
        _output.WriteLine($"GET /api/trabajadores/{trabajadorId} (verificar eliminación)");
        var verifyResponse = await _client.GetAsync($"/api/trabajadores/{trabajadorId}", TestContext.Current.CancellationToken);
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _output.WriteLine($"Status: {(int)verifyResponse.StatusCode} - Confirmado: trabajador no existe");
        _output.WriteLine("\n========================================");
        _output.WriteLine("✓ CRUD COMPLETO - TEST PASSED");
        _output.WriteLine("========================================");
    }
    #endregion
    #region Helpers
    private static string GenerarDocumentoUnico()
    {
        return DateTime.Now.Ticks.ToString().Substring(10, 8);
    }
    private class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public int? Total { get; set; }
    }
    #endregion
}
