using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Paraba.DriverApp.Models;

namespace Paraba.DriverApp.Services;

public class TripService : IAsyncDisposable
{
    private readonly HttpClient _httpClient;
    private HubConnection? _hubConnection;
    private int? _realtimeDriverId;

    public TripService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(GetApiBaseUrl())
        };
    }

    public async Task<List<DriverTripResponse>> GetAvailableTripsAsync(int driverId)
    {
        return await _httpClient.GetFromJsonAsync<List<DriverTripResponse>>($"api/conductores/{driverId}/viajes/disponibles") ?? new();
    }

    public async Task<DriverOperationsSummaryResponse?> GetOperationsSummaryAsync(int driverId)
    {
        return await _httpClient.GetFromJsonAsync<DriverOperationsSummaryResponse>($"api/conductores/{driverId}/operacion/resumen");
    }

    public async Task<List<DriverTripResponse>> GetActiveTripsAsync(int driverId)
    {
        return await _httpClient.GetFromJsonAsync<List<DriverTripResponse>>($"api/conductores/{driverId}/viajes/activos") ?? new();
    }

    public Task AcceptTripAsync(int driverId, int tripId)
    {
        return PostAsync($"api/conductores/{driverId}/viajes/{tripId}/aceptar", null);
    }

    public Task CounterOfferAsync(int driverId, int tripId, decimal amount)
    {
        return PostAsync(
            $"api/conductores/{driverId}/viajes/{tripId}/contraoferta",
            new DriverCounterOfferRequest { TarifaContraoferta = amount });
    }

    public Task CancelTripAsync(int driverId, int tripId, string reason)
    {
        return PostAsync(
            $"api/conductores/{driverId}/viajes/{tripId}/cancelar",
            new DriverCancelTripRequest { Motivo = reason });
    }

    public Task AcceptCounterOfferAsPassengerDemoAsync(int driverId, int tripId)
    {
        return PostAsync($"api/conductores/{driverId}/viajes/{tripId}/demo/aceptar-contraoferta", null);
    }

    public Task CreateDemoTripAsync(int driverId, int? serviceTypeId = null)
    {
        return PostAsync(
            $"api/conductores/{driverId}/demo/viajes",
            new DriverDemoTripRequest { IdTipoServicio = serviceTypeId });
    }

    public async Task SetAvailabilityAsync(int driverId, bool available)
    {
        HttpResponseMessage response = await _httpClient.PutAsJsonAsync(
            $"api/conductores/{driverId}/disponibilidad",
            new DriverAvailabilityRequest { Disponible = available });

        if (response.IsSuccessStatusCode)
        {
            return;
        }

        string content = await response.Content.ReadAsStringAsync();
        string message = TryReadApiMessage(content);

        throw new InvalidOperationException(string.IsNullOrWhiteSpace(message)
            ? $"La API respondio con estado {(int)response.StatusCode}."
            : message);
    }

    public Task StartTripAsync(int driverId, int tripId)
    {
        return PostAsync($"api/conductores/{driverId}/viajes/{tripId}/iniciar", null);
    }

    public Task FinishTripAsync(int driverId, int tripId)
    {
        return PostAsync($"api/conductores/{driverId}/viajes/{tripId}/finalizar", null);
    }

    public async Task StartRealtimeAsync(int driverId, Func<Task> onTripChanged)
    {
        if (driverId <= 0)
        {
            throw new ArgumentException("El conductor no es valido.", nameof(driverId));
        }

        if (_hubConnection != null && _realtimeDriverId == driverId)
        {
            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }

            return;
        }

        await StopRealtimeAsync();

        _realtimeDriverId = driverId;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(new Uri(_httpClient.BaseAddress!, $"hubs/trips?idConductor={driverId}"))
            .WithAutomaticReconnect([TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)])
            .Build();

        _hubConnection.On<JsonElement>("TripChanged", async _ => await onTripChanged());
        await _hubConnection.StartAsync();
    }

    public async Task StopRealtimeAsync()
    {
        if (_hubConnection == null)
        {
            return;
        }

        await _hubConnection.DisposeAsync();
        _hubConnection = null;
        _realtimeDriverId = null;
    }

    public async ValueTask DisposeAsync()
    {
        await StopRealtimeAsync();
        _httpClient.Dispose();
    }

    private async Task PostAsync(string url, object? body)
    {
        HttpResponseMessage response = body == null
            ? await _httpClient.PostAsync(url, null)
            : await _httpClient.PostAsJsonAsync(url, body);

        if (response.IsSuccessStatusCode)
        {
            return;
        }

        string content = await response.Content.ReadAsStringAsync();
        string message = TryReadApiMessage(content);

        throw new InvalidOperationException(string.IsNullOrWhiteSpace(message)
            ? $"La API respondio con estado {(int)response.StatusCode}."
            : message);
    }

    private static string TryReadApiMessage(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return string.Empty;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(content);

            if (document.RootElement.TryGetProperty("mensaje", out JsonElement message))
            {
                return message.GetString() ?? string.Empty;
            }
        }
        catch (JsonException)
        {
            return content;
        }

        return content;
    }

    private static string GetApiBaseUrl()
    {
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            return "http://127.0.0.1:5183/";
        }

        return "http://localhost:5183/";
    }
}
