namespace Paraba.DriverApp;

public partial class MainPage : ContentPage
{
    private bool _isAvailable = true;
    private bool _tripStarted;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnToggleAvailabilityClicked(object sender, EventArgs e)
    {
        _isAvailable = !_isAvailable;

        DriverStatusLabel.Text = _isAvailable ? "Disponible" : "No disponible";
        DriverStatusLabel.TextColor = _isAvailable ? Color.FromArgb("#2DFF72") : Color.FromArgb("#F23845");
        ToggleAvailabilityButton.Text = _isAvailable ? "Cambiar a no disponible" : "Cambiar a disponible";
        ToggleAvailabilityButton.BackgroundColor = _isAvailable ? Color.FromArgb("#20C65A") : Color.FromArgb("#F23845");
        ToggleAvailabilityButton.TextColor = Colors.White;
    }

    private async void OnStartTripClicked(object sender, EventArgs e)
    {
        if (_tripStarted)
        {
            await DisplayAlert("Viaje en curso", "El viaje ya fue iniciado.", "Aceptar");
            return;
        }

        _tripStarted = true;
        await DisplayAlert("PARABA", "Viaje iniciado. En la siguiente fase esto actualizará la API y notificará al pasajero.", "Aceptar");
    }

    private async void OnFinishTripClicked(object sender, EventArgs e)
    {
        if (!_tripStarted)
        {
            await DisplayAlert("Viaje pendiente", "Primero debes iniciar el viaje antes de finalizarlo.", "Aceptar");
            return;
        }

        _tripStarted = false;
        await DisplayAlert("PARABA", "Viaje finalizado. Luego conectaremos calificación, pago y liquidación.", "Aceptar");
    }
}
