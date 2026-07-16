namespace Paraba.ENTITY.Models
{
    public class Viaje
    {
        public int IdViaje { get; set; }

        public int IdPasajero { get; set; }

        public int IdConductor { get; set; }

        public int IdVehiculo { get; set; }

        public int IdTipoServicio { get; set; }

        public string TipoServicio { get; set; } = string.Empty;

        public string Origen { get; set; } = string.Empty;

        public string Destino { get; set; } = string.Empty;

        public decimal TarifaEstimada { get; set; }

        public decimal TarifaFinal { get; set; }

        public decimal TarifaSugerida { get; set; }

        public decimal TarifaOfertada { get; set; }

        public decimal? TarifaContraoferta { get; set; }

        public decimal? TarifaAceptada { get; set; }

        public int IdEstadoViaje { get; set; }

        public string EstadoViaje { get; set; } = string.Empty;

        public DateTime FechaSolicitud { get; set; }

        public DateTime? FechaAceptacion { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public DateTime? FechaCancelacion { get; set; }

        public string MotivoCancelacion { get; set; } = string.Empty;
    }
}
