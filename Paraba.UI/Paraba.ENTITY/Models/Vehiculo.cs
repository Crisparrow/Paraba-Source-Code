using System;
using System.Collections.Generic;
using System.Text;

namespace Paraba.ENTITY.Models
{
    public class Vehiculo
    {
        public int IdVehiculo { get; set; }

        public int IdConductor { get; set; }

        public int IdTipoServicio { get; set; }

        public string Placa { get; set; } = string.Empty;

        public string Marca { get; set; } = string.Empty;

        public string Modelo { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public int Anio { get; set; }

        public bool Verificado { get; set; }

        public string EstadoVerificacion { get; set; } = "Pendiente";

        public string Observacion { get; set; } = string.Empty;

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
