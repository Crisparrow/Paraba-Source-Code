namespace Paraba.ENTITY.Models
{
    public class Calificacion
    {
        public int IdCalificacion { get; set; }

        public int IdViaje { get; set; }

        public int IdPasajero { get; set; }

        public int IdConductor { get; set; }

        public int Puntaje { get; set; }

        public string Comentario { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
