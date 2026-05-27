namespace Paraba.UI.ViewModels
{
    public class TarifaViewModel
    {
        public int IdTarifa { get; set; }

        public string TipoServicio { get; set; } = string.Empty;

        public decimal TarifaBase { get; set; }

        public decimal CostoPorKilometro { get; set; }

        public decimal CostoPorMinuto { get; set; }

        public decimal TarifaMinima { get; set; }

        public bool Estado { get; set; }
    }
}
