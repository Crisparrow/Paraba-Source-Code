using Paraba.DriverApp.Models;
using Paraba.DriverApp.Services;

namespace Paraba.DriverApp.Views.Profile;

public partial class AddVehiclePage : ContentPage
{
    private readonly DriverProfileService service = new();
    private readonly int driverId;
    private readonly Func<Task>? completed;

    public AddVehiclePage() : this(0, null) { }

    public AddVehiclePage(int driverId, Func<Task>? completed)
    {
        InitializeComponent();
        this.driverId = driverId;
        this.completed = completed;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ServiceTypePicker.ItemsSource == null)
            ServiceTypePicker.ItemsSource = await service.GetServiceTypesAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (ServiceTypePicker.SelectedItem is not DriverServiceTypeResponse type ||
            !int.TryParse(YearEntry.Text, out int year))
        {
            await DisplayAlert("Vehículo", "Selecciona el servicio e ingresa un año válido.", "Aceptar");
            return;
        }

        try
        {
            SaveButton.IsEnabled = false;
            await service.CreateVehicleAsync(driverId, new DriverVehicleCreateRequest
            {
                IdTipoServicio = type.IdTipoServicio,
                Placa = PlateEntry.Text ?? string.Empty,
                Marca = BrandEntry.Text ?? string.Empty,
                Modelo = ModelEntry.Text ?? string.Empty,
                Color = ColorEntry.Text ?? string.Empty,
                Anio = year
            });
            await DisplayAlert("PARABA", "Vehículo registrado como Pendiente.", "Aceptar");
            if (completed != null) await completed();
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Vehículo", ex.Message, "Aceptar");
        }
        finally
        {
            SaveButton.IsEnabled = true;
        }
    }
}
