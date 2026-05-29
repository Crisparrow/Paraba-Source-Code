namespace Paraba.UI.ViewModels
{
    public class SoporteCasoViewModel
    {
        public string TipoCaso { get; set; } = string.Empty;

        public int? IdViaje { get; set; }

        public string Pasajero { get; set; } = string.Empty;

        public string Conductor { get; set; } = string.Empty;

        public string Ruta { get; set; } = string.Empty;

        public string Detalle { get; set; } = string.Empty;

        public string Prioridad { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }
    }
}
