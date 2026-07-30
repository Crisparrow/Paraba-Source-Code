using Paraba.DriverApp.Models;
using Paraba.DriverApp.Services;

namespace Paraba.DriverApp.Views.Profile;

public partial class ProfilePage : ContentView
{
    private readonly DriverProfileService service = new();
    private int? driverId;

    public ProfilePage()
    {
        InitializeComponent();
        RenderEmpty();
    }

    public async Task LoadAsync(int? idConductor)
    {
        driverId = idConductor;
        if (idConductor == null)
        {
            ShowRegistrationPreview("Conductor PARABA", "Completa el registro inicial para crear tu perfil.");
            return;
        }

        try
        {
            DriverProfileResponse? profile = await service.GetProfileAsync(idConductor.Value);
            List<DriverVehicleResponse> vehicles = await service.GetVehiclesAsync(idConductor.Value);
            List<DriverDocumentResponse> documents = await service.GetDocumentsAsync(idConductor.Value);

            DriverNameLabel.Text = profile?.NombreCompleto ?? "Conductor PARABA";
            bool approved = profile?.PuedeTrabajar == true;
            ApprovalStatusLabel.Text = approved ? "Aprobado" : profile?.EstadoAprobacion ?? "Pendiente";
            ApprovalStatusLabel.TextColor = approved ? Color.FromArgb("#15803D") : Color.FromArgb("#D97706");
            EligibilityLabel.Text = approved
                ? "Perfil, documentos y vehículo aprobados. Puedes recibir viajes."
                : BuildPendingMessage(vehicles, documents);
            RenderVehicles(vehicles);
            RenderDocuments(documents);
        }
        catch (Exception ex)
        {
            await ShowAlertAsync("Perfil", $"No se pudo actualizar el perfil: {ex.Message}");
        }
    }

    public void ShowRegistrationPreview(string name, string message)
    {
        driverId = null;
        DriverNameLabel.Text = name;
        ApprovalStatusLabel.Text = "Registro incompleto";
        ApprovalStatusLabel.TextColor = Color.FromArgb("#DC2626");
        EligibilityLabel.Text = message;
        RenderEmpty();
    }

    private async void OnRefreshClicked(object sender, EventArgs e) => await LoadAsync(driverId);

    private async void OnAddVehicleClicked(object sender, EventArgs e)
    {
        if (!await EnsureDriverAsync()) return;
        await Shell.Current.Navigation.PushAsync(new AddVehiclePage(driverId!.Value, async () => await LoadAsync(driverId)));
    }

    private async void OnUploadDocumentClicked(object sender, EventArgs e)
    {
        if (!await EnsureDriverAsync()) return;
        await Shell.Current.Navigation.PushAsync(new UploadDocumentsPage(driverId!.Value, false, async () => await LoadAsync(driverId)));
    }

    private async void OnPhotoVerificationClicked(object sender, EventArgs e)
    {
        if (!await EnsureDriverAsync()) return;
        await Shell.Current.Navigation.PushAsync(new UploadDocumentsPage(driverId!.Value, true, async () => await LoadAsync(driverId)));
    }

    private async Task<bool> EnsureDriverAsync()
    {
        if (driverId != null) return true;
        await ShowAlertAsync("Perfil", "Primero completa el registro inicial del conductor.");
        return false;
    }

    private void RenderVehicles(IEnumerable<DriverVehicleResponse> vehicles)
    {
        VehiclesContainer.Children.Clear();
        foreach (DriverVehicleResponse vehicle in vehicles)
        {
            VehiclesContainer.Children.Add(CreateStatusCard(
                $"{vehicle.Marca} {vehicle.Modelo} · {vehicle.Placa}",
                $"{vehicle.TipoServicio} · {vehicle.Anio}",
                vehicle.EstadoVerificacion,
                vehicle.Observacion));
        }
        if (VehiclesContainer.Children.Count == 0)
            VehiclesContainer.Children.Add(CreateEmptyLabel("Todavía no registraste un vehículo."));
    }

    private void RenderDocuments(IEnumerable<DriverDocumentResponse> documents)
    {
        DocumentsContainer.Children.Clear();
        foreach (DriverDocumentResponse document in documents)
        {
            DocumentsContainer.Children.Add(CreateStatusCard(
                FriendlyDocumentName(document.TipoDocumento),
                string.IsNullOrWhiteSpace(document.NumeroDocumento) ? "Verificación por foto" : document.NumeroDocumento,
                document.EstadoVerificacion,
                document.Observacion));
        }
        if (DocumentsContainer.Children.Count == 0)
            DocumentsContainer.Children.Add(CreateEmptyLabel("Todavía no cargaste documentos."));
    }

    private void RenderEmpty()
    {
        VehiclesContainer.Children.Clear();
        DocumentsContainer.Children.Clear();
        VehiclesContainer.Children.Add(CreateEmptyLabel("Registra tu vehículo desde esta pantalla."));
        DocumentsContainer.Children.Add(CreateEmptyLabel("Carga identidad, licencia, foto y documento del vehículo."));
    }

    private static View CreateStatusCard(string title, string subtitle, string status, string observation)
    {
        Color statusColor = status == "Aprobado" ? Color.FromArgb("#15803D")
            : status == "Rechazado" ? Color.FromArgb("#DC2626")
            : Color.FromArgb("#D97706");
        VerticalStackLayout content = new() { Spacing = 4 };
        content.Children.Add(new Label { Text = title, FontSize = 17, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#111827") });
        content.Children.Add(new Label { Text = subtitle, FontSize = 14, TextColor = Color.FromArgb("#6B7280") });
        content.Children.Add(new Label { Text = status, FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = statusColor });
        if (!string.IsNullOrWhiteSpace(observation))
            content.Children.Add(new Label { Text = observation, FontSize = 13, TextColor = Color.FromArgb("#4B5563") });
        return new Border { BackgroundColor = Colors.White, Stroke = Color.FromArgb("#E5E7EB"), Padding = 15, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 }, Content = content };
    }

    private static Label CreateEmptyLabel(string text) => new() { Text = text, TextColor = Color.FromArgb("#6B7280"), FontSize = 15 };

    private static string BuildPendingMessage(IReadOnlyCollection<DriverVehicleResponse> vehicles, IReadOnlyCollection<DriverDocumentResponse> documents)
    {
        if (!vehicles.Any()) return "Registra un vehículo. Todo vehículo nuevo entra como Pendiente.";
        if (vehicles.Any(item => item.EstadoVerificacion == "Rechazado")) return "Tu vehículo fue rechazado. Revisa la observación y registra la corrección.";
        if (documents.Any(item => item.EstadoVerificacion == "Rechazado")) return "Tienes documentos rechazados. Carga una versión corregida.";
        return "La administración debe aprobar el vehículo y todos los documentos obligatorios.";
    }

    private static string FriendlyDocumentName(string value) => value switch
    {
        "CedulaIdentidad" => "Cédula de identidad",
        "LicenciaConducir" => "Licencia de conducir",
        "FotoVerificacion" => "Verificación por foto",
        "DocumentoVehiculo" => "Documento del vehículo",
        "DocumentoMicrobus" => "Documento del micro/microbús",
        _ => value
    };

    private static Task ShowAlertAsync(string title, string message) =>
        Application.Current?.Windows.FirstOrDefault()?.Page?.DisplayAlert(title, message, "Aceptar") ?? Task.CompletedTask;
}
