namespace Paraba.UI.ViewModels
{
    public class CalificacionViewModel
    {
        public int IdCalificacion { get; set; }

        public int IdViaje { get; set; }

        public string Pasajero { get; set; } = string.Empty;

        public string Conductor { get; set; } = string.Empty;

        public int Puntaje { get; set; }

        public string Comentario { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
