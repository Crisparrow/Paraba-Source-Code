namespace Paraba.ENTITY.Models
{
    public class Tarifa
    {
        public int IdTarifa { get; set; }

        public int IdTipoServicio { get; set; }

        public decimal TarifaBase { get; set; }

        public decimal CostoPorKilometro { get; set; }

        public decimal CostoPorMinuto { get; set; }

        public decimal TarifaMinima { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
