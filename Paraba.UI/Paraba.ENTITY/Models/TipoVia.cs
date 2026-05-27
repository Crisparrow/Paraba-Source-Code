namespace Paraba.ENTITY.Models
{
    public class TipoVia
    {
        public int IdTipoVia { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public decimal PorcentajeIncremento { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
