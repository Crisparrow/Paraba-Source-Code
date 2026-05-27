namespace Paraba.ENTITY.Models
{
    public class AuditoriaConductor
    {
        public int IdAuditoriaConductor { get; set; }

        public int IdConductor { get; set; }

        public string Accion { get; set; } = string.Empty;

        public string EstadoAnterior { get; set; } = string.Empty;

        public string EstadoNuevo { get; set; } = string.Empty;

        public string UsuarioSistema { get; set; } = string.Empty;

        public string Observacion { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }
    }
}
