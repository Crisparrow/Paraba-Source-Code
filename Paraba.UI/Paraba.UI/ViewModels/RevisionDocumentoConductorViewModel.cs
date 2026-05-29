using System.ComponentModel.DataAnnotations;

namespace Paraba.UI.ViewModels
{
    public class RevisionDocumentoConductorViewModel
    {
        public int IdDocumentoConductor { get; set; }

        public string Conductor { get; set; } = string.Empty;

        public string TipoDocumento { get; set; } = string.Empty;

        public string NumeroDocumento { get; set; } = string.Empty;

        public string UrlArchivo { get; set; } = string.Empty;

        public DateTime? FechaVencimiento { get; set; }

        public string EstadoVerificacion { get; set; } = string.Empty;

        public string Observacion { get; set; } = string.Empty;

        public bool EstaVencido { get; set; }

        [StringLength(300, ErrorMessage = "La observacion no puede superar los 300 caracteres.")]
        public string ObservacionAprobacion { get; set; } = string.Empty;
    }
}
