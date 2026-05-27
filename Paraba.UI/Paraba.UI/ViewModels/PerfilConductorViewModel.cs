namespace Paraba.UI.ViewModels
{
    public class PerfilConductorViewModel
    {
        public int IdConductor { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string TipoServicio { get; set; } = string.Empty;

        public string Vehiculo { get; set; } = string.Empty;

        public string Placa { get; set; } = string.Empty;

        public bool ConductorVerificado { get; set; }

        public bool VehiculoVerificado { get; set; }

        public bool Disponible { get; set; }

        public decimal PromedioCalificacion { get; set; }

        public int TotalCalificaciones { get; set; }
    }
}
