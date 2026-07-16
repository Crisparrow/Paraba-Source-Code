using Paraba.DriverApp.Models;

namespace Paraba.DriverApp.Views.Orders;

public partial class TripRequestCard : ContentView
{
    private const int InitialCountdownSeconds = 20;
    private IDispatcherTimer? _countdownTimer;
    private int _remainingSeconds = InitialCountdownSeconds;
    private bool _isExpired;
    private bool _actionRequested;

    public event EventHandler<DriverTripResponse>? AcceptRequested;
    public event EventHandler<DriverTripResponse>? CounterOfferRequested;
    public event EventHandler<DriverTripResponse>? CancelRequested;
    public event EventHandler<DriverTripResponse>? ExpiredRequested;

    public static readonly BindableProperty TripProperty = BindableProperty.Create(
        nameof(Trip),
        typeof(DriverTripResponse),
        typeof(TripRequestCard),
        propertyChanged: OnTripChanged);

    public DriverTripResponse? Trip
    {
        get => (DriverTripResponse?)GetValue(TripProperty);
        set => SetValue(TripProperty, value);
    }

    public TripRequestCard()
    {
        InitializeComponent();
        Unloaded += (_, _) => StopCountdown();
    }

    private static void OnTripChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((TripRequestCard)bindable).Render();
    }

    private void Render()
    {
        if (Trip == null)
        {
            StopCountdown();
            ServiceLabel.Text = string.Empty;
            OriginLabel.Text = string.Empty;
            DestinationLabel.Text = string.Empty;
            FareLabel.Text = string.Empty;
            DistanceLabel.Text = string.Empty;
            TimeLabel.Text = string.Empty;
            TripDistanceLabel.Text = string.Empty;
            TripDurationLabel.Text = string.Empty;
            PassengerOfferLabel.Text = string.Empty;
            return;
        }

        _actionRequested = false;
        _isExpired = false;
        ServiceLabel.Text = GetTripServiceName(Trip);
        OriginLabel.Text = Trip.Origen;
        DestinationLabel.Text = Trip.Destino;
        FareLabel.Text = FormatMoney(GetTripFare(Trip));
        DistanceLabel.Text = GetPickupDistance(Trip);
        TimeLabel.Text = GetPickupEta(Trip);
        TripDistanceLabel.Text = GetTripDistance(Trip);
        TripDurationLabel.Text = GetTripDuration(Trip);
        PassengerOfferLabel.Text = FormatMoney(Trip.TarifaOfertada > 0 ? Trip.TarifaOfertada : Trip.TarifaSugerida);
        ResetVisualState();
        StartCountdown();
    }

    private void OnAcceptClicked(object sender, EventArgs e)
    {
        if (Trip != null && CanRequestAction())
        {
            _actionRequested = true;
            StopCountdown();
            AcceptRequested?.Invoke(this, Trip);
        }
    }

    private void OnCounterOfferClicked(object sender, EventArgs e)
    {
        if (Trip != null && CanRequestAction())
        {
            _actionRequested = true;
            StopCountdown();
            CounterOfferRequested?.Invoke(this, Trip);
        }
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        if (Trip != null && CanRequestAction())
        {
            _actionRequested = true;
            StopCountdown();
            CancelRequested?.Invoke(this, Trip);
        }
    }

    private bool CanRequestAction()
    {
        return !_isExpired && !_actionRequested;
    }

    private void StartCountdown()
    {
        StopCountdown();
        _remainingSeconds = InitialCountdownSeconds;
        RenderCountdown();

        _countdownTimer = Dispatcher.CreateTimer();
        _countdownTimer.Interval = TimeSpan.FromSeconds(1);
        _countdownTimer.Tick += OnCountdownTick;
        _countdownTimer.Start();
    }

    private void StopCountdown()
    {
        if (_countdownTimer == null)
        {
            return;
        }

        _countdownTimer.Stop();
        _countdownTimer.Tick -= OnCountdownTick;
        _countdownTimer = null;
    }

    private void OnCountdownTick(object? sender, EventArgs e)
    {
        if (Trip == null || _actionRequested)
        {
            StopCountdown();
            return;
        }

        _remainingSeconds--;
        RenderCountdown();

        if (_remainingSeconds > 0)
        {
            return;
        }

        _isExpired = true;
        StopCountdown();
        RenderExpiredState();
        ExpiredRequested?.Invoke(this, Trip);
    }

    private void RenderCountdown()
    {
        int visibleSeconds = Math.Max(_remainingSeconds, 0);
        CountdownLabel.Text = $"{visibleSeconds}s";
        CountdownProgress.Progress = Math.Clamp((double)visibleSeconds / InitialCountdownSeconds, 0, 1);

        if (visibleSeconds <= 5)
        {
            CountdownLabel.TextColor = Color.FromArgb("#991B1B");
            CountdownProgress.ProgressColor = Color.FromArgb("#EF4444");
        }
        else
        {
            CountdownLabel.TextColor = Color.FromArgb("#92400E");
            CountdownProgress.ProgressColor = Color.FromArgb("#F59E0B");
        }
    }

    private void RenderExpiredState()
    {
        RootBorder.BackgroundColor = Color.FromArgb("#F3F4F6");
        RootBorder.Stroke = Color.FromArgb("#D1D5DB");
        CountdownLabel.Text = "Expirado";
        CountdownLabel.TextColor = Color.FromArgb("#6B7280");
        CountdownProgress.Progress = 0;
        FareLabel.Text = "Pedido expirado";
        FareLabel.FontSize = 22;
        AcceptButton.IsEnabled = false;
        CounterOfferButton.IsEnabled = false;
        CancelButton.IsEnabled = false;
    }

    private void ResetVisualState()
    {
        RootBorder.BackgroundColor = Color.FromArgb("#F8FAFC");
        RootBorder.Stroke = Color.FromArgb("#E5E7EB");
        FareLabel.FontSize = 32;
        AcceptButton.IsEnabled = true;
        CounterOfferButton.IsEnabled = true;
        CancelButton.IsEnabled = true;
    }

    private static decimal GetTripFare(DriverTripResponse trip)
    {
        return trip.TarifaAceptada
            ?? trip.TarifaContraoferta
            ?? (trip.TarifaOfertada > 0 ? trip.TarifaOfertada : trip.TarifaSugerida);
    }

    private static string GetTripServiceName(DriverTripResponse trip)
    {
        return string.IsNullOrWhiteSpace(trip.TipoServicio)
            ? GetServiceName(trip.IdTipoServicio)
            : trip.TipoServicio;
    }

    private static string FormatMoney(decimal amount)
    {
        return $"Bs {amount:0.00}";
    }

    private static string GetServiceName(int serviceTypeId)
    {
        return serviceTypeId switch
        {
            2 => "Moto taxi",
            3 => "Taxi confort",
            4 => "Taxi XL",
            5 => "Taxi premium",
            _ => "Taxi economico"
        };
    }

    private static string GetPickupDistance(DriverTripResponse trip)
    {
        decimal kilometers = trip.IdTipoServicio switch
        {
            2 => 1.4m,
            3 => 2.7m,
            4 => 3.2m,
            5 => 2.1m,
            _ => 2.3m
        };

        return $"{kilometers:0.0} km al pasajero";
    }

    private static string GetPickupEta(DriverTripResponse trip)
    {
        int minutes = trip.IdTipoServicio switch
        {
            2 => 4,
            3 => 7,
            4 => 8,
            5 => 6,
            _ => 6
        };

        return $"Llegas en {minutes} min";
    }

    private static string GetTripDistance(DriverTripResponse trip)
    {
        decimal kilometers = trip.IdTipoServicio switch
        {
            2 => 4.2m,
            3 => 9.8m,
            4 => 11.4m,
            5 => 8.5m,
            _ => 6.3m
        };

        return $"{kilometers:0.0} km";
    }

    private static string GetTripDuration(DriverTripResponse trip)
    {
        int minutes = trip.IdTipoServicio switch
        {
            2 => 12,
            3 => 20,
            4 => 24,
            5 => 18,
            _ => 16
        };

        return $"{minutes} min";
    }
}
