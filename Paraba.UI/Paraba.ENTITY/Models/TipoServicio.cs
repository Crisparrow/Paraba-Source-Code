namespace Paraba.ENTITY.Models
{
    public class TipoServicio
    {
        public int IdTipoServicio { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string CategoriaVehiculo { get; set; } = "Taxi";

        public bool Estado { get; set; }
    }
}
