namespace Paraba.UI.ViewModels
{
    public class LiquidacionDetalleItemViewModel
    {
        public int IdViaje { get; set; }

        public string Pasajero { get; set; } = string.Empty;

        public string TipoServicio { get; set; } = string.Empty;

        public string Ruta { get; set; } = string.Empty;

        public decimal TarifaFinal { get; set; }

        public decimal ComisionParaba { get; set; }

        public decimal NetoConductor { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
