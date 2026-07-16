using System.Security.Cryptography;
using System.Text;
using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class ConductorAuthService
    {
        private const int OtpMinutes = 5;
        private const int MaxOtpAttempts = 5;
        private static readonly TimeSpan SessionDuration = TimeSpan.FromDays(30);
        private readonly ConductorAuthRepository authRepository = new ConductorAuthRepository();
        private readonly AuditoriaConductorService auditoriaConductorService = new AuditoriaConductorService();

        public OtpSolicitudResult SolicitarCodigo(string codigoPais, string telefono, string canal)
        {
            string telefonoNormalizado = NormalizarTelefono(codigoPais, telefono);
            string canalNormalizado = NormalizarCanal(canal);
            string codigo = RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");
            DateTime fechaExpiracion = DateTime.Now.AddMinutes(OtpMinutes);

            authRepository.CrearOtp(telefonoNormalizado, CrearHashCodigo(telefonoNormalizado, codigo), canalNormalizado, fechaExpiracion);

            return new OtpSolicitudResult
            {
                Telefono = telefonoNormalizado,
                Codigo = codigo,
                Canal = canalNormalizado,
                FechaExpiracion = fechaExpiracion
            };
        }

        public OtpVerificacionResult VerificarCodigo(string codigoPais, string telefono, string codigo)
        {
            string telefonoNormalizado = NormalizarTelefono(codigoPais, telefono);
            string codigoLimpio = NormalizarCodigo(codigo);
            OtpVerificacion otp = ObtenerOtpValido(telefonoNormalizado);

            bool codigoCorrecto = otp.CodigoHash == CrearHashCodigo(telefonoNormalizado, codigoLimpio);
            authRepository.RegistrarIntentoOtp(otp.IdOtpVerificacion, codigoCorrecto);

            if (!codigoCorrecto)
            {
                throw new ArgumentException("El codigo ingresado no es correcto.");
            }

            Conductor? conductor = authRepository.ObtenerConductorPorTelefono(telefonoNormalizado);

            if (conductor == null)
            {
                return new OtpVerificacionResult
                {
                    Telefono = telefonoNormalizado,
                    RegistroRequerido = true
                };
            }

            string tokenSesion = authRepository.CrearSesionConductor(
                conductor.IdConductor,
                "App conductor",
                DateTime.Now.Add(SessionDuration));

            return new OtpVerificacionResult
            {
                Telefono = telefonoNormalizado,
                RegistroRequerido = false,
                IdConductor = conductor.IdConductor,
                NombreCompleto = conductor.NombreCompleto,
                TokenSesion = tokenSesion,
                Verificado = conductor.Verificado,
                Activo = conductor.Estado
            };
        }

        public RegistroConductorResult CompletarRegistro(string codigoPais, string telefono, string nombre, string apellido, string dispositivo)
        {
            string telefonoNormalizado = NormalizarTelefono(codigoPais, telefono);
            string nombreCompleto = NormalizarNombreCompleto(nombre, apellido);
            OtpVerificacion otp = authRepository.ObtenerOtpVerificadoReciente(telefonoNormalizado)
                ?? throw new ArgumentException("Primero debes verificar el codigo enviado al telefono.");

            Conductor? conductor = authRepository.ObtenerConductorPorTelefono(telefonoNormalizado);
            bool conductorNuevo = conductor == null;
            int idConductor = conductor?.IdConductor ?? authRepository.RegistrarConductorBasico(nombreCompleto, telefonoNormalizado);

            authRepository.MarcarOtpUsado(otp.IdOtpVerificacion);

            string tokenSesion = authRepository.CrearSesionConductor(
                idConductor,
                string.IsNullOrWhiteSpace(dispositivo) ? "App conductor" : dispositivo.Trim(),
                DateTime.Now.Add(SessionDuration));

            if (conductorNuevo)
            {
                auditoriaConductorService.RegistrarAuditoria(new AuditoriaConductor
                {
                    IdConductor = idConductor,
                    Accion = "Registro desde app",
                    EstadoAnterior = "No registrado",
                    EstadoNuevo = "Pendiente de verificacion",
                    UsuarioSistema = "App conductor",
                    Observacion = "Conductor creado desde la app movil con telefono verificado."
                });
            }

            return new RegistroConductorResult
            {
                IdConductor = idConductor,
                NombreCompleto = nombreCompleto,
                Telefono = telefonoNormalizado,
                TokenSesion = tokenSesion,
                RegistroNuevo = conductorNuevo,
                EstadoVerificacion = "Pendiente"
            };
        }

        private static OtpVerificacion ObtenerOtpValido(string telefono)
        {
            ConductorAuthRepository repository = new ConductorAuthRepository();
            OtpVerificacion otp = repository.ObtenerOtpActivo(telefono)
                ?? throw new ArgumentException("No existe un codigo activo para este telefono o ya expiro.");

            if (otp.Intentos >= MaxOtpAttempts)
            {
                throw new ArgumentException("Se supero el numero de intentos permitidos. Solicita un nuevo codigo.");
            }

            return otp;
        }

        private static string NormalizarTelefono(string codigoPais, string telefono)
        {
            string pais = string.IsNullOrWhiteSpace(codigoPais) ? "+591" : codigoPais.Trim();
            string numero = new string((telefono ?? string.Empty).Where(char.IsDigit).ToArray());

            if (pais != "+591")
            {
                throw new ArgumentException("Por ahora PARABA solo acepta numeros de Bolivia (+591).");
            }

            if (numero.StartsWith("591"))
            {
                numero = numero.Substring(3);
            }

            if (numero.Length < 7 || numero.Length > 8)
            {
                throw new ArgumentException("Ingresa un numero de celular boliviano valido.");
            }

            return $"+591{numero}";
        }

        private static string NormalizarCanal(string canal)
        {
            string canalNormalizado = string.IsNullOrWhiteSpace(canal) ? "SMS" : canal.Trim().ToUpperInvariant();

            return canalNormalizado switch
            {
                "SMS" => "SMS",
                "WHATSAPP" => "WHATSAPP",
                _ => throw new ArgumentException("El canal debe ser SMS o WhatsApp.")
            };
        }

        private static string NormalizarCodigo(string codigo)
        {
            string codigoLimpio = new string((codigo ?? string.Empty).Where(char.IsDigit).ToArray());

            if (codigoLimpio.Length != 6)
            {
                throw new ArgumentException("El codigo debe tener 6 digitos.");
            }

            return codigoLimpio;
        }

        private static string NormalizarNombreCompleto(string nombre, string apellido)
        {
            string nombreLimpio = (nombre ?? string.Empty).Trim();
            string apellidoLimpio = (apellido ?? string.Empty).Trim();

            if (nombreLimpio.Length < 2 || apellidoLimpio.Length < 2)
            {
                throw new ArgumentException("Ingresa nombre y apellido validos.");
            }

            return $"{nombreLimpio} {apellidoLimpio}";
        }

        private static string CrearHashCodigo(string telefono, string codigo)
        {
            string contenido = $"PARABA|{telefono}|{codigo}";
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(contenido));
            return Convert.ToHexString(bytes);
        }
    }

    public class OtpSolicitudResult
    {
        public string Telefono { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Canal { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
    }

    public class OtpVerificacionResult
    {
        public string Telefono { get; set; } = string.Empty;
        public bool RegistroRequerido { get; set; }
        public int? IdConductor { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string TokenSesion { get; set; } = string.Empty;
        public bool Verificado { get; set; }
        public bool Activo { get; set; }
    }

    public class RegistroConductorResult
    {
        public int IdConductor { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string TokenSesion { get; set; } = string.Empty;
        public bool RegistroNuevo { get; set; }
        public string EstadoVerificacion { get; set; } = string.Empty;
    }
}
