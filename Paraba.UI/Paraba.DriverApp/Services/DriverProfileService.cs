using System.Net.Http.Json;
using System.Text.Json;
using Paraba.DriverApp.Models;

namespace Paraba.DriverApp.Services;

public sealed class DriverProfileService
{
    private readonly HttpClient httpClient = new() { BaseAddress = new Uri(GetApiBaseUrl()) };

    public Task<DriverProfileResponse?> GetProfileAsync(int driverId) =>
        httpClient.GetFromJsonAsync<DriverProfileResponse>($"api/conductores/{driverId}/perfil");

    public async Task<List<DriverVehicleResponse>> GetVehiclesAsync(int driverId) =>
        await httpClient.GetFromJsonAsync<List<DriverVehicleResponse>>($"api/conductores/{driverId}/vehiculos") ?? new();

    public async Task<List<DriverDocumentResponse>> GetDocumentsAsync(int driverId) =>
        await httpClient.GetFromJsonAsync<List<DriverDocumentResponse>>($"api/conductores/{driverId}/documentos") ?? new();

    public async Task<List<DriverServiceTypeResponse>> GetServiceTypesAsync() =>
        await httpClient.GetFromJsonAsync<List<DriverServiceTypeResponse>>("api/tipos-servicio") ?? new();

    public async Task CreateVehicleAsync(int driverId, DriverVehicleCreateRequest request)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"api/conductores/{driverId}/vehiculos", request);
        await EnsureSuccessAsync(response);
    }

    public async Task UploadDocumentAsync(
        int driverId,
        string documentType,
        string documentNumber,
        DateTime? expirationDate,
        FileResult file)
    {
        await using Stream stream = await file.OpenReadAsync();
        using MultipartFormDataContent content = new();
        using StreamContent fileContent = new(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(GetContentType(file.FileName));
        content.Add(new StringContent(documentType), "TipoDocumento");
        content.Add(new StringContent(documentNumber ?? string.Empty), "NumeroDocumento");
        if (expirationDate != null)
        {
            content.Add(new StringContent(expirationDate.Value.ToString("yyyy-MM-dd")), "FechaVencimiento");
        }
        content.Add(fileContent, "Archivo", file.FileName);

        HttpResponseMessage response = await httpClient.PostAsync($"api/conductores/{driverId}/documentos", content);
        await EnsureSuccessAsync(response);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        string content = await response.Content.ReadAsStringAsync();
        try
        {
            using JsonDocument json = JsonDocument.Parse(content);
            if (json.RootElement.TryGetProperty("mensaje", out JsonElement message))
            {
                throw new InvalidOperationException(message.GetString());
            }
        }
        catch (JsonException)
        {
        }

        throw new InvalidOperationException($"La API respondio {(int)response.StatusCode}: {content}");
    }

    private static string GetContentType(string fileName) => Path.GetExtension(fileName).ToLowerInvariant() switch
    {
        ".png" => "image/png",
        ".pdf" => "application/pdf",
        _ => "image/jpeg"
    };

    private static string GetApiBaseUrl() => DeviceInfo.Platform == DevicePlatform.Android
        ? "http://127.0.0.1:5183/"
        : "http://localhost:5183/";
}
