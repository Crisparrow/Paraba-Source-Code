using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class DocumentoConductorService
    {
        private readonly DocumentoConductorRepository documentoConductorRepository = new DocumentoConductorRepository();
        private readonly ConductorRepository conductorRepository = new ConductorRepository();
        private readonly AuditoriaConductorService auditoriaConductorService = new AuditoriaConductorService();

        public List<DocumentoConductor> ListarDocumentos()
        {
            return documentoConductorRepository.Listar();
        }

        public bool AprobarDocumento(int idDocumentoConductor)
        {
            bool actualizado = documentoConductorRepository.ActualizarEstadoVerificacion(
                idDocumentoConductor,
                "Aprobado",
                "Documento aprobado por administracion.");

            ActualizarVerificacionConductor(idDocumentoConductor);
            RegistrarAuditoriaDocumento(idDocumentoConductor, "Documento aprobado", "Pendiente", "Aprobado", "Documento aprobado por administracion.");

            return actualizado;
        }

        public bool RechazarDocumento(int idDocumentoConductor)
        {
            return RechazarDocumento(idDocumentoConductor, "Documento rechazado. Requiere correccion.");
        }

        public bool RechazarDocumento(int idDocumentoConductor, string motivoRechazo)
        {
            bool actualizado = documentoConductorRepository.ActualizarEstadoVerificacion(
                idDocumentoConductor,
                "Rechazado",
                motivoRechazo);

            ActualizarVerificacionConductor(idDocumentoConductor);
            RegistrarAuditoriaDocumento(idDocumentoConductor, "Documento rechazado", "Pendiente", "Rechazado", motivoRechazo);

            return actualizado;
        }

        private void ActualizarVerificacionConductor(int idDocumentoConductor)
        {
            DocumentoConductor? documento = documentoConductorRepository.ObtenerPorId(idDocumentoConductor);

            if (documento == null)
            {
                return;
            }

            List<DocumentoConductor> documentosConductor = documentoConductorRepository
                .Listar()
                .Where(item => item.IdConductor == documento.IdConductor)
                .ToList();

            bool todosAprobados = documentosConductor.Count > 0 &&
                documentosConductor.All(item => item.EstadoVerificacion == "Aprobado");

            conductorRepository.ActualizarVerificado(documento.IdConductor, todosAprobados);

            if (todosAprobados)
            {
                auditoriaConductorService.RegistrarAuditoria(new AuditoriaConductor
                {
                    IdConductor = documento.IdConductor,
                    Accion = "Conductor verificado",
                    EstadoAnterior = "No verificado",
                    EstadoNuevo = "Verificado",
                    UsuarioSistema = "Admin PARABA",
                    Observacion = "Todos los documentos del conductor fueron aprobados."
                });
            }
        }

        private void RegistrarAuditoriaDocumento(
            int idDocumentoConductor,
            string accion,
            string estadoAnterior,
            string estadoNuevo,
            string observacion)
        {
            DocumentoConductor? documento = documentoConductorRepository.ObtenerPorId(idDocumentoConductor);

            if (documento == null)
            {
                return;
            }

            auditoriaConductorService.RegistrarAuditoria(new AuditoriaConductor
            {
                IdConductor = documento.IdConductor,
                Accion = accion,
                EstadoAnterior = estadoAnterior,
                EstadoNuevo = estadoNuevo,
                UsuarioSistema = "Admin PARABA",
                Observacion = observacion
            });
        }
    }
}
