using System.Net.Http.Json;
using Paraba.DriverApp.Models;

namespace Paraba.DriverApp.Services;

public class DriverApiService
{
    private readonly HttpClient _httpClient;

    public DriverApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(GetApiBaseUrl())
        };
    }

    public async Task<DriverProfileResponse?> GetProfileAsync(int driverId)
    {
        return await _httpClient.GetFromJsonAsync<DriverProfileResponse>($"api/conductores/{driverId}/perfil");
    }

    public async Task<List<DriverTripResponse>> GetActiveTripsAsync(int driverId)
    {
        return await _httpClient.GetFromJsonAsync<List<DriverTripResponse>>($"api/conductores/{driverId}/viajes/activos") ?? new();
    }

    public async Task StartTripAsync(int driverId, int tripId)
    {
        HttpResponseMessage response = await _httpClient.PostAsync($"api/conductores/{driverId}/viajes/{tripId}/iniciar", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task FinishTripAsync(int driverId, int tripId)
    {
        HttpResponseMessage response = await _httpClient.PostAsync($"api/conductores/{driverId}/viajes/{tripId}/finalizar", null);
        response.EnsureSuccessStatusCode();
    }

    private static string GetApiBaseUrl()
    {
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            return "http://10.0.2.2:5183/";
        }

        return "http://localhost:5183/";
    }
}
