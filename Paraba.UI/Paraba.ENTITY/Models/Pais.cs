namespace Paraba.ENTITY.Models
{
    public class Pais
    {
        public int IdPais { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string CodigoIso { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
