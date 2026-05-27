namespace Paraba.UI.ViewModels
{
    public class AuditoriaViajeViewModel
    {
        public int IdAuditoriaViaje { get; set; }

        public int IdViaje { get; set; }

        public string Accion { get; set; } = string.Empty;

        public string EstadoAnterior { get; set; } = string.Empty;

        public string EstadoNuevo { get; set; } = string.Empty;

        public decimal? TarifaAnterior { get; set; }

        public decimal? TarifaNueva { get; set; }

        public string UsuarioSistema { get; set; } = string.Empty;

        public string Observacion { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }
    }
}
