namespace Paraba.UI.ViewModels
{
    public class ZonaViewModel
    {
        public int IdZona { get; set; }

        public string Ciudad { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public bool CoberturaActiva { get; set; }

        public bool EsZonaRiesgo { get; set; }

        public bool AltaDemanda { get; set; }

        public string ObservacionOperativa { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }
    }
}
