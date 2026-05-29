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

        public bool AprobarDocumento(int idDocumentoConductor, string observacionAprobacion)
        {
            DocumentoConductor? documento = documentoConductorRepository.ObtenerPorId(idDocumentoConductor);

            if (documento == null)
            {
                throw new ArgumentException("El documento no existe.");
            }

            if (documento.EstadoVerificacion != "Pendiente")
            {
                throw new ArgumentException("Solo se pueden aprobar documentos pendientes.");
            }

            if (documento.FechaVencimiento != null && documento.FechaVencimiento.Value.Date < DateTime.Today)
            {
                throw new ArgumentException("No se puede aprobar un documento vencido.");
            }

            string observacion = string.IsNullOrWhiteSpace(observacionAprobacion)
                ? "Documento aprobado por administracion."
                : observacionAprobacion.Trim();

            bool actualizado = documentoConductorRepository.ActualizarEstadoVerificacion(
                idDocumentoConductor,
                "Aprobado",
                observacion);

            ActualizarVerificacionConductor(idDocumentoConductor);
            RegistrarAuditoriaDocumento(idDocumentoConductor, "Documento aprobado", documento.EstadoVerificacion, "Aprobado", observacion);

            return actualizado;
        }

        public bool RechazarDocumento(int idDocumentoConductor)
        {
            return RechazarDocumento(idDocumentoConductor, "Documento rechazado. Requiere correccion.");
        }

        public bool RechazarDocumento(int idDocumentoConductor, string motivoRechazo)
        {
            DocumentoConductor? documento = documentoConductorRepository.ObtenerPorId(idDocumentoConductor);

            if (documento == null)
            {
                throw new ArgumentException("El documento no existe.");
            }

            if (documento.EstadoVerificacion != "Pendiente")
            {
                throw new ArgumentException("Solo se pueden rechazar documentos pendientes.");
            }

            if (string.IsNullOrWhiteSpace(motivoRechazo) || motivoRechazo.Trim().Length < 10)
            {
                throw new ArgumentException("Debe ingresar un motivo de rechazo valido.");
            }

            bool actualizado = documentoConductorRepository.ActualizarEstadoVerificacion(
                idDocumentoConductor,
                "Rechazado",
                motivoRechazo.Trim());

            ActualizarVerificacionConductor(idDocumentoConductor);
            RegistrarAuditoriaDocumento(idDocumentoConductor, "Documento rechazado", documento.EstadoVerificacion, "Rechazado", motivoRechazo.Trim());

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
