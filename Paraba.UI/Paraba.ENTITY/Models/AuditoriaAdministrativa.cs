namespace Paraba.ENTITY.Models
{
    public class AuditoriaAdministrativa
    {
        public int IdAuditoriaAdministrativa { get; set; }
        public string Modulo { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string Entidad { get; set; } = string.Empty;
        public int? IdEntidad { get; set; }
        public string UsuarioSistema { get; set; } = string.Empty;
        public string Observacion { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }
}
