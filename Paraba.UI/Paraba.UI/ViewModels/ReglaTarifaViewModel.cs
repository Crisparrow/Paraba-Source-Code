namespace Paraba.UI.ViewModels
{
    public class ReglaTarifaViewModel
    {
        public int IdReglaTarifa { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string TipoRegla { get; set; } = string.Empty;

        public string TipoServicio { get; set; } = string.Empty;

        public string Zona { get; set; } = string.Empty;

        public decimal PorcentajeIncremento { get; set; }

        public decimal MontoIncremento { get; set; }

        public string Horario { get; set; } = string.Empty;

        public int Prioridad { get; set; }

        public bool Estado { get; set; }
    }
}
