namespace Paraba.UI.ViewModels
{
    public class RechazarDocumentoConductorViewModel
    {
        public int IdDocumentoConductor { get; set; }

        public string Conductor { get; set; } = string.Empty;

        public string TipoDocumento { get; set; } = string.Empty;

        public string NumeroDocumento { get; set; } = string.Empty;

        public string MotivoRechazo { get; set; } = string.Empty;
    }
}
