namespace Paraba.ENTITY.Models
{
    public class Zona
    {
        public int IdZona { get; set; }

        public int IdCiudad { get; set; }

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
