using System;
using System.Collections.Generic;
using System.Text;

namespace Paraba.ENTITY.Models
{
    public class Conductor
    {
        public int IdConductor { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string DocumentoIdentidad { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string LicenciaConducir { get; set; } = string.Empty;

        public DateTime FechaVencimientoLicencia { get; set; }

        public bool Disponible { get; set; }

        public bool Verificado { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
