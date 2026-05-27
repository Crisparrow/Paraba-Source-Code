namespace Paraba.ENTITY.Models
{
    public class Departamento
    {
        public int IdDepartamento { get; set; }

        public int IdPais { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
