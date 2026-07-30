namespace Paraba.ENTITY.Models
{
    public class DocumentoConductor
    {
        public int IdDocumentoConductor { get; set; }

        public int IdConductor { get; set; }

        public string TipoDocumento { get; set; } = string.Empty;

        public string NumeroDocumento { get; set; } = string.Empty;

        public string UrlArchivo { get; set; } = string.Empty;

        public DateTime? FechaVencimiento { get; set; }

        public string EstadoVerificacion { get; set; } = string.Empty;

        public string Observacion { get; set; } = string.Empty;

        public bool EsVigente { get; set; } = true;

        public DateTime FechaRegistro { get; set; }
    }
}
