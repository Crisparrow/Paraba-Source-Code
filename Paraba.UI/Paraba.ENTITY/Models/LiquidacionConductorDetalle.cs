namespace Paraba.ENTITY.Models
{
    public class LiquidacionConductorDetalle
    {
        public int IdLiquidacionConductorDetalle { get; set; }

        public int IdLiquidacionConductor { get; set; }

        public int IdViaje { get; set; }

        public decimal TarifaFinal { get; set; }

        public decimal ComisionParaba { get; set; }

        public decimal NetoConductor { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
