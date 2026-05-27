using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class AuditoriaConductorService
    {
        private readonly AuditoriaConductorRepository auditoriaConductorRepository = new AuditoriaConductorRepository();

        public List<AuditoriaConductor> ListarAuditoriaConductores()
        {
            return auditoriaConductorRepository.Listar();
        }

        public void RegistrarAuditoria(AuditoriaConductor auditoria)
        {
            auditoria.UsuarioSistema = string.IsNullOrWhiteSpace(auditoria.UsuarioSistema)
                ? "Admin PARABA"
                : auditoria.UsuarioSistema;

            auditoria.FechaRegistro = auditoria.FechaRegistro == default
                ? DateTime.Now
                : auditoria.FechaRegistro;

            auditoriaConductorRepository.Registrar(auditoria);
        }
    }
}
