namespace Paraba.ENTITY.Models
{
    public class Ciudad
    {
        public int IdCiudad { get; set; }

        public int IdDepartamento { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
