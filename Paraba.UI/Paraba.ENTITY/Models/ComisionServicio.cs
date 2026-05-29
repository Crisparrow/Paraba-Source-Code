namespace Paraba.ENTITY.Models
{
    public class ComisionServicio
    {
        public int IdComisionServicio { get; set; }

        public int IdTipoServicio { get; set; }

        public decimal PorcentajeComision { get; set; }

        public DateTime FechaInicioVigencia { get; set; }

        public DateTime? FechaFinVigencia { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
