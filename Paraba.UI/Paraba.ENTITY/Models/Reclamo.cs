namespace Paraba.ENTITY.Models
{
    public class Reclamo
    {
        public int IdReclamo { get; set; }
        public int? IdViaje { get; set; }
        public int? IdPasajero { get; set; }
        public int? IdConductor { get; set; }
        public string TipoReclamo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string UsuarioRegistro { get; set; } = string.Empty;
        public string UsuarioCierre { get; set; } = string.Empty;
        public string ObservacionCierre { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaCierre { get; set; }
    }
}
