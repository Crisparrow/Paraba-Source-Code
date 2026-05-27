namespace Paraba.ENTITY.Models
{
    public class Zona
    {
        public int IdZona { get; set; }

        public int IdCiudad { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
