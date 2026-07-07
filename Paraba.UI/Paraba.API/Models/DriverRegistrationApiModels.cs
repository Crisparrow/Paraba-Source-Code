using Paraba.ENTITY.Models;

namespace Paraba.API.Models
{
    public class DriverRequestCodeRequest
    {
        public string Telefono { get; set; } = string.Empty;
    }

    public class DriverVerifyCodeRequest
    {
        public string Telefono { get; set; } = string.Empty;

        public string Codigo { get; set; } = string.Empty;
    }

    public class DriverRegistrationDraftRequest
    {
        public string Telefono { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public string DocumentoIdentidad { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string LicenciaConducir { get; set; } = string.Empty;

        public DateTime? FechaVencimientoLicencia { get; set; }

        public int? IdTipoServicio { get; set; }

        public string Placa { get; set; } = string.Empty;

        public string Marca { get; set; } = string.Empty;

        public string Modelo { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public int? Anio { get; set; }
    }

    public class DriverRegistrationResponse
    {
        public int IdSolicitudRegistroConductor { get; set; }

        public int? IdConductor { get; set; }

        public string Telefono { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public string DocumentoIdentidad { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string LicenciaConducir { get; set; } = string.Empty;

        public DateTime? FechaVencimientoLicencia { get; set; }

        public int? IdTipoServicio { get; set; }

        public string Placa { get; set; } = string.Empty;

        public string Marca { get; set; } = string.Empty;

        public string Modelo { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public int? Anio { get; set; }

        public string EstadoSolicitud { get; set; } = string.Empty;

        public string ObservacionRevision { get; set; } = string.Empty;

        public DateTime FechaActualizacion { get; set; }

        public DateTime? FechaEnvio { get; set; }

        public bool PuedeOperar => EstadoSolicitud == "Aprobado" && IdConductor != null;

        public static DriverRegistrationResponse FromEntity(SolicitudRegistroConductor solicitud)
        {
            return new DriverRegistrationResponse
            {
                IdSolicitudRegistroConductor = solicitud.IdSolicitudRegistroConductor,
                IdConductor = solicitud.IdConductor,
                Telefono = solicitud.Telefono,
                NombreCompleto = solicitud.NombreCompleto,
                DocumentoIdentidad = solicitud.DocumentoIdentidad,
                Correo = solicitud.Correo,
                LicenciaConducir = solicitud.LicenciaConducir,
                FechaVencimientoLicencia = solicitud.FechaVencimientoLicencia,
                IdTipoServicio = solicitud.IdTipoServicio,
                Placa = solicitud.Placa,
                Marca = solicitud.Marca,
                Modelo = solicitud.Modelo,
                Color = solicitud.Color,
                Anio = solicitud.Anio,
                EstadoSolicitud = solicitud.EstadoSolicitud,
                ObservacionRevision = solicitud.ObservacionRevision,
                FechaActualizacion = solicitud.FechaActualizacion,
                FechaEnvio = solicitud.FechaEnvio
            };
        }
    }

    public class DriverRequestCodeResponse
    {
        public string Mensaje { get; set; } = string.Empty;

        public string Canal { get; set; } = string.Empty;

        public string? CodigoDemo { get; set; }

        public DriverRegistrationResponse Solicitud { get; set; } = new();
    }

    public class DriverVerifyCodeResponse
    {
        public string Token { get; set; } = string.Empty;

        public DriverRegistrationResponse Solicitud { get; set; } = new();
    }
}
