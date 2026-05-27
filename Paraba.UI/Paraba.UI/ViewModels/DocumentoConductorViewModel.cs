namespace Paraba.UI.ViewModels
{
    public class DocumentoConductorViewModel
    {
        public int IdDocumentoConductor { get; set; }

        public string Conductor { get; set; } = string.Empty;

        public string TipoDocumento { get; set; } = string.Empty;

        public string NumeroDocumento { get; set; } = string.Empty;

        public string UrlArchivo { get; set; } = string.Empty;

        public DateTime? FechaVencimiento { get; set; }

        public string EstadoVerificacion { get; set; } = string.Empty;

        public string Observacion { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }
    }
}
