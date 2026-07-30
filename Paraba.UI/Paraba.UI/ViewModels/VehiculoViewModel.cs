namespace Paraba.UI.ViewModels
{
    public class VehiculoViewModel
    {
        public int IdVehiculo { get; set; }

        public string Conductor { get; set; } = string.Empty;

        public string TipoServicio { get; set; } = string.Empty;

        public string Placa { get; set; } = string.Empty;

        public string Marca { get; set; } = string.Empty;

        public string Modelo { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public int Anio { get; set; }

        public bool Verificado { get; set; }

        public string EstadoVerificacion { get; set; } = string.Empty;

        public string Observacion { get; set; } = string.Empty;

        public bool Estado { get; set; }
    }
}
