using System.Net.Http.Json;
using Paraba.DriverApp.Models;

namespace Paraba.DriverApp.Services;

public class DriverApiService
{
    private readonly HttpClient _httpClient;
    private string _sessionToken = string.Empty;

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

    public async Task<DriverRequestCodeResponse?> RequestCodeAsync(string phone)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "api/conductores/auth/solicitar-codigo",
            new DriverRequestCodeRequest { Telefono = phone });

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<DriverRequestCodeResponse>();
    }

    public async Task<DriverVerifyCodeResponse?> VerifyCodeAsync(string phone, string code)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "api/conductores/auth/verificar-codigo",
            new DriverVerifyCodeRequest { Telefono = phone, Codigo = code });

        response.EnsureSuccessStatusCode();

        DriverVerifyCodeResponse? result = await response.Content.ReadFromJsonAsync<DriverVerifyCodeResponse>();
        _sessionToken = result?.Token ?? string.Empty;

        return result;
    }

    public async Task<DriverRegistrationResponse?> SaveRegistrationDraftAsync(DriverRegistrationDraftRequest request)
    {
        using HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, "api/conductores/auth/solicitud/borrador")
        {
            Content = JsonContent.Create(request)
        };

        AddAuthorization(message);

        HttpResponseMessage response = await _httpClient.SendAsync(message);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<DriverRegistrationResponse>();
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
            return "http://127.0.0.1:5183/";
        }

        return "http://localhost:5183/";
    }

    private void AddAuthorization(HttpRequestMessage message)
    {
        if (!string.IsNullOrWhiteSpace(_sessionToken))
        {
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _sessionToken);
        }
    }
}

