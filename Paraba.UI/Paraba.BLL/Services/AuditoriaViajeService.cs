using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class AuditoriaViajeService
    {
        private readonly AuditoriaViajeRepository auditoriaViajeRepository = new AuditoriaViajeRepository();

        public List<AuditoriaViaje> ListarAuditoriaViajes()
        {
            return auditoriaViajeRepository.Listar();
        }

        public void RegistrarAuditoria(AuditoriaViaje auditoria)
        {
            if (auditoria.IdViaje <= 0)
            {
                throw new ArgumentException("Debe seleccionar un viaje valido.");
            }

            if (string.IsNullOrWhiteSpace(auditoria.Accion))
            {
                throw new ArgumentException("La accion de auditoria es obligatoria.");
            }

            auditoria.UsuarioSistema = string.IsNullOrWhiteSpace(auditoria.UsuarioSistema)
                ? "Admin PARABA"
                : auditoria.UsuarioSistema;

            auditoria.FechaRegistro = auditoria.FechaRegistro == default
                ? DateTime.Now
                : auditoria.FechaRegistro;

            auditoriaViajeRepository.Registrar(auditoria);
        }
    }
}
