namespace Paraba.UI.ViewModels
{
    public class ComisionServicioViewModel
    {
        public int IdComisionServicio { get; set; }

        public string TipoServicio { get; set; } = string.Empty;

        public decimal PorcentajeComision { get; set; }

        public DateTime FechaInicioVigencia { get; set; }

        public DateTime? FechaFinVigencia { get; set; }

        public bool Estado { get; set; }
    }
}
