using GestiónTrabajadores.Application.DTOs;
using GestiónTrabajadores.Web.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GestiónTrabajadores.Web.Components.Pages.Trabajadores;

public partial class CreateDialog
{
    [CascadingParameter]
    public IMudDialogInstance MudDialog { get; set; } = default!;

    [Inject]
    public ITrabajadorApiService TrabajadorService { get; set; } = default!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = default!;

    private MudForm _form = default!;
    private bool _formIsValid;
    private bool _saving;
    private string? _serverError;
    private CreateTrabajadorDto _model = new();
    private DateTime? _fechaNacimiento = DateTime.Today.AddYears(-25);
    private string _sexoSeleccionado = "M";

    protected override void OnInitialized()
    {
        _model.TipoDocumento = "DNI";
    }

    private void Cancel()
    {
        MudDialog.Close(DialogResult.Cancel());
    }

    private async Task Submit()
    {
        await _form.Validate();

        bool hasErrors = false;

        if (string.IsNullOrWhiteSpace(_model.Nombres))
        {
            Snackbar.Add("Los nombres son obligatorios", Severity.Warning);
            hasErrors = true;
        }

        if (string.IsNullOrWhiteSpace(_model.Apellidos))
        {
            Snackbar.Add("Los apellidos son obligatorios", Severity.Warning);
            hasErrors = true;
        }

        if (string.IsNullOrWhiteSpace(_model.NumeroDocumento))
        {
            Snackbar.Add("El número de documento es obligatorio", Severity.Warning);
            hasErrors = true;
        }

        if (_fechaNacimiento == null)
        {
            Snackbar.Add("La fecha de nacimiento es obligatoria", Severity.Warning);
            hasErrors = true;
        }

        if (string.IsNullOrWhiteSpace(_model.Direccion))
        {
            Snackbar.Add("La dirección es obligatoria", Severity.Warning);
            hasErrors = true;
        }

        if (hasErrors)
        {
            return;
        }

        _saving = true;
        _serverError = null;

        try
        {
            _model.FechaNacimiento = _fechaNacimiento!.Value;
            _model.Sexo = _sexoSeleccionado[0];

            var result = await TrabajadorService.CreateAsync(_model);

            var genero = result.Sexo == 'F' ? "Trabajadora" : "Trabajador";
            Snackbar.Add($"{genero} '{result.NombreCompleto}' creado/a exitosamente", Severity.Success);
            MudDialog.Close(DialogResult.Ok(result));
        }
        catch (HttpRequestException ex)
        {
            var message = ex.Message;
            if (message.Contains(" - "))
            {
                var parts = message.Split(" - ", 2);
                if (parts.Length > 1)
                {
                    try
                    {
                        var json = System.Text.Json.JsonDocument.Parse(parts[1]);
                        if (json.RootElement.TryGetProperty("errors", out var errors))
                        {
                            message = errors.GetString() ?? message;
                        }
                        else if (json.RootElement.TryGetProperty("message", out var msg))
                        {
                            message = msg.GetString() ?? message;
                        }
                    }
                    catch
                    {
                        message = parts[1];
                    }
                }
            }
            _serverError = message;
        }
        catch (Exception ex)
        {
            _serverError = ex.Message;
        }
        finally
        {
            _saving = false;
        }
    }
}