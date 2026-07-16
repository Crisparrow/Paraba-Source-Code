namespace Paraba.ENTITY.Models
{
    public class ResumenOperacionConductor
    {
        public int IdConductor { get; set; }

        public bool Conectado { get; set; }

        public int Prioridad { get; set; }

        public int PedidosDisponibles { get; set; }

        public int ViajesActivos { get; set; }

        public int ViajesHoy { get; set; }

        public int ViajesFinalizadosHoy { get; set; }

        public decimal GananciaHoy { get; set; }

        public string ObjetivoTitulo { get; set; } = string.Empty;

        public string ObjetivoDetalle { get; set; } = string.Empty;

        public decimal ObjetivoActual { get; set; }

        public decimal ObjetivoMeta { get; set; }

        public string EstadoOperativo { get; set; } = string.Empty;
    }
}
