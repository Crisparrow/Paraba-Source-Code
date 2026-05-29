namespace Paraba.UI.ViewModels
{
    public class ReporteConductorItemViewModel
    {
        public int IdConductor { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public bool Verificado { get; set; }

        public bool Disponible { get; set; }

        public bool Estado { get; set; }

        public int TotalViajes { get; set; }

        public int ViajesFinalizados { get; set; }

        public int ViajesCancelados { get; set; }

        public decimal IngresosFinalizados { get; set; }

        public decimal PromedioCalificacion { get; set; }
    }
}
