using GestiónTrabajadores.Application.DTOs;
using GestiónTrabajadores.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace GestiónTrabajadores.Web.Components.Pages.Trabajadores;

public partial class CreateDialog
{
    [CascadingParameter]
    public IMudDialogInstance MudDialog { get; set; } = default!;

    [Inject]
    public ITrabajadorApiService TrabajadorService { get; set; } = default!;

    [Inject]
    public IImagenApiService ImagenService { get; set; } = default!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = default!;

    private MudForm _form = default!;
    private bool _formIsValid;
    private bool _saving;
    private bool _uploading;
    private string? _previewUrl;
    private string? _uploadError;
    private string? _serverError;
    private CreateTrabajadorDto _model = new();
    private DateTime? _fechaNacimiento = DateTime.Today.AddYears(-25);
    private string _sexoSeleccionado = "M";

    protected override void OnInitialized()
    {
        _model.TipoDocumento = "DNI";
    }

    private async Task OnFileSelected(IBrowserFile file)
    {
        _uploadError = null;

        if (file.Size > 5 * 1024 * 1024)
        {
            _uploadError = "El archivo no puede superar los 5MB";
            Snackbar.Add(_uploadError, Severity.Error);
            return;
        }

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
        {
            _uploadError = "Solo se permiten archivos JPG, JPEG o PNG";
            Snackbar.Add(_uploadError, Severity.Error);
            return;
        }

        _uploading = true;
        StateHasChanged();

        try
        {
            var buffer = new byte[file.Size];
            using var browserStream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
            await browserStream.ReadExactlyAsync(buffer);

            _previewUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";
            StateHasChanged();

            var fileName = file.Name;
            var contentType = file.ContentType;

            var url = await Task.Run(async () =>
            {
                using var stream = new MemoryStream(buffer);
                return await ImagenService.UploadImageAsync(stream, fileName, contentType);
            });

            if (!string.IsNullOrEmpty(url))
            {
                _model.Foto = url;
                _previewUrl = url;
                Snackbar.Add("Imagen subida correctamente", Severity.Success);
            }
            else
            {
                _uploadError = "Error al subir la imagen. Intente de nuevo.";
                Snackbar.Add(_uploadError, Severity.Error);
                _previewUrl = null;
                _model.Foto = null;
            }
        }
        catch (Exception ex)
        {
            _uploadError = $"Error: {ex.Message}";
            Snackbar.Add(_uploadError, Severity.Error);
            _previewUrl = null;
            _model.Foto = null;
        }
        finally
        {
            _uploading = false;
            StateHasChanged();
        }
    }

    private void RemovePhoto()
    {
        _model.Foto = null;
        _previewUrl = null;
        _uploadError = null;
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
                        if (json.RootElement.TryGetProperty("message", out var msg))
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