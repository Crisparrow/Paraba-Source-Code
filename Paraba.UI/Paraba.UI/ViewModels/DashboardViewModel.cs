namespace Paraba.UI.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalViajes { get; set; }

        public int ViajesSolicitados { get; set; }

        public int ViajesAceptados { get; set; }

        public int ViajesEnCurso { get; set; }

        public int ViajesFinalizados { get; set; }

        public int ViajesCancelados { get; set; }

        public int ViajesHoy { get; set; }

        public int ViajesFinalizadosHoy { get; set; }

        public int ViajesCanceladosHoy { get; set; }

        public decimal IngresosFinalizados { get; set; }

        public decimal IngresosHoy { get; set; }

        public int ConductoresActivos { get; set; }

        public int ConductoresVerificados { get; set; }

        public int ConductoresPendientes { get; set; }

        public int DocumentosPendientes { get; set; }

        public int DocumentosRechazados { get; set; }

        public int DocumentosVencidos { get; set; }

        public int PasajerosActivos { get; set; }

        public decimal PromedioCalificacion { get; set; }

        public int ViajesPendientesLiquidacion { get; set; }

        public decimal MontoPendienteLiquidacion { get; set; }

        public int LiquidacionesPendientesPago { get; set; }

        public decimal NetoPendientePago { get; set; }

        public int TotalAlertasOperativas =>
            ConductoresPendientes +
            DocumentosPendientes +
            DocumentosVencidos +
            ViajesPendientesLiquidacion +
            LiquidacionesPendientesPago;

        public decimal PorcentajeViajesFinalizados =>
            TotalViajes == 0 ? 0 : Math.Round((decimal)ViajesFinalizados * 100 / TotalViajes, 1);

        public decimal PorcentajeViajesCancelados =>
            TotalViajes == 0 ? 0 : Math.Round((decimal)ViajesCancelados * 100 / TotalViajes, 1);

        public decimal PorcentajeConductoresVerificados =>
            ConductoresActivos == 0 ? 0 : Math.Round((decimal)ConductoresVerificados * 100 / ConductoresActivos, 1);

        public string EstadoOperativo
        {
            get
            {
                if (DocumentosVencidos > 0 || LiquidacionesPendientesPago > 0)
                {
                    return "Atencion requerida";
                }

                if (ConductoresPendientes > 0 || DocumentosPendientes > 0 || ViajesPendientesLiquidacion > 0)
                {
                    return "Operacion con pendientes";
                }

                return "Operacion estable";
            }
        }

        public string EstadoOperativoCss =>
            EstadoOperativo == "Operacion estable"
                ? "status-success"
                : EstadoOperativo == "Operacion con pendientes"
                    ? "status-warning"
                    : "status-danger";
    }
}
