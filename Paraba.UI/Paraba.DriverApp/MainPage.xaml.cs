using Paraba.DriverApp.Models;
using Paraba.DriverApp.Services;
using System.IO;

namespace Paraba.DriverApp;

public partial class MainPage : ContentPage
{
    private readonly DriverApiService _driverApiService = new();
    private DriverTripResponse? _activeTrip;
    private DriverRegistrationResponse? _registration;
    private string _phone = string.Empty;
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

        try
        {
            SetBusyState(true);
            _phone = PhoneEntry.Text.Trim();
            DriverRequestCodeResponse? response = await _driverApiService.RequestCodeAsync(_phone);
            _registration = response?.Solicitud;

            OtpPhoneLabel.Text = $"Enviado al +591 {_phone} por WhatsApp o SMS";
            OtpCodeEntry.Text = string.Empty;
            LoginView.IsVisible = false;
            OtpView.IsVisible = true;
            NameRegistrationView.IsVisible = false;
            BiometricView.IsVisible = false;
            DashboardView.IsVisible = false;

            if (!string.IsNullOrWhiteSpace(response?.CodigoDemo))
            {
                await DisplayAlert("PARABA beta", $"Código demo generado por la API: {response.CodigoDemo}", "Aceptar");
            }

            OtpCodeEntry.Focus();
        }
        catch (Exception ex)
        {
            await DisplayAlert("PARABA", $"No se pudo solicitar el código. Detalle: {ex.Message}", "Aceptar");
        }
        finally
        {
            SetBusyState(false);
        }
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
        await RequestNewCodeAsync("WhatsApp");
    }

    private async void OnSendSmsClicked(object sender, EventArgs e)
    {
        await RequestNewCodeAsync("SMS");
    }

    private async void OnVerifyCodeClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(OtpCodeEntry.Text))
        {
            await DisplayAlert("PARABA", "Ingresa el código de verificación.", "Aceptar");
            OtpCodeEntry.Focus();
            return;
        }

        try
        {
            SetBusyState(true);
            DriverVerifyCodeResponse? response = await _driverApiService.VerifyCodeAsync(_phone, OtpCodeEntry.Text);
            _registration = response?.Solicitud;

            if (_registration != null && !string.IsNullOrWhiteSpace(_registration.NombreCompleto))
            {
                await ContinueAfterBiometricAsync();
                return;
            }

            OtpView.IsVisible = false;
            LoginView.IsVisible = false;
            NameRegistrationView.IsVisible = true;
            BiometricView.IsVisible = false;
            DashboardView.IsVisible = false;
            FirstNameEntry.Focus();
        }
        catch (Exception ex)
        {
            await DisplayAlert("PARABA", $"Código inválido o expirado. Detalle: {ex.Message}", "Aceptar");
            OtpCodeEntry.Focus();
        }
        finally
        {
            SetBusyState(false);
        }
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

        try
        {
            SetBusyState(true);
            string fullName = $"{FirstNameEntry.Text.Trim()} {LastNameEntry.Text.Trim()}";
            _registration = await _driverApiService.SaveRegistrationDraftAsync(new DriverRegistrationDraftRequest
            {
                Telefono = _phone,
                NombreCompleto = fullName
            });

            await DisplayAlert("PARABA", "Datos guardados. Podrás completar tus documentos y vehículo más adelante.", "Aceptar");
            NameRegistrationView.IsVisible = false;
            BiometricView.IsVisible = true;
            DashboardView.IsVisible = false;
        }
        catch (Exception ex)
        {
            await DisplayAlert("PARABA", $"No se pudo guardar tu registro. Detalle: {ex.Message}", "Aceptar");
        }
        finally
        {
            SetBusyState(false);
        }
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

            if (_registration?.IdConductor == null || !_registration.PuedeOperar)
            {
                LoadRegistrationPreview();
                return;
            }

            int driverId = _registration.IdConductor.Value;
            DriverProfileResponse? profile = await _driverApiService.GetProfileAsync(driverId);
            List<DriverTripResponse> trips = await _driverApiService.GetActiveTripsAsync(driverId);

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
        UpdateRegistrationStatusUi();
    }

    private void LoadTrip(DriverTripResponse? trip)
    {
        _activeTrip = trip;
    }

    private void LoadRegistrationPreview()
    {
        _activeTrip = null;
        _isAvailable = false;
        ProfileNameLabel.Text = string.IsNullOrWhiteSpace(_registration?.NombreCompleto)
            ? "Conductor PARABA"
            : _registration.NombreCompleto;
        ProfileRatingLabel.Text = _registration?.EstadoSolicitud ?? "Borrador";
        DriverTopStatusLabel.Text = "Completa tu registro para operar";
        UpdateRegistrationStatusUi();
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
            int? driverId = _registration?.IdConductor;

            if (driverId == null)
            {
                await DisplayAlert("PARABA", "Completa y espera la aprobación de tu registro para iniciar viajes.", "Aceptar");
                return;
            }

            await _driverApiService.StartTripAsync(driverId.Value, _activeTrip.IdViaje);
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
            int? driverId = _registration?.IdConductor;

            if (driverId == null)
            {
                await DisplayAlert("PARABA", "Completa y espera la aprobación de tu registro para finalizar viajes.", "Aceptar");
                return;
            }

            await _driverApiService.FinishTripAsync(driverId.Value, _activeTrip.IdViaje);
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

    private async void OnCompleteRegistrationClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Completar registro",
            "Beta actual: ya puedes guardar teléfono y nombre. El siguiente paso conectará las pantallas para datos del conductor, vehículo y carga de documentos.",
            "Aceptar");
    }

    private void UpdateRegistrationStatusUi()
    {
        SetStatusLabel(DriverDataStatusLabel, "Datos conductor", _registration?.DatosConductorCompletos == true);
        SetStatusLabel(VehicleDataStatusLabel, "Datos vehículo", _registration?.DatosVehiculoCompletos == true);
        SetStatusLabel(DocumentsStatusLabel, "Documentos", _registration?.DocumentosCompletos == true);
    }

    private static void SetStatusLabel(Label label, string title, bool complete)
    {
        label.Text = complete ? $"{title}: completo" : $"{title}: falta completar";
        label.TextColor = complete
            ? Color.FromArgb("#16A34A")
            : Color.FromArgb("#DC2626");
    }

    private async Task RequestNewCodeAsync(string channel)
    {
        try
        {
            SetBusyState(true);
            OtpCodeEntry.Text = string.Empty;
            DriverRequestCodeResponse? response = await _driverApiService.RequestCodeAsync(_phone);

            await DisplayAlert("PARABA beta", $"Código reenviado por {channel}. Código demo: {response?.CodigoDemo}", "Aceptar");
            OtpCodeEntry.Focus();
        }
        catch (Exception ex)
        {
            await DisplayAlert("PARABA", $"No se pudo reenviar el código. Detalle: {ex.Message}", "Aceptar");
        }
        finally
        {
            SetBusyState(false);
        }
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


