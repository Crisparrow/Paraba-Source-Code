using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class ConductorService
    {
        private readonly ConductorRepository conductorRepository = new ConductorRepository();
        private readonly AuditoriaConductorService auditoriaConductorService = new AuditoriaConductorService();

        public List<Conductor> ListarConductores()
        {
            return conductorRepository.Listar();
        }

        public bool SuspenderConductor(int idConductor, string motivo)
        {
            if (idConductor <= 0)
            {
                throw new ArgumentException("Debe seleccionar un conductor valido.");
            }

            if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length < 10)
            {
                throw new ArgumentException("Debe ingresar un motivo administrativo valido.");
            }

            bool actualizado = conductorRepository.ActualizarEstado(idConductor, false);

            if (actualizado)
            {
                RegistrarAuditoria(idConductor, "Conductor suspendido", "Activo", "Suspendido", motivo.Trim());
            }

            return actualizado;
        }

        public bool ReactivarConductor(int idConductor, string motivo)
        {
            if (idConductor <= 0)
            {
                throw new ArgumentException("Debe seleccionar un conductor valido.");
            }

            if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length < 10)
            {
                throw new ArgumentException("Debe ingresar un motivo administrativo valido.");
            }

            bool actualizado = conductorRepository.ActualizarEstado(idConductor, true);

            if (actualizado)
            {
                RegistrarAuditoria(idConductor, "Conductor reactivado", "Suspendido", "Activo", motivo.Trim());
            }

            return actualizado;
        }

        private void RegistrarAuditoria(int idConductor, string accion, string estadoAnterior, string estadoNuevo, string observacion)
        {
            auditoriaConductorService.RegistrarAuditoria(new AuditoriaConductor
            {
                IdConductor = idConductor,
                Accion = accion,
                EstadoAnterior = estadoAnterior,
                EstadoNuevo = estadoNuevo,
                UsuarioSistema = "Admin PARABA",
                Observacion = observacion
            });
        }
    }
}
