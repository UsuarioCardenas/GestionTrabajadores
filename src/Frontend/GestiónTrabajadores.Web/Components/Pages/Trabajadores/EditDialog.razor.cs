using GestiónTrabajadores.Application.DTOs;
using GestiónTrabajadores.Web.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GestiónTrabajadores.Web.Components.Pages.Trabajadores;

public partial class EditDialog
{
    [CascadingParameter]
    public IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter]
    public int TrabajadorId { get; set; }

    [Inject]
    public ITrabajadorApiService TrabajadorService { get; set; } = default!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = default!;

    protected MudForm _form = default!;
    protected bool _formIsValid;
    protected bool _loading = true;
    protected bool _saving;
    protected string? _serverError;
    protected UpdateTrabajadorDto _model = new();
    protected DateTime? _fechaNacimiento;
    protected string _sexoSeleccionado = "M";

    protected override async Task OnInitializedAsync()
    {
        await LoadTrabajador();
    }

    protected async Task LoadTrabajador()
    {
        _loading = true;
        try
        {
            var trabajador = await TrabajadorService.GetByIdAsync(TrabajadorId);

            if (trabajador != null)
            {
                _model = new UpdateTrabajadorDto
                {
                    IdTrabajador = trabajador.IdTrabajador,
                    Nombres = trabajador.Nombres,
                    Apellidos = trabajador.Apellidos,
                    TipoDocumento = trabajador.TipoDocumento,
                    NumeroDocumento = trabajador.NumeroDocumento,
                    Sexo = trabajador.Sexo,
                    FechaNacimiento = trabajador.FechaNacimiento,
                    Foto = trabajador.Foto,
                    Direccion = trabajador.Direccion
                };

                _fechaNacimiento = trabajador.FechaNacimiento;
                _sexoSeleccionado = trabajador.Sexo.ToString();
            }
            else
            {
                Snackbar.Add("No se pudo cargar el trabajador", Severity.Error);
                MudDialog.Close(DialogResult.Cancel());
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error al cargar trabajador: {ex.Message}", Severity.Error);
            MudDialog.Close(DialogResult.Cancel());
        }
        finally
        {
            _loading = false;
        }
    }

    protected void Cancel()
    {
        MudDialog.Close(DialogResult.Cancel());
    }

    protected async Task Submit()
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
        StateHasChanged();

        try
        {
            _model.FechaNacimiento = _fechaNacimiento!.Value;
            _model.Sexo = _sexoSeleccionado[0];

            var result = await TrabajadorService.UpdateAsync(TrabajadorId, _model);

            var genero = result.Sexo == 'F' ? "Trabajadora" : "Trabajador";
            Snackbar.Add($"{genero} '{result.NombreCompleto}' actualizado/a exitosamente", Severity.Success);
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