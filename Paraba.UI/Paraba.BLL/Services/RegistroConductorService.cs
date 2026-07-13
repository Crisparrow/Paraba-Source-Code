using System.Security.Cryptography;
using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class RegistroConductorService
    {
        private readonly RegistroConductorRepository registroConductorRepository = new RegistroConductorRepository();
        private static readonly HashSet<string> DocumentosObligatorios = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CarnetFrontal",
            "CarnetReverso",
            "Licencia",
            "FotoConductor",
            "FotoVehiculo",
            "DocumentoVehiculo"
        };

        public (SolicitudRegistroConductor Solicitud, string CodigoDemo) SolicitarCodigo(string telefono)
        {
            telefono = NormalizarTelefono(telefono);
            string codigo = GenerarCodigo();

            registroConductorRepository.CrearOBuscarSolicitud(telefono);
            registroConductorRepository.RegistrarCodigo(telefono, codigo);

            SolicitudRegistroConductor solicitud = registroConductorRepository.CrearOBuscarSolicitud(telefono);

            return (solicitud, codigo);
        }

        public (SolicitudRegistroConductor Solicitud, string Token) VerificarCodigo(string telefono, string codigo)
        {
            telefono = NormalizarTelefono(telefono);

            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new ArgumentException("Debe ingresar el codigo de verificacion.");
            }

            bool codigoValido = registroConductorRepository.VerificarCodigo(telefono, codigo.Trim());

            if (!codigoValido)
            {
                throw new ArgumentException("El codigo es invalido o ya expiro.");
            }

            SolicitudRegistroConductor solicitud = registroConductorRepository.CrearOBuscarSolicitud(telefono);
            string token = GenerarTokenSesion();
            registroConductorRepository.CrearSesion(telefono, token);

            return (solicitud, token);
        }

        public SolicitudRegistroConductor ObtenerSolicitud(string telefono, string token)
        {
            telefono = NormalizarTelefono(telefono);
            ValidarSesion(telefono, token);

            return registroConductorRepository.CrearOBuscarSolicitud(telefono);
        }

        public SolicitudRegistroConductor GuardarBorrador(SolicitudRegistroConductor solicitud, string token)
        {
            solicitud.Telefono = NormalizarTelefono(solicitud.Telefono);
            ValidarSesion(solicitud.Telefono, token);
            solicitud.NombreCompleto = Limpiar(solicitud.NombreCompleto);
            solicitud.DocumentoIdentidad = Limpiar(solicitud.DocumentoIdentidad);
            solicitud.Correo = Limpiar(solicitud.Correo);
            solicitud.LicenciaConducir = Limpiar(solicitud.LicenciaConducir);
            solicitud.Placa = Limpiar(solicitud.Placa).ToUpperInvariant();
            solicitud.Marca = Limpiar(solicitud.Marca);
            solicitud.Modelo = Limpiar(solicitud.Modelo);
            solicitud.Color = Limpiar(solicitud.Color);

            if (solicitud.Anio != null && (solicitud.Anio < 1980 || solicitud.Anio > DateTime.Today.Year + 1))
            {
                throw new ArgumentException("El anio del vehiculo no es valido.");
            }

            return registroConductorRepository.GuardarBorrador(solicitud);
        }

        public List<SolicitudRegistroConductor> ListarSolicitudes()
        {
            return registroConductorRepository.ListarSolicitudes();
        }

        public List<SolicitudRegistroConductorDocumento> ListarDocumentos(int idSolicitudRegistroConductor)
        {
            if (idSolicitudRegistroConductor <= 0)
            {
                throw new ArgumentException("Debe seleccionar una solicitud valida.");
            }

            return registroConductorRepository.ListarDocumentos(idSolicitudRegistroConductor);
        }

        public List<SolicitudRegistroConductorDocumento> GuardarDocumento(
            string telefono,
            string token,
            string tipoDocumento,
            string numeroDocumento,
            string urlArchivo,
            DateTime? fechaVencimiento)
        {
            telefono = NormalizarTelefono(telefono);
            ValidarSesion(telefono, token);

            SolicitudRegistroConductor solicitud = registroConductorRepository.CrearOBuscarSolicitud(telefono);
            tipoDocumento = Limpiar(tipoDocumento);

            if (!EsTipoDocumentoPermitido(tipoDocumento))
            {
                throw new ArgumentException("El tipo de documento no es valido.");
            }

            if (string.IsNullOrWhiteSpace(urlArchivo))
            {
                throw new ArgumentException("Debe adjuntar un archivo.");
            }

            bool esOpcional = string.Equals(tipoDocumento, "RUAT", StringComparison.OrdinalIgnoreCase);

            return registroConductorRepository.GuardarDocumento(new SolicitudRegistroConductorDocumento
            {
                IdSolicitudRegistroConductor = solicitud.IdSolicitudRegistroConductor,
                TipoDocumento = tipoDocumento,
                NumeroDocumento = Limpiar(numeroDocumento),
                UrlArchivo = urlArchivo,
                FechaVencimiento = fechaVencimiento,
                EsOpcional = esOpcional
            });
        }

        public bool RevisarCategoria(int idSolicitudRegistroConductor, string categoria, string estado, string observacion)
        {
            if (idSolicitudRegistroConductor <= 0)
            {
                throw new ArgumentException("Debe seleccionar una solicitud valida.");
            }

            categoria = Limpiar(categoria);
            estado = Limpiar(estado);

            if (categoria != "Conductor" && categoria != "Vehiculo" && categoria != "Documentos")
            {
                throw new ArgumentException("La categoria de revision no es valida.");
            }

            if (estado != "Pendiente" && estado != "Aprobado" && estado != "Observado" && estado != "Rechazado")
            {
                throw new ArgumentException("El estado de revision no es valido.");
            }

            return registroConductorRepository.RevisarCategoria(idSolicitudRegistroConductor, categoria, estado, Limpiar(observacion));
        }

        public SolicitudRegistroConductor EnviarRevision(string telefono, string token)
        {
            telefono = NormalizarTelefono(telefono);
            ValidarSesion(telefono, token);
            SolicitudRegistroConductor solicitud = registroConductorRepository.CrearOBuscarSolicitud(telefono);
            List<SolicitudRegistroConductorDocumento> documentos = registroConductorRepository.ListarDocumentos(solicitud.IdSolicitudRegistroConductor);

            ValidarSolicitudCompleta(solicitud, documentos);

            return registroConductorRepository.EnviarRevision(telefono);
        }

        private static void ValidarSolicitudCompleta(
            SolicitudRegistroConductor solicitud,
            IEnumerable<SolicitudRegistroConductorDocumento> documentos)
        {
            if (!DatosConductorCompletos(solicitud))
            {
                throw new ArgumentException("Debe completar los datos del conductor.");
            }

            if (!DatosVehiculoCompletos(solicitud))
            {
                throw new ArgumentException("Debe completar los datos del vehiculo.");
            }

            if (!DocumentosCompletos(documentos))
            {
                throw new ArgumentException("Debe cargar todos los documentos obligatorios.");
            }
        }

        public static bool DatosConductorCompletos(SolicitudRegistroConductor solicitud)
        {
            return !string.IsNullOrWhiteSpace(solicitud.NombreCompleto) &&
                !string.IsNullOrWhiteSpace(solicitud.DocumentoIdentidad) &&
                !string.IsNullOrWhiteSpace(solicitud.LicenciaConducir) &&
                solicitud.FechaVencimientoLicencia != null &&
                solicitud.FechaVencimientoLicencia.Value.Date > DateTime.Today;
        }

        public static bool DatosVehiculoCompletos(SolicitudRegistroConductor solicitud)
        {
            return solicitud.IdTipoServicio != null &&
                solicitud.IdTipoServicio > 0 &&
                !string.IsNullOrWhiteSpace(solicitud.Placa) &&
                !string.IsNullOrWhiteSpace(solicitud.Marca) &&
                !string.IsNullOrWhiteSpace(solicitud.Modelo) &&
                !string.IsNullOrWhiteSpace(solicitud.Color) &&
                solicitud.Anio != null;
        }

        public static bool DocumentosCompletos(IEnumerable<SolicitudRegistroConductorDocumento> documentos)
        {
            HashSet<string> tiposCargados = documentos
                .Where(item => !item.EsOpcional && !string.IsNullOrWhiteSpace(item.UrlArchivo))
                .Select(item => item.TipoDocumento)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return DocumentosObligatorios.All(tiposCargados.Contains);
        }

        private static string NormalizarTelefono(string telefono)
        {
            string limpio = new string((telefono ?? string.Empty).Where(char.IsDigit).ToArray());

            if (limpio.StartsWith("591") && limpio.Length > 8)
            {
                limpio = limpio[3..];
            }

            if (limpio.Length != 8)
            {
                throw new ArgumentException("Debe ingresar un numero de telefono boliviano valido de 8 digitos.");
            }

            return $"+591{limpio}";
        }

        private void ValidarSesion(string telefono, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedAccessException("Debe iniciar sesion para continuar.");
            }

            if (!registroConductorRepository.ValidarSesion(telefono, token.Trim()))
            {
                throw new UnauthorizedAccessException("La sesion del conductor es invalida o expiro.");
            }
        }

        private static string Limpiar(string valor)
        {
            return (valor ?? string.Empty).Trim();
        }

        private static string GenerarCodigo()
        {
            return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        }

        private static string GenerarTokenSesion()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(48))
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", string.Empty);
        }

        private static bool EsTipoDocumentoPermitido(string tipoDocumento)
        {
            return DocumentosObligatorios.Contains(tipoDocumento) ||
                string.Equals(tipoDocumento, "RUAT", StringComparison.OrdinalIgnoreCase);
        }
    }
}
