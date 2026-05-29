namespace Paraba.UI.ViewModels
{
    public class LiquidacionItemViewModel
    {
        public int IdViaje { get; set; }

        public string Conductor { get; set; } = string.Empty;

        public string Pasajero { get; set; } = string.Empty;

        public string TipoServicio { get; set; } = string.Empty;

        public string Ruta { get; set; } = string.Empty;

        public decimal TarifaFinal { get; set; }

        public decimal PorcentajeComision { get; set; }

        public decimal ComisionParaba { get; set; }

        public decimal NetoConductor { get; set; }

        public DateTime FechaFin { get; set; }
    }
}
