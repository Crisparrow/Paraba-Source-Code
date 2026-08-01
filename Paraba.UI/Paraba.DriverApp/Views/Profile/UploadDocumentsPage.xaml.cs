using Paraba.DriverApp.Services;

namespace Paraba.DriverApp.Views.Profile;

public partial class UploadDocumentsPage : ContentPage
{
    private static readonly string[] DocumentTypes =
    {
        "CedulaIdentidad", "LicenciaConducir", "FotoVerificacion", "DocumentoVehiculo", "RUAT", "SOAT", "DocumentoMicrobus"
    };

    private readonly DriverProfileService service = new();
    private readonly int driverId;
    private readonly bool photoMode;
    private readonly Func<Task>? completed;
    private FileResult? selectedFile;

    public UploadDocumentsPage() : this(0, false, null) { }

    public UploadDocumentsPage(int driverId, bool photoMode, Func<Task>? completed)
    {
        InitializeComponent();
        this.driverId = driverId;
        this.photoMode = photoMode;
        this.completed = completed;
        DocumentTypePicker.ItemsSource = DocumentTypes;
        ExpirationDatePicker.MinimumDate = DateTime.Today.AddDays(1);
        ExpirationDatePicker.Date = DateTime.Today.AddYears(1);
        if (photoMode)
        {
            DocumentTypePicker.SelectedItem = "FotoVerificacion";
            DocumentTypePicker.IsEnabled = false;
            DocumentNumberEntry.Text = $"SELFIE-{DateTime.UtcNow:yyyyMMdd}";
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (photoMode && selectedFile == null)
            await CapturePhotoAsync();
    }

    private void OnExpirationChanged(object sender, CheckedChangedEventArgs e)
    {
        ExpirationDateLabel.IsVisible = e.Value;
        ExpirationDatePicker.IsVisible = e.Value;
    }

    private async void OnPickFileClicked(object sender, EventArgs e)
    {
        selectedFile = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Selecciona el documento" });
        SelectedFileLabel.Text = selectedFile?.FileName ?? "Ningún archivo seleccionado";
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e) => await CapturePhotoAsync();

    private async Task CapturePhotoAsync()
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await DisplayAlert("Cámara", "Este dispositivo no permite capturar fotos desde la app.", "Aceptar");
                return;
            }
            selectedFile = await MediaPicker.Default.CapturePhotoAsync();
            SelectedFileLabel.Text = selectedFile?.FileName ?? "Ninguna foto seleccionada";
        }
        catch (PermissionException)
        {
            await DisplayAlert("Cámara", "Autoriza el uso de la cámara para completar la verificación.", "Aceptar");
        }
    }

    private async void OnUploadClicked(object sender, EventArgs e)
    {
        if (DocumentTypePicker.SelectedItem is not string documentType || selectedFile == null)
        {
            await DisplayAlert("Documentos", "Selecciona el tipo y adjunta un archivo.", "Aceptar");
            return;
        }

        try
        {
            UploadButton.IsEnabled = false;
            string number = documentType == "FotoVerificacion"
                ? $"SELFIE-{DateTime.UtcNow:yyyyMMddHHmmss}"
                : DocumentNumberEntry.Text ?? string.Empty;
            DateTime? expiration = HasExpirationCheckBox.IsChecked ? ExpirationDatePicker.Date : null;
            await service.UploadDocumentAsync(driverId, documentType, number, expiration, selectedFile);
            await DisplayAlert("PARABA", "Archivo guardado y enviado a revisión.", "Aceptar");
            if (completed != null) await completed();
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Documentos", ex.Message, "Aceptar");
        }
        finally
        {
            UploadButton.IsEnabled = true;
        }
    }
}
