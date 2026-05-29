namespace Paraba.UI.ViewModels
{
    public class ReporteViajeItemViewModel
    {
        public int IdViaje { get; set; }

        public string Pasajero { get; set; } = string.Empty;

        public string Conductor { get; set; } = string.Empty;

        public string TipoServicio { get; set; } = string.Empty;

        public string Ruta { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public decimal TarifaSugerida { get; set; }

        public decimal TarifaOfertada { get; set; }

        public decimal? TarifaAceptada { get; set; }

        public decimal TarifaFinal { get; set; }

        public DateTime FechaSolicitud { get; set; }
    }
}
