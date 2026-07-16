using Paraba.DriverApp.Models;

namespace Paraba.DriverApp.Views.Orders;

public partial class TripDetailView : ContentView
{
    private const int EstadoAceptado = 2;
    private const int EstadoEnCurso = 3;
    private const int EstadoContraofertado = 6;
    private const int EstadoCaminoPasajero = 7;

    public event EventHandler<DriverTripResponse>? StartRequested;
    public event EventHandler<DriverTripResponse>? FinishRequested;
    public event EventHandler<DriverTripResponse>? PassengerAcceptRequested;

    public static readonly BindableProperty TripProperty = BindableProperty.Create(
        nameof(Trip),
        typeof(DriverTripResponse),
        typeof(TripDetailView),
        propertyChanged: OnTripChanged);

    public DriverTripResponse? Trip
    {
        get => (DriverTripResponse?)GetValue(TripProperty);
        set => SetValue(TripProperty, value);
    }

    public TripDetailView()
    {
        InitializeComponent();
        Render();
    }

    private static void OnTripChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((TripDetailView)bindable).Render();
    }

    private void Render()
    {
        bool hasTrip = Trip != null;
        IsVisible = hasTrip;
        RootFrame.IsVisible = hasTrip;

        if (!hasTrip)
        {
            StepTitleLabel.Text = "Viaje activo";
            StatusLabel.Text = string.Empty;
            OriginLabel.Text = string.Empty;
            DestinationLabel.Text = string.Empty;
            FareLabel.Text = string.Empty;
            PassengerAcceptButton.IsVisible = false;
            StartTripButton.IsVisible = false;
            FinishTripButton.IsVisible = false;
            return;
        }

        DriverTripResponse trip = Trip!;

        StepTitleLabel.Text = GetStepTitle(trip);
        StatusLabel.Text = $"{GetTripServiceName(trip)} - {GetOperationalStep(trip)}";
        OriginLabel.Text = trip.Origen;
        DestinationLabel.Text = trip.Destino;
        FareLabel.Text = FormatMoney(GetTripFare(trip));

        PassengerAcceptButton.IsVisible = trip.IdEstadoViaje == EstadoContraofertado;
        PassengerAcceptButton.IsEnabled = trip.IdEstadoViaje == EstadoContraofertado;

        StartTripButton.IsVisible = trip.IdEstadoViaje == EstadoAceptado || trip.IdEstadoViaje == EstadoCaminoPasajero;
        StartTripButton.IsEnabled = trip.IdEstadoViaje == EstadoAceptado || trip.IdEstadoViaje == EstadoCaminoPasajero;
        StartTripButton.Text = trip.IdEstadoViaje == EstadoCaminoPasajero ? "Llegue al pasajero" : "Ir a recoger";

        FinishTripButton.IsVisible = trip.IdEstadoViaje == EstadoEnCurso;
        FinishTripButton.IsEnabled = trip.IdEstadoViaje == EstadoEnCurso;
    }

    private void OnStartClicked(object sender, EventArgs e)
    {
        if (Trip != null)
        {
            StartRequested?.Invoke(this, Trip);
        }
    }

    private void OnFinishClicked(object sender, EventArgs e)
    {
        if (Trip != null)
        {
            FinishRequested?.Invoke(this, Trip);
        }
    }

    private void OnPassengerAcceptClicked(object sender, EventArgs e)
    {
        if (Trip != null)
        {
            PassengerAcceptRequested?.Invoke(this, Trip);
        }
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

    private static string GetOperationalStep(DriverTripResponse trip)
    {
        return trip.IdEstadoViaje switch
        {
            EstadoContraofertado => "Esperando respuesta del pasajero",
            EstadoAceptado => "Aceptado",
            EstadoCaminoPasajero => "Ir a recoger pasajero",
            EstadoEnCurso => "Viaje en curso",
            _ => string.IsNullOrWhiteSpace(trip.EstadoViaje) ? "Viaje activo" : trip.EstadoViaje
        };
    }

    private static string GetStepTitle(DriverTripResponse trip)
    {
        return trip.IdEstadoViaje switch
        {
            EstadoContraofertado => "Esperando al pasajero demo",
            EstadoAceptado => "Pedido aceptado",
            EstadoCaminoPasajero => "Ir a recoger pasajero",
            EstadoEnCurso => "Viaje en curso",
            _ => "Viaje activo"
        };
    }
}
