using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class AuditoriaAdministrativaService
    {
        private readonly AuditoriaAdministrativaRepository auditoriaAdministrativaRepository = new AuditoriaAdministrativaRepository();

        public List<AuditoriaAdministrativa> ListarAuditorias()
        {
            return auditoriaAdministrativaRepository.Listar();
        }

        public void Registrar(string modulo, string accion, string entidad, int? idEntidad, string usuario, string observacion)
        {
            auditoriaAdministrativaRepository.Registrar(new AuditoriaAdministrativa
            {
                Modulo = string.IsNullOrWhiteSpace(modulo) ? "General" : modulo.Trim(),
                Accion = string.IsNullOrWhiteSpace(accion) ? "Accion administrativa" : accion.Trim(),
                Entidad = string.IsNullOrWhiteSpace(entidad) ? "N/A" : entidad.Trim(),
                IdEntidad = idEntidad,
                UsuarioSistema = string.IsNullOrWhiteSpace(usuario) ? "Admin PARABA" : usuario.Trim(),
                Observacion = string.IsNullOrWhiteSpace(observacion) ? "Sin observacion." : observacion.Trim()
            });
        }
    }
}
