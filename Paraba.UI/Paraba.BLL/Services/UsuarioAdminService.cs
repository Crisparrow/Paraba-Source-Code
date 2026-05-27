using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class UsuarioAdminService
    {
        private readonly UsuarioAdminRepository usuarioAdminRepository = new UsuarioAdminRepository();
        private readonly PasswordHasherService passwordHasherService = new PasswordHasherService();

        public List<UsuarioAdmin> ListarUsuarios()
        {
            var usuarios = usuarioAdminRepository.Listar();

            foreach (var usuario in usuarios)
            {
                usuario.Roles = usuarioAdminRepository.ListarRoles(usuario.IdUsuarioAdmin);
            }

            return usuarios;
        }

        public List<RolAdmin> ListarRoles()
        {
            return usuarioAdminRepository.ListarRolesAdmin();
        }

        public UsuarioAdmin? ValidarLogin(string correo, string password, string ipOrigen)
        {
            UsuarioAdmin? usuario = usuarioAdminRepository.ObtenerPorCorreo(correo);

            if (usuario == null)
            {
                RegistrarAcceso(null, correo, false, ipOrigen, "Usuario no encontrado.");
                return null;
            }

            if (!usuario.Estado)
            {
                RegistrarAcceso(usuario.IdUsuarioAdmin, correo, false, ipOrigen, "Usuario suspendido o inactivo.");
                return null;
            }

            bool passwordValido = passwordHasherService.VerificarPassword(
                password,
                usuario.PasswordHash,
                usuario.PasswordSalt,
                usuario.PasswordIterations);

            if (!passwordValido)
            {
                usuarioAdminRepository.RegistrarIntentoFallido(usuario.IdUsuarioAdmin);
                RegistrarAcceso(usuario.IdUsuarioAdmin, correo, false, ipOrigen, "Password incorrecto.");
                return null;
            }

            usuario.Roles = usuarioAdminRepository.ListarRoles(usuario.IdUsuarioAdmin);
            usuarioAdminRepository.MarcarAccesoCorrecto(usuario.IdUsuarioAdmin);
            RegistrarAcceso(usuario.IdUsuarioAdmin, correo, true, ipOrigen, "Login correcto.");

            return usuario;
        }

        public void RegistrarLogout(int idUsuarioAdmin, string correo, string ipOrigen)
        {
            RegistrarAcceso(idUsuarioAdmin, correo, true, ipOrigen, "Logout correcto.");
        }

        public void CrearUsuario(string nombreCompleto, string correo, string password, int idRolAdmin)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                throw new ArgumentException("Debe ingresar el nombre completo.");
            }

            if (string.IsNullOrWhiteSpace(correo))
            {
                throw new ArgumentException("Debe ingresar el correo.");
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 10)
            {
                throw new ArgumentException("El password debe tener al menos 10 caracteres.");
            }

            if (idRolAdmin <= 0)
            {
                throw new ArgumentException("Debe seleccionar un rol.");
            }

            if (usuarioAdminRepository.ObtenerPorCorreo(correo) != null)
            {
                throw new ArgumentException("Ya existe un usuario admin con ese correo.");
            }

            var hash = passwordHasherService.CrearHash(password);

            int idUsuarioAdmin = usuarioAdminRepository.Crear(new UsuarioAdmin
            {
                NombreCompleto = nombreCompleto,
                Correo = correo,
                PasswordHash = hash.Hash,
                PasswordSalt = hash.Salt,
                PasswordIterations = hash.Iterations,
                Estado = true,
                FechaRegistro = DateTime.Now
            });

            usuarioAdminRepository.AsignarRol(idUsuarioAdmin, idRolAdmin);
        }

        public void SuspenderUsuario(int idUsuarioAdmin)
        {
            usuarioAdminRepository.ActualizarEstado(idUsuarioAdmin, false);
        }

        public void ReactivarUsuario(int idUsuarioAdmin)
        {
            usuarioAdminRepository.ActualizarEstado(idUsuarioAdmin, true);
        }

        private void RegistrarAcceso(int? idUsuarioAdmin, string correo, bool exitoso, string ipOrigen, string observacion)
        {
            usuarioAdminRepository.RegistrarAcceso(new AuditoriaAccesoAdmin
            {
                IdUsuarioAdmin = idUsuarioAdmin,
                Correo = correo,
                Accion = exitoso ? "Acceso autorizado" : "Acceso rechazado",
                Exitoso = exitoso,
                IpOrigen = string.IsNullOrWhiteSpace(ipOrigen) ? "No identificada" : ipOrigen,
                Observacion = observacion,
                FechaRegistro = DateTime.Now
            });
        }
    }
}
