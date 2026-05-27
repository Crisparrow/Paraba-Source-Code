namespace Paraba.UI.ViewModels
{
    public class ResponderContraofertaViewModel
    {
        public int IdViaje { get; set; }

        public string Pasajero { get; set; } = string.Empty;

        public string Conductor { get; set; } = string.Empty;

        public string Ruta { get; set; } = string.Empty;

        public decimal TarifaSugerida { get; set; }

        public decimal TarifaOfertada { get; set; }

        public decimal? TarifaContraoferta { get; set; }

        public decimal? TarifaAceptada { get; set; }
    }
}
