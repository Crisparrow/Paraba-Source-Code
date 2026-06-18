using Paraba.DriverApp.Models;
using Paraba.DriverApp.Services;

namespace Paraba.DriverApp;

public partial class MainPage : ContentPage
{
    private const int DemoDriverId = 1;

    private readonly DriverApiService _driverApiService = new();
    private DriverTripResponse? _activeTrip;
    private bool _isAvailable = true;
    private bool _initialized;

    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_initialized)
        {
            return;
        }

        _initialized = true;
        await ShowSplashAsync();
    }

    private async Task ShowSplashAsync()
    {
        SplashView.IsVisible = true;
        LoginView.IsVisible = false;
        DashboardView.IsVisible = false;

        await Task.Delay(1200);

        SplashView.IsVisible = false;
        LoginView.IsVisible = true;
        DashboardView.IsVisible = false;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PhoneEntry.Text))
        {
            await DisplayAlert("PARABA", "Ingresa tu numero de telefono para continuar.", "Aceptar");
            return;
        }

        LoginView.IsVisible = false;
        DashboardView.IsVisible = true;
        await LoadDriverDashboardAsync();
    }

    private async Task LoadDriverDashboardAsync()
    {
        try
        {
            SetBusyState(true);

            DriverProfileResponse? profile = await _driverApiService.GetProfileAsync(DemoDriverId);
            List<DriverTripResponse> trips = await _driverApiService.GetActiveTripsAsync(DemoDriverId);

            if (profile == null)
            {
                await DisplayAlert("PARABA", "No se encontro el perfil del conductor.", "Aceptar");
                return;
            }

            LoadProfile(profile);
            LoadTrip(trips.FirstOrDefault());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Conexion API", $"No se pudo conectar con Paraba.API. Detalle: {ex.Message}", "Aceptar");
        }
        finally
        {
            SetBusyState(false);
        }
    }

    private void LoadProfile(DriverProfileResponse profile)
    {
        _isAvailable = profile.Disponible;
        DriverNameLabel.Text = profile.NombreCompleto;
        DriverRatingLabel.Text = profile.Verificado ? "5.00" : "Pend.";

        DriverVehicleResponse? vehicle = profile.Vehiculos.FirstOrDefault(item => item.Activo);
        DriverVehicleLabel.Text = vehicle == null
            ? "Sin vehiculo activo"
            : $"{GetServiceName(vehicle.IdTipoServicio)} - {vehicle.Marca} {vehicle.Modelo} {vehicle.Placa}";

        TodayAmountLabel.Text = "Bs 0.00";
        TodayTripsLabel.Text = "Viajes reales desde API";
        PendingAmountLabel.Text = "Bs 0.00";

        UpdateAvailabilityUi();
    }

    private void LoadTrip(DriverTripResponse? trip)
    {
        _activeTrip = trip;

        if (trip == null)
        {
            ActiveTripTitleLabel.Text = "Sin viaje activo";
            ActiveTripStatusLabel.Text = "Disponible";
            OriginLabel.Text = "Esperando solicitud";
            DestinationLabel.Text = "Sin destino";
            SuggestedFareLabel.Text = "Bs 0.00";
            OfferedFareLabel.Text = "Bs 0.00";
            AcceptedFareLabel.Text = "Bs 0.00";
            StartTripButton.IsEnabled = false;
            FinishTripButton.IsEnabled = false;
            return;
        }

        ActiveTripTitleLabel.Text = $"Viaje #{trip.IdViaje}";
        ActiveTripStatusLabel.Text = trip.EstadoViaje;
        OriginLabel.Text = trip.Origen;
        DestinationLabel.Text = trip.Destino;
        SuggestedFareLabel.Text = FormatMoney(trip.TarifaSugerida);
        OfferedFareLabel.Text = FormatMoney(trip.TarifaOfertada);
        AcceptedFareLabel.Text = FormatMoney(trip.TarifaAceptada ?? trip.TarifaContraoferta ?? trip.TarifaFinal);
        PendingAmountLabel.Text = FormatMoney(trip.TarifaAceptada ?? trip.TarifaFinal);

        StartTripButton.IsEnabled = trip.EstadoViaje is "Aceptado" or "Solicitado";
        FinishTripButton.IsEnabled = trip.EstadoViaje == "En curso";
    }

    private void OnToggleAvailabilityClicked(object sender, EventArgs e)
    {
        _isAvailable = !_isAvailable;
        UpdateAvailabilityUi();
    }

    private async void OnStartTripClicked(object sender, EventArgs e)
    {
        if (_activeTrip == null)
        {
            await DisplayAlert("PARABA", "No hay viaje activo para iniciar.", "Aceptar");
            return;
        }

        try
        {
            SetBusyState(true);
            await _driverApiService.StartTripAsync(DemoDriverId, _activeTrip.IdViaje);
            await LoadDriverDashboardAsync();
            await DisplayAlert("PARABA", "Viaje iniciado correctamente.", "Aceptar");
        }
        catch (Exception ex)
        {
            await DisplayAlert("PARABA", $"No se pudo iniciar el viaje. Detalle: {ex.Message}", "Aceptar");
        }
        finally
        {
            SetBusyState(false);
        }
    }

    private async void OnFinishTripClicked(object sender, EventArgs e)
    {
        if (_activeTrip == null)
        {
            await DisplayAlert("PARABA", "No hay viaje activo para finalizar.", "Aceptar");
            return;
        }

        try
        {
            SetBusyState(true);
            await _driverApiService.FinishTripAsync(DemoDriverId, _activeTrip.IdViaje);
            await LoadDriverDashboardAsync();
            await DisplayAlert("PARABA", "Viaje finalizado correctamente.", "Aceptar");
        }
        catch (Exception ex)
        {
            await DisplayAlert("PARABA", $"No se pudo finalizar el viaje. Detalle: {ex.Message}", "Aceptar");
        }
        finally
        {
            SetBusyState(false);
        }
    }

    private void UpdateAvailabilityUi()
    {
        DriverStatusLabel.Text = _isAvailable ? "Disponible" : "No disponible";
        DriverStatusLabel.TextColor = _isAvailable ? Color.FromArgb("#2DFF72") : Color.FromArgb("#F23845");
        ToggleAvailabilityButton.Text = _isAvailable ? "Cambiar a no disponible" : "Cambiar a disponible";
        ToggleAvailabilityButton.BackgroundColor = _isAvailable ? Color.FromArgb("#20C65A") : Color.FromArgb("#F23845");
        ToggleAvailabilityButton.TextColor = Colors.White;
    }

    private void SetBusyState(bool isBusy)
    {
        StartTripButton.IsEnabled = !isBusy && _activeTrip != null && _activeTrip.EstadoViaje is "Aceptado" or "Solicitado";
        FinishTripButton.IsEnabled = !isBusy && _activeTrip != null && _activeTrip.EstadoViaje == "En curso";
        ToggleAvailabilityButton.IsEnabled = !isBusy;
    }

    private static string FormatMoney(decimal amount)
    {
        return $"Bs {amount:0.00}";
    }

    private static string GetServiceName(int serviceTypeId)
    {
        return serviceTypeId == 2 ? "Moto taxi" : "Taxi";
    }
}
