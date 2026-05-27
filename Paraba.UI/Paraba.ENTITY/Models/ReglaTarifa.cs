namespace Paraba.ENTITY.Models
{
    public class ReglaTarifa
    {
        public int IdReglaTarifa { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string TipoRegla { get; set; } = string.Empty;

        public int? IdTipoServicio { get; set; }

        public int? IdZona { get; set; }

        public decimal PorcentajeIncremento { get; set; }

        public decimal MontoIncremento { get; set; }

        public TimeSpan? HoraInicio { get; set; }

        public TimeSpan? HoraFin { get; set; }

        public int Prioridad { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
