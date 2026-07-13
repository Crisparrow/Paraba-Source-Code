using Paraba.ENTITY.Models;
using Paraba.BLL.Services;

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

    public class DriverRegistrationDocumentUploadRequest
    {
        public string Telefono { get; set; } = string.Empty;

        public string TipoDocumento { get; set; } = string.Empty;

        public string NumeroDocumento { get; set; } = string.Empty;

        public DateTime? FechaVencimiento { get; set; }

        public IFormFile? Archivo { get; set; }
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

        public string EstadoDatosConductor { get; set; } = string.Empty;

        public string EstadoDatosVehiculo { get; set; } = string.Empty;

        public string EstadoDocumentos { get; set; } = string.Empty;

        public string ObservacionDatosConductor { get; set; } = string.Empty;

        public string ObservacionDatosVehiculo { get; set; } = string.Empty;

        public string ObservacionDocumentos { get; set; } = string.Empty;

        public DateTime FechaActualizacion { get; set; }

        public DateTime? FechaEnvio { get; set; }

        public bool PuedeOperar => EstadoSolicitud == "Aprobado" && IdConductor != null;

        public bool DatosConductorCompletos { get; set; }

        public bool DatosVehiculoCompletos { get; set; }

        public bool DocumentosCompletos { get; set; }

        public List<DriverRegistrationDocumentResponse> Documentos { get; set; } = new();

        public static DriverRegistrationResponse FromEntity(
            SolicitudRegistroConductor solicitud,
            List<SolicitudRegistroConductorDocumento>? documentos = null)
        {
            documentos ??= new List<SolicitudRegistroConductorDocumento>();

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
                EstadoDatosConductor = solicitud.EstadoDatosConductor,
                EstadoDatosVehiculo = solicitud.EstadoDatosVehiculo,
                EstadoDocumentos = solicitud.EstadoDocumentos,
                ObservacionDatosConductor = solicitud.ObservacionDatosConductor,
                ObservacionDatosVehiculo = solicitud.ObservacionDatosVehiculo,
                ObservacionDocumentos = solicitud.ObservacionDocumentos,
                FechaActualizacion = solicitud.FechaActualizacion,
                FechaEnvio = solicitud.FechaEnvio,
                DatosConductorCompletos = RegistroConductorService.DatosConductorCompletos(solicitud),
                DatosVehiculoCompletos = RegistroConductorService.DatosVehiculoCompletos(solicitud),
                DocumentosCompletos = RegistroConductorService.DocumentosCompletos(documentos),
                Documentos = documentos.Select(DriverRegistrationDocumentResponse.FromEntity).ToList()
            };
        }
    }

    public class DriverRegistrationDocumentResponse
    {
        public int IdSolicitudRegistroConductorDocumento { get; set; }

        public string TipoDocumento { get; set; } = string.Empty;

        public string NumeroDocumento { get; set; } = string.Empty;

        public string UrlArchivo { get; set; } = string.Empty;

        public DateTime? FechaVencimiento { get; set; }

        public bool EsOpcional { get; set; }

        public string EstadoVerificacion { get; set; } = string.Empty;

        public string Observacion { get; set; } = string.Empty;

        public static DriverRegistrationDocumentResponse FromEntity(SolicitudRegistroConductorDocumento documento)
        {
            return new DriverRegistrationDocumentResponse
            {
                IdSolicitudRegistroConductorDocumento = documento.IdSolicitudRegistroConductorDocumento,
                TipoDocumento = documento.TipoDocumento,
                NumeroDocumento = documento.NumeroDocumento,
                UrlArchivo = documento.UrlArchivo,
                FechaVencimiento = documento.FechaVencimiento,
                EsOpcional = documento.EsOpcional,
                EstadoVerificacion = documento.EstadoVerificacion,
                Observacion = documento.Observacion
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
