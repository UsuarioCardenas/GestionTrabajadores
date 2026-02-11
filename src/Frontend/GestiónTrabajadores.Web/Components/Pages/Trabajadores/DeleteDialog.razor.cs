using GestiónTrabajadores.Application.DTOs;
using GestiónTrabajadores.Web.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GestiónTrabajadores.Web.Components.Pages.Trabajadores;

public partial class DeleteDialog
{
    [CascadingParameter]
    public IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter]
    public TrabajadorDto Trabajador { get; set; } = default!;

    [Inject]
    public ITrabajadorApiService TrabajadorService { get; set; } = default!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = default!;

    protected bool _deleting;

    protected void Cancel()
    {
        MudDialog.Close(DialogResult.Cancel());
    }

    protected async Task Confirm()
    {
        _deleting = true;

        try
        {
            var success = await TrabajadorService.DeleteAsync(Trabajador.IdTrabajador);

            if (success)
            {
                var genero = Trabajador.Sexo == 'F' ? "Trabajadora" : "Trabajador";
                Snackbar.Add($"{genero} '{Trabajador.NombreCompleto}' eliminado/a exitosamente", Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add("No se pudo eliminar el registro", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error al eliminar: {ex.Message}", Severity.Error);
        }
        finally
        {
            _deleting = false;
        }
    }
}