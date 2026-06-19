using Paraba.DriverApp.Models;
using Paraba.DriverApp.Services;
using System.IO;

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
        OtpView.IsVisible = false;
        NameRegistrationView.IsVisible = false;
        BiometricView.IsVisible = false;
        DashboardView.IsVisible = false;
        await LoadSplashImageAsync();

        await Task.Delay(5000);

        SplashView.IsVisible = false;
        LoginView.IsVisible = true;
        OtpView.IsVisible = false;
        NameRegistrationView.IsVisible = false;
        BiometricView.IsVisible = false;
        DashboardView.IsVisible = false;
    }

    private async Task LoadSplashImageAsync()
    {
        if (SplashImage.Source != null)
        {
            return;
        }

        try
        {
            byte[] imageBytes;
            string outputPath = Path.Combine(AppContext.BaseDirectory, "paraba_intro.png");

            if (File.Exists(outputPath))
            {
                imageBytes = await File.ReadAllBytesAsync(outputPath);
            }
            else
            {
                await using Stream stream = await FileSystem.OpenAppPackageFileAsync("paraba_intro.png");
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            SplashImage.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            SplashFallback.IsVisible = false;
        }
        catch (IOException)
        {
            SplashFallback.IsVisible = true;
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PhoneEntry.Text))
        {
            await DisplayAlert("PARABA", "Ingresa tu número de teléfono para continuar.", "Aceptar");
            return;
        }

        OtpPhoneLabel.Text = $"Enviado al +591 {PhoneEntry.Text} por WhatsApp";
        OtpCodeEntry.Text = string.Empty;
        LoginView.IsVisible = false;
        OtpView.IsVisible = true;
        NameRegistrationView.IsVisible = false;
        BiometricView.IsVisible = false;
        DashboardView.IsVisible = false;
        OtpCodeEntry.Focus();
    }

    private void OnBackToLoginClicked(object sender, EventArgs e)
    {
        OtpView.IsVisible = false;
        LoginView.IsVisible = true;
        NameRegistrationView.IsVisible = false;
        BiometricView.IsVisible = false;
        DashboardView.IsVisible = false;
        PhoneEntry.Focus();
    }

    private async void OnResendCodeClicked(object sender, EventArgs e)
    {
        OtpCodeEntry.Text = string.Empty;
        await DisplayAlert("PARABA", "Código reenviado por WhatsApp. Código demo: 123456", "Aceptar");
        OtpCodeEntry.Focus();
    }

    private async void OnSendSmsClicked(object sender, EventArgs e)
    {
        OtpCodeEntry.Text = string.Empty;
        await DisplayAlert("PARABA", "Código enviado por SMS. Código demo: 123456", "Aceptar");
        OtpCodeEntry.Focus();
    }

    private async void OnVerifyCodeClicked(object sender, EventArgs e)
    {
        if (OtpCodeEntry.Text != "123456")
        {
            await DisplayAlert("PARABA", "Código incorrecto. Por ahora usa el código demo: 123456", "Aceptar");
            OtpCodeEntry.Focus();
            return;
        }

        OtpView.IsVisible = false;
        LoginView.IsVisible = false;
        NameRegistrationView.IsVisible = true;
        BiometricView.IsVisible = false;
        DashboardView.IsVisible = false;
        FirstNameEntry.Focus();
    }

    private void OnBackToOtpClicked(object sender, EventArgs e)
    {
        NameRegistrationView.IsVisible = false;
        OtpView.IsVisible = true;
        LoginView.IsVisible = false;
        BiometricView.IsVisible = false;
        DashboardView.IsVisible = false;
        OtpCodeEntry.Focus();
    }

    private async void OnNameNextClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FirstNameEntry.Text) || string.IsNullOrWhiteSpace(LastNameEntry.Text))
        {
            await DisplayAlert("PARABA", "Ingresa tu nombre y apellido para continuar.", "Aceptar");
            return;
        }

        await DisplayAlert("PARABA", "Datos guardados. Ahora puedes activar el acceso por rostro o huella.", "Aceptar");
        NameRegistrationView.IsVisible = false;
        BiometricView.IsVisible = true;
        DashboardView.IsVisible = false;
    }

    private void OnBackToNameClicked(object sender, EventArgs e)
    {
        BiometricView.IsVisible = false;
        NameRegistrationView.IsVisible = true;
        OtpView.IsVisible = false;
        LoginView.IsVisible = false;
        DashboardView.IsVisible = false;
        FirstNameEntry.Focus();
    }

    private async void OnEnableBiometricClicked(object sender, EventArgs e)
    {
        await DisplayAlert("PARABA", "Biometría activada en modo demo. Luego conectaremos rostro/huella real del dispositivo.", "Aceptar");
        await ContinueAfterBiometricAsync();
    }

    private async void OnSkipBiometricClicked(object sender, EventArgs e)
    {
        await ContinueAfterBiometricAsync();
    }

    private async Task ContinueAfterBiometricAsync()
    {
        BiometricView.IsVisible = false;
        NameRegistrationView.IsVisible = false;
        OtpView.IsVisible = false;
        LoginView.IsVisible = false;
        DashboardView.IsVisible = true;
        await LoadDriverDashboardAsync();
    }

    private async void OnUserAgreementTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert(
            "Acuerdo de usuario",
            "Aquí se mostrarán los términos de uso de PARABA: uso de la plataforma, responsabilidades del conductor, reglas de viajes, pagos, suspensiones y condiciones del servicio.",
            "Aceptar");
    }

    private async void OnPrivacyPolicyTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert(
            "Politica de privacidad",
            "Aquí se mostrará cómo PARABA trata datos personales: teléfono, documentos, ubicación, viajes, pagos, seguridad y conservación de información.",
            "Aceptar");
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
                await DisplayAlert("PARABA", "No se encontró el perfil del conductor.", "Aceptar");
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
        ProfileNameLabel.Text = string.IsNullOrWhiteSpace(profile.NombreCompleto)
            ? "Conductor PARABA"
            : profile.NombreCompleto;
        ProfileRatingLabel.Text = profile.Verificado ? "5.0" : "Pend.";
        DriverTopStatusLabel.Text = _isAvailable ? "Pedidos disponibles" : "Pedidos no disponibles";
    }

    private void LoadTrip(DriverTripResponse? trip)
    {
        _activeTrip = trip;
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
        DriverTopStatusLabel.Text = _isAvailable ? "Pedidos disponibles" : "Pedidos no disponibles";
    }

    private void SetBusyState(bool isBusy)
    {
    }

    private void OnOrdersTabClicked(object sender, EventArgs e)
    {
        ShowDashboardTab(OrdersTabView, OrdersTabButton);
    }

    private void OnMoneyTabClicked(object sender, EventArgs e)
    {
        ShowDashboardTab(MoneyTabView, MoneyTabButton);
    }

    private void OnChatsTabClicked(object sender, EventArgs e)
    {
        ShowDashboardTab(ChatsTabView, ChatsTabButton);
    }

    private void OnProfileTabClicked(object sender, EventArgs e)
    {
        ShowDashboardTab(ProfileTabView, ProfileTabButton);
    }

    private void ShowDashboardTab(View activeView, Button activeButton)
    {
        OrdersTabView.IsVisible = activeView == OrdersTabView;
        MoneyTabView.IsVisible = activeView == MoneyTabView;
        ChatsTabView.IsVisible = activeView == ChatsTabView;
        ProfileTabView.IsVisible = activeView == ProfileTabView;

        OrdersTabButton.TextColor = Color.FromArgb("#9AA0A8");
        MoneyTabButton.TextColor = Color.FromArgb("#9AA0A8");
        ChatsTabButton.TextColor = Color.FromArgb("#9AA0A8");
        ProfileTabButton.TextColor = Color.FromArgb("#9AA0A8");
        activeButton.TextColor = Color.FromArgb("#111827");
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


