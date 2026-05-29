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
    }
}
