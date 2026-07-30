using Paraba.DriverApp.Models;
using Paraba.DriverApp.Services;

namespace Paraba.DriverApp.Views.Orders;

public partial class OrdersView : ContentView
{
    private readonly TripService _tripService = new();
    private readonly List<DriverTripResponse> _availableTrips = new();
    private DriverTripResponse? _activeTrip;
    private DriverOperationsSummaryResponse? _summary;
    private DriverTripResponse? _lastCompletedTrip;
    private int? _driverId;
    private bool _isProcessing;
    private bool _isRealtimeRefreshing;

    public OrdersView()
    {
        InitializeComponent();
        ActiveTripDetail.StartRequested += async (_, trip) => await StartTripAsync(trip);
        ActiveTripDetail.FinishRequested += async (_, trip) => await FinishTripAsync(trip);
        ActiveTripDetail.PassengerAcceptRequested += async (_, trip) => await AcceptCounterOfferAsPassengerDemoAsync(trip);
        Unloaded += async (_, _) => await _tripService.StopRealtimeAsync();
        RenderActiveTrip();
        RenderAvailableTrips();
    }

    public async Task LoadAsync(int? driverId)
    {
        _driverId = driverId;

        if (driverId == null)
        {
            await _tripService.StopRealtimeAsync();
            _activeTrip = null;
            _summary = null;
            _availableTrips.Clear();
            RenderSimulatedMap(null);
            RenderActiveTrip();
            RenderAvailableTrips();
            return;
        }

        try
        {
            await _tripService.StartRealtimeAsync(driverId.Value, RefreshFromRealtimeAsync);
        }
        catch
        {
            // La carga REST y la actualizacion manual siguen disponibles si SignalR esta temporalmente fuera de linea.
        }

        _summary = await LoadOperationsSummaryAsync(driverId.Value);
        List<DriverTripResponse> activeTrips = await _tripService.GetActiveTripsAsync(driverId.Value);
        List<DriverTripResponse> availableTrips = await _tripService.GetAvailableTripsAsync(driverId.Value);

        _activeTrip = activeTrips.FirstOrDefault();
        _availableTrips.Clear();
        _availableTrips.AddRange(availableTrips);

        RenderSimulatedMap(_activeTrip ?? _availableTrips.FirstOrDefault());
        RenderActiveTrip();
        RenderAvailableTrips();
    }

    private async void OnRefreshTripsClicked(object sender, EventArgs e)
    {
        if (_isProcessing)
        {
            return;
        }

        await RefreshAsync();
    }

    private async void OnToggleAvailabilityClicked(object sender, EventArgs e)
    {
        if (_isProcessing)
        {
            return;
        }

        if (_driverId == null)
        {
            await ShowAlertAsync("Inicia sesion con un conductor aprobado para cambiar disponibilidad.");
            return;
        }

        bool currentlyConnected = _summary?.Conectado == true;
        bool newAvailability = !currentlyConnected;

        bool confirmed = await ConfirmAsync(
            newAvailability ? "Conectarte" : "Desconectarte",
            newAvailability
                ? "Al conectarte podras recibir pedidos disponibles."
                : "Al desconectarte dejaras de recibir pedidos nuevos.",
            newAvailability ? "Conectar" : "Desconectar",
            "Volver");

        if (!confirmed)
        {
            return;
        }

        await ExecuteTripActionAsync(
            () => _tripService.SetAvailabilityAsync(_driverId.Value, newAvailability),
            newAvailability ? "Ya estas conectado para recibir pedidos." : "Te desconectaste correctamente.",
            "No se pudo cambiar la disponibilidad.");
    }

    private async void OnCreateDemoTripClicked(object sender, EventArgs e)
    {
        if (_isProcessing)
        {
            return;
        }

        if (_driverId == null)
        {
            await ShowAlertAsync("Inicia sesion con un conductor aprobado para crear pedidos demo.");
            return;
        }

        _lastCompletedTrip = null;
        RenderCompletionSummary();

        await ExecuteTripActionAsync(
            () => _tripService.CreateDemoTripAsync(_driverId.Value),
            "Pedido demo creado correctamente.",
            "No se pudo crear el pedido demo.");
    }

    private async Task StartTripAsync(DriverTripResponse trip)
    {
        if (_isProcessing)
        {
            return;
        }

        if (_driverId == null)
        {
            await ShowAlertAsync("No hay viaje activo para iniciar.");
            return;
        }

        bool confirmed = await ConfirmAsync(
            "Confirmar recogida",
            $"Confirma que ya recogiste al pasajero en: {trip.Origen}",
            "Si, iniciar viaje",
            "Volver");

        if (!confirmed)
        {
            return;
        }

        await ExecuteTripActionAsync(
            () => _tripService.StartTripAsync(_driverId.Value, trip.IdViaje),
            "Viaje iniciado correctamente.",
            "No se pudo iniciar el viaje.");
    }

    private async Task FinishTripAsync(DriverTripResponse trip)
    {
        if (_isProcessing)
        {
            return;
        }

        if (_driverId == null)
        {
            await ShowAlertAsync("No hay viaje activo para finalizar.");
            return;
        }

        bool confirmed = await ConfirmAsync(
            "Finalizar viaje",
            $"Confirma que llegaste al destino: {trip.Destino}",
            "Si, finalizar",
            "Volver");

        if (!confirmed)
        {
            return;
        }

        await ExecuteTripActionAsync(
            () => _tripService.FinishTripAsync(_driverId.Value, trip.IdViaje),
            "Viaje finalizado correctamente.",
            "No se pudo finalizar el viaje.",
            () =>
            {
                _lastCompletedTrip = trip;
                RenderCompletionSummary();
            });
    }

    private async Task AcceptTripAsync(DriverTripResponse trip)
    {
        if (_isProcessing)
        {
            return;
        }

        if (_driverId == null)
        {
            await ShowAlertAsync("Completa y espera la aprobacion de tu registro para aceptar viajes.");
            return;
        }

        bool confirmed = await ConfirmAsync(
            "Aceptar pedido",
            $"Aceptaras este pedido por Bs {GetTripFare(trip):0.00}. Luego deberas ir a recoger al pasajero.",
            "Aceptar",
            "Volver");

        if (!confirmed)
        {
            return;
        }

        await ExecuteTripActionAsync(
            () => _tripService.AcceptTripAsync(_driverId.Value, trip.IdViaje),
            "Viaje aceptado correctamente.",
            "No se pudo aceptar el viaje.");
    }

    private async Task CounterOfferTripAsync(DriverTripResponse trip)
    {
        if (_isProcessing)
        {
            return;
        }

        if (_driverId == null)
        {
            await ShowAlertAsync("Completa y espera la aprobacion de tu registro para contraofertar.");
            return;
        }

        string suggested = (trip.TarifaContraoferta ?? trip.TarifaOfertada).ToString("0.00");
        string amountText = await PromptAsync(
            "Contraoferta",
            "Ingresa la nueva tarifa en Bs.",
            "Enviar",
            "Cancelar",
            suggested,
            keyboard: Keyboard.Numeric);

        if (string.IsNullOrWhiteSpace(amountText))
        {
            return;
        }

        if (!decimal.TryParse(amountText.Replace(',', '.'), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
        {
            await ShowAlertAsync("Ingresa una tarifa valida mayor a cero.");
            return;
        }

        bool confirmed = await ConfirmAsync(
            "Enviar contraoferta",
            $"Enviar contraoferta por Bs {amount:0.00}. El pasajero demo debera aceptarla.",
            "Enviar",
            "Volver");

        if (!confirmed)
        {
            return;
        }

        await ExecuteTripActionAsync(
            () => _tripService.CounterOfferAsync(_driverId.Value, trip.IdViaje, amount),
            "Contraoferta enviada correctamente.",
            "No se pudo contraofertar.");
    }

    private async Task AcceptCounterOfferAsPassengerDemoAsync(DriverTripResponse trip)
    {
        if (_isProcessing)
        {
            return;
        }

        if (_driverId == null)
        {
            await ShowAlertAsync("No hay conductor activo para simular la respuesta del pasajero.");
            return;
        }

        bool confirmed = await ConfirmAsync(
            "Pasajero demo",
            "Simular que el pasajero acepta la contraoferta y pasar a recogerlo.",
            "Aceptar demo",
            "Volver");

        if (!confirmed)
        {
            return;
        }

        await ExecuteTripActionAsync(
            () => _tripService.AcceptCounterOfferAsPassengerDemoAsync(_driverId.Value, trip.IdViaje),
            "El pasajero demo acepto la contraoferta. Ve a recogerlo.",
            "No se pudo simular la aceptacion del pasajero.");
    }

    private async Task CancelTripAsync(DriverTripResponse trip)
    {
        if (_isProcessing)
        {
            return;
        }

        if (_driverId == null)
        {
            await ShowAlertAsync("Completa y espera la aprobacion de tu registro para cancelar viajes.");
            return;
        }

        string reason = await SelectCancellationReasonAsync();

        if (string.IsNullOrWhiteSpace(reason))
        {
            return;
        }

        bool confirmed = await ConfirmAsync(
            "Confirmar cancelacion",
            "Cancelar un viaje queda registrado en auditoria. Confirma solo si corresponde.",
            "Cancelar viaje",
            "Volver");

        if (!confirmed)
        {
            return;
        }

        await ExecuteTripActionAsync(
            () => _tripService.CancelTripAsync(_driverId.Value, trip.IdViaje, reason.Trim()),
            "Viaje cancelado correctamente.",
            "No se pudo cancelar el viaje.");
    }

    private async Task ExecuteTripActionAsync(
        Func<Task> action,
        string successMessage,
        string failurePrefix,
        Action? afterSuccess = null)
    {
        if (_isProcessing)
        {
            return;
        }

        try
        {
            SetProcessingState(true);
            await action();
            await RefreshAsync();
            afterSuccess?.Invoke();
            await ShowAlertAsync(successMessage);
        }
        catch (Exception ex)
        {
            await ShowAlertAsync($"{failurePrefix} Detalle: {ex.Message}");
        }
        finally
        {
            SetProcessingState(false);
        }
    }

    private async Task RefreshAsync()
    {
        await LoadAsync(_driverId);
    }

    private async Task RefreshFromRealtimeAsync()
    {
        if (_isRealtimeRefreshing || _isProcessing)
        {
            return;
        }

        _isRealtimeRefreshing = true;

        try
        {
            await MainThread.InvokeOnMainThreadAsync(RefreshAsync);
        }
        finally
        {
            _isRealtimeRefreshing = false;
        }
    }

    private void RenderActiveTrip()
    {
        if (_activeTrip != null && _lastCompletedTrip != null)
        {
            _lastCompletedTrip = null;
            RenderCompletionSummary();
        }

        ActiveTripDetail.Trip = _activeTrip;
        RenderHeaderState();
    }

    private void RenderAvailableTrips()
    {
        OrdersList.Children.Clear();
        AvailableTripsCountLabel.Text = (_summary?.PedidosDisponibles ?? _availableTrips.Count).ToString();
        RenderHeaderState();

        if (_availableTrips.Count == 0)
        {
            OrdersList.Children.Add(new Frame
            {
                CornerRadius = 18,
                HasShadow = false,
                Padding = new Thickness(18, 16),
                BackgroundColor = Color.FromArgb("#F8FAFC"),
                BorderColor = Color.FromArgb("#E5E7EB"),
                Content = new VerticalStackLayout
                {
                    Spacing = 12,
                    Children =
                    {
                        new ActivityIndicator
                        {
                            IsRunning = true,
                            Color = Color.FromArgb("#20C65A"),
                            WidthRequest = 34,
                            HeightRequest = 34,
                            HorizontalOptions = LayoutOptions.Start
                        },
                        new Label
                        {
                            Text = _summary?.Conectado == true ? "Buscando pedidos cercanos" : "Estas desconectado",
                            TextColor = Color.FromArgb("#111827"),
                            FontSize = 18,
                            FontAttributes = FontAttributes.Bold
                        },
                        new Label
                        {
                            Text = _summary?.Conectado == true
                                ? "Mantente disponible. Te avisaremos cuando entre una solicitud."
                                : "Toca Conectar para volver a recibir pedidos.",
                            TextColor = Color.FromArgb("#6B7280"),
                            FontSize = 13
                        },
                        new Label
                        {
                            Text = "Para demo, usa el boton + del mapa cuando quieras generar una solicitud.",
                            TextColor = Color.FromArgb("#374151"),
                            FontSize = 12
                        }
                    }
                }
            });

            return;
        }

        foreach (DriverTripResponse trip in _availableTrips)
        {
            TripRequestCard card = new TripRequestCard
            {
                Trip = trip
            };

            card.AcceptRequested += async (_, selectedTrip) => await AcceptTripAsync(selectedTrip);
            card.CounterOfferRequested += async (_, selectedTrip) => await CounterOfferTripAsync(selectedTrip);
            card.CancelRequested += async (_, selectedTrip) => await CancelTripAsync(selectedTrip);
            card.ExpiredRequested += async (_, selectedTrip) => await ExpireTripAsync(selectedTrip);

            OrdersList.Children.Add(card);
        }
    }

    private async Task ExpireTripAsync(DriverTripResponse trip)
    {
        if (_driverId == null || _isProcessing)
        {
            return;
        }

        try
        {
            SetProcessingState(true);
            await _tripService.CancelTripAsync(
                _driverId.Value,
                trip.IdViaje,
                "Pedido expirado por tiempo de respuesta del conductor.");
            await RefreshAsync();
        }
        catch
        {
            await RefreshAsync();
        }
        finally
        {
            SetProcessingState(false);
        }
    }

    private void RenderHeaderState()
    {
        bool isConnected = _driverId != null && (_summary?.Conectado ?? true);
        bool hasActiveTrip = _activeTrip != null;
        bool hasIncomingTrips = _availableTrips.Count > 0;

        CreateDemoTripButton.IsVisible = isConnected && !hasActiveTrip && !hasIncomingTrips && !_isProcessing;
        TopRefreshButton.Text = isConnected ? "Desconectar" : "Conectar";

        TopStatusBar.BackgroundColor = isConnected ? Color.FromArgb("#20C65A") : Color.FromArgb("#F23845");
        ConnectionIndicator.BackgroundColor = isConnected ? Colors.White : Color.FromArgb("#FEE2E2");
        ConnectionStatusLabel.Text = isConnected ? "Conectado" : "No conectado";
        WorkStatusLabel.TextColor = isConnected ? Color.FromArgb("#E8FFF0") : Color.FromArgb("#FEE2E2");

        if (!isConnected)
        {
            WorkStatusLabel.Text = _summary?.EstadoOperativo ?? "Completa tu registro para operar";
            PanelTitleLabel.Text = "Registro pendiente";
            PanelSubtitleLabel.Text = "Cuando seas aprobado podras recibir pedidos";
            MapHintLabel.Text = "Mapa simulado";
            RenderSimulatedMap(null);
            return;
        }

        if (hasActiveTrip)
        {
            DriverTripResponse activeTrip = _activeTrip!;

            WorkStatusLabel.Text = GetOperationalStep(activeTrip);
            PanelTitleLabel.Text = GetOperationalStep(activeTrip);
            PanelSubtitleLabel.Text = GetActiveTripSubtitle(activeTrip);
            MapHintLabel.Text = GetMapHint(activeTrip);
            RenderSimulatedMap(activeTrip);
            return;
        }

        if (hasIncomingTrips)
        {
            WorkStatusLabel.Text = $"{_availableTrips.Count} pedido(s) disponible(s)";
            PanelTitleLabel.Text = "Pedido entrante";
            PanelSubtitleLabel.Text = "Revisa la tarifa y decide rapidamente";
            MapHintLabel.Text = "Pedido cercano simulado";
            RenderSimulatedMap(_availableTrips.FirstOrDefault());
            return;
        }

        WorkStatusLabel.Text = "Sin pedidos por ahora";
        PanelTitleLabel.Text = "Esperando pedidos";
        PanelSubtitleLabel.Text = "Mantente conectado para recibir solicitudes";
        MapHintLabel.Text = "Mapa simulado";
        RenderSimulatedMap(null);
    }

    private async Task<DriverOperationsSummaryResponse?> LoadOperationsSummaryAsync(int driverId)
    {
        try
        {
            return await _tripService.GetOperationsSummaryAsync(driverId);
        }
        catch
        {
            return null;
        }
    }

    private void RenderCompletionSummary()
    {
        bool hasCompletedTrip = _lastCompletedTrip != null;
        TripCompletedSummary.IsVisible = hasCompletedTrip;

        if (!hasCompletedTrip)
        {
            CompletedRouteLabel.Text = string.Empty;
            CompletedFareLabel.Text = string.Empty;
            CompletedDurationLabel.Text = string.Empty;
            CompletedDistanceLabel.Text = string.Empty;
            return;
        }

        DriverTripResponse trip = _lastCompletedTrip!;
        CompletedRouteLabel.Text = $"{trip.Origen} -> {trip.Destino}";
        CompletedFareLabel.Text = $"Bs {GetTripFare(trip):0.00}";
        CompletedDurationLabel.Text = GetSimulatedDuration(trip);
        CompletedDistanceLabel.Text = GetSimulatedDistance(trip);
    }

    private void SetProcessingState(bool isProcessing)
    {
        _isProcessing = isProcessing;

        TopRefreshButton.IsEnabled = !isProcessing;
        MapRefreshButton.IsEnabled = !isProcessing;
        CreateDemoTripButton.IsEnabled = !isProcessing;
        ActiveTripDetail.IsEnabled = !isProcessing;
        OrdersList.IsEnabled = !isProcessing;

        if (isProcessing)
        {
            WorkStatusLabel.Text = "Procesando...";
        }
        else
        {
            RenderHeaderState();
        }
    }

    private void RenderSimulatedMap(DriverTripResponse? trip)
    {
        if (trip == null)
        {
            MapStateLabel.Text = "Esperando pedidos";
            MapEtaLabel.Text = "Zona PARABA activa";
            MapDistanceLabel.Text = "Sin ruta";
            MapHintLabel.Text = "Mapa simulado";
            RouteToPickupLine.Opacity = 0.25;
            RouteToDestinationLine.Opacity = 0.15;
            OriginPin.Opacity = 0.35;
            DestinationPin.Opacity = 0.35;
            _ = MoveDriverMarkerAsync(0, 0, "#111827");
            return;
        }

        switch (trip.IdEstadoViaje)
        {
            case 1:
                MapStateLabel.Text = "Pedido entrante";
                MapEtaLabel.Text = GetPickupEta(trip);
                MapDistanceLabel.Text = GetPickupDistance(trip);
                MapHintLabel.Text = "Pedido cercano simulado";
                RouteToPickupLine.Opacity = 1;
                RouteToDestinationLine.Opacity = 0.25;
                OriginPin.Opacity = 1;
                DestinationPin.Opacity = 0.65;
                _ = MoveDriverMarkerAsync(-58, 62, "#111827");
                break;
            case 6:
                MapStateLabel.Text = "Contraoferta enviada";
                MapEtaLabel.Text = "Esperando pasajero demo";
                MapDistanceLabel.Text = "Ruta reservada";
                MapHintLabel.Text = "Contraoferta enviada";
                RouteToPickupLine.Opacity = 0.7;
                RouteToDestinationLine.Opacity = 0.15;
                OriginPin.Opacity = 0.85;
                DestinationPin.Opacity = 0.45;
                _ = MoveDriverMarkerAsync(-30, 44, "#111827");
                break;
            case 7:
                MapStateLabel.Text = "Yendo al pasajero";
                MapEtaLabel.Text = "Llegas en 3 min";
                MapDistanceLabel.Text = "1.1 km al origen";
                MapHintLabel.Text = "Ruta al pasajero simulada";
                RouteToPickupLine.Opacity = 1;
                RouteToDestinationLine.Opacity = 0.18;
                OriginPin.Opacity = 1;
                DestinationPin.Opacity = 0.45;
                _ = MoveDriverMarkerAsync(-112, -68, "#20C65A");
                break;
            case 3:
                MapStateLabel.Text = "Yendo al destino";
                MapEtaLabel.Text = "Llegas en 12 min";
                MapDistanceLabel.Text = GetSimulatedDistance(trip);
                MapHintLabel.Text = "Ruta al destino simulada";
                RouteToPickupLine.Opacity = 0.35;
                RouteToDestinationLine.Opacity = 1;
                OriginPin.Opacity = 0.55;
                DestinationPin.Opacity = 1;
                _ = MoveDriverMarkerAsync(70, 76, "#F23845");
                break;
            case 4:
                MapStateLabel.Text = "Finalizado";
                MapEtaLabel.Text = "Viaje completado";
                MapDistanceLabel.Text = GetSimulatedDistance(trip);
                MapHintLabel.Text = "Destino alcanzado";
                RouteToPickupLine.Opacity = 0.2;
                RouteToDestinationLine.Opacity = 0.35;
                OriginPin.Opacity = 0.45;
                DestinationPin.Opacity = 1;
                _ = MoveDriverMarkerAsync(136, 118, "#20C65A");
                break;
            default:
                MapStateLabel.Text = GetOperationalStep(trip);
                MapEtaLabel.Text = "Ruta activa";
                MapDistanceLabel.Text = GetSimulatedDistance(trip);
                RouteToPickupLine.Opacity = 0.8;
                RouteToDestinationLine.Opacity = 0.5;
                OriginPin.Opacity = 1;
                DestinationPin.Opacity = 1;
                _ = MoveDriverMarkerAsync(0, 0, "#111827");
                break;
        }
    }

    private async Task MoveDriverMarkerAsync(double x, double y, string color)
    {
        DriverMarker.BackgroundColor = Color.FromArgb(color);

        try
        {
            await DriverMarker.TranslateTo(x, y, 450, Easing.CubicOut);
        }
        catch
        {
            DriverMarker.TranslationX = x;
            DriverMarker.TranslationY = y;
        }
    }

    private static decimal GetTripFare(DriverTripResponse trip)
    {
        return trip.TarifaAceptada
            ?? trip.TarifaContraoferta
            ?? (trip.TarifaOfertada > 0 ? trip.TarifaOfertada : trip.TarifaSugerida);
    }

    private static string GetOperationalStep(DriverTripResponse trip)
    {
        return trip.IdEstadoViaje switch
        {
            6 => "Esperando respuesta del pasajero",
            7 => "Ir a recoger pasajero",
            3 => "Viaje en curso",
            2 => "Aceptado",
            _ => string.IsNullOrWhiteSpace(trip.EstadoViaje) ? "Viaje activo" : trip.EstadoViaje
        };
    }

    private static string GetActiveTripSubtitle(DriverTripResponse trip)
    {
        return trip.IdEstadoViaje switch
        {
            6 => "Espera la respuesta del pasajero demo antes de moverte.",
            7 => $"Dirigete al origen: {trip.Origen}",
            3 => $"Lleva al pasajero hacia: {trip.Destino}",
            _ => $"{trip.Origen} -> {trip.Destino}"
        };
    }

    private static string GetMapHint(DriverTripResponse trip)
    {
        return trip.IdEstadoViaje switch
        {
            6 => "Contraoferta enviada",
            7 => "Ruta al pasajero simulada",
            3 => "Ruta al destino simulada",
            _ => "Ruta activa simulada"
        };
    }

    private static string GetSimulatedDuration(DriverTripResponse trip)
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

    private static string GetSimulatedDistance(DriverTripResponse trip)
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

    private async Task ShowAlertAsync(string message)
    {
        Page? page = Window?.Page ?? Application.Current?.Windows.FirstOrDefault()?.Page;

        if (page != null)
        {
            await page.DisplayAlert("PARABA", message, "Aceptar");
        }
    }

    private async Task<bool> ConfirmAsync(string title, string message, string accept, string cancel)
    {
        Page? page = Window?.Page ?? Application.Current?.Windows.FirstOrDefault()?.Page;

        if (page == null)
        {
            return false;
        }

        return await page.DisplayAlert(title, message, accept, cancel);
    }

    private async Task<string> PromptAsync(
        string title,
        string message,
        string accept,
        string cancel,
        string placeholder,
        Keyboard? keyboard = null)
    {
        Page? page = Window?.Page ?? Application.Current?.Windows.FirstOrDefault()?.Page;

        if (page == null)
        {
            return string.Empty;
        }

        return await page.DisplayPromptAsync(title, message, accept, cancel, placeholder, keyboard: keyboard) ?? string.Empty;
    }

    private async Task<string> SelectCancellationReasonAsync()
    {
        Page? page = Window?.Page ?? Application.Current?.Windows.FirstOrDefault()?.Page;

        if (page == null)
        {
            return string.Empty;
        }

        string selected = await page.DisplayActionSheet(
            "Motivo de rechazo/cancelacion",
            "Volver",
            null,
            "Pasajero no responde",
            "Ubicacion incorrecta",
            "Problema con el vehiculo",
            "Emergencia del conductor",
            "Direccion fuera de zona",
            "Otro motivo") ?? string.Empty;

        if (string.IsNullOrWhiteSpace(selected) || selected == "Volver")
        {
            return string.Empty;
        }

        if (selected != "Otro motivo")
        {
            return selected;
        }

        return await PromptAsync(
            "Otro motivo",
            "Escribe el motivo de cancelacion.",
            "Continuar",
            "Volver",
            "Motivo operativo del conductor");
    }

}


