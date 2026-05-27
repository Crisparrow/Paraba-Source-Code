namespace Paraba.UI.ViewModels
{
    public class ZonaViewModel
    {
        public int IdZona { get; set; }

        public string Ciudad { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
