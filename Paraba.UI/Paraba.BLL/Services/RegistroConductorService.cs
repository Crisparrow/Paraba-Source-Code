using System.Security.Cryptography;
using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class RegistroConductorService
    {
        private readonly RegistroConductorRepository registroConductorRepository = new RegistroConductorRepository();

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

        public SolicitudRegistroConductor EnviarRevision(string telefono, string token)
        {
            telefono = NormalizarTelefono(telefono);
            ValidarSesion(telefono, token);
            SolicitudRegistroConductor solicitud = registroConductorRepository.CrearOBuscarSolicitud(telefono);

            ValidarSolicitudCompleta(solicitud);

            return registroConductorRepository.EnviarRevision(telefono);
        }

        private static void ValidarSolicitudCompleta(SolicitudRegistroConductor solicitud)
        {
            if (string.IsNullOrWhiteSpace(solicitud.NombreCompleto))
            {
                throw new ArgumentException("Debe ingresar el nombre completo.");
            }

            if (string.IsNullOrWhiteSpace(solicitud.DocumentoIdentidad))
            {
                throw new ArgumentException("Debe ingresar el documento de identidad.");
            }

            if (string.IsNullOrWhiteSpace(solicitud.LicenciaConducir))
            {
                throw new ArgumentException("Debe ingresar la licencia de conducir.");
            }

            if (solicitud.FechaVencimientoLicencia == null || solicitud.FechaVencimientoLicencia.Value.Date <= DateTime.Today)
            {
                throw new ArgumentException("Debe ingresar una fecha de vencimiento de licencia vigente.");
            }

            if (solicitud.IdTipoServicio == null || solicitud.IdTipoServicio <= 0)
            {
                throw new ArgumentException("Debe seleccionar el tipo de servicio.");
            }

            if (string.IsNullOrWhiteSpace(solicitud.Placa) ||
                string.IsNullOrWhiteSpace(solicitud.Marca) ||
                string.IsNullOrWhiteSpace(solicitud.Modelo) ||
                string.IsNullOrWhiteSpace(solicitud.Color) ||
                solicitud.Anio == null)
            {
                throw new ArgumentException("Debe completar los datos del vehiculo.");
            }
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
    }
}
