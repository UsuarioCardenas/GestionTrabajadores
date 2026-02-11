using GestiónTrabajadores.Application.DTOs;
using GestiónTrabajadores.Web.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GestiónTrabajadores.Web.Components.Pages.Trabajadores;

public partial class Index
{
    [Inject]
    ITrabajadorApiService TrabajadorService { get; set; } = default!;

    [Inject]
    IDialogService DialogService { get; set; } = default!;

    [Inject]
    ISnackbar Snackbar { get; set; } = default!;

    List<TrabajadorDto> _trabajadores = new();
    bool _loading = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadTrabajadores();
    }

    async Task LoadTrabajadores()
    {
        _loading = true;
        try
        {
            _trabajadores = await TrabajadorService.GetAllAsync();
            Snackbar.Add($"Se obtuvieron {_trabajadores.Count} trabajadores", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error al cargar trabajadores: {ex.Message}", Severity.Error);
        }
        finally
        {
            _loading = false;
        }
    }
}