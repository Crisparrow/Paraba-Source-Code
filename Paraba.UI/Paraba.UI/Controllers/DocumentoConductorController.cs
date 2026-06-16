using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;
using System.Security.Claims;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Verificador")]
    public class DocumentoConductorController : Controller
    {
        private readonly DocumentoConductorService documentoConductorService = new DocumentoConductorService();
        private readonly ConductorService conductorService = new ConductorService();
        private readonly AuditoriaAdministrativaService auditoriaAdministrativaService = new AuditoriaAdministrativaService();

        public IActionResult Index(DocumentoConductorFiltroViewModel filtros)
        {
            var documentos = documentoConductorService.ListarDocumentos();
            var documentosViewModel = documentos.Select(documento => new DocumentoConductorViewModel
            {
                IdDocumentoConductor = documento.IdDocumentoConductor,
                Conductor = ObtenerNombreConductor(documento.IdConductor),
                TipoDocumento = documento.TipoDocumento,
                NumeroDocumento = documento.NumeroDocumento,
                UrlArchivo = documento.UrlArchivo,
                FechaVencimiento = documento.FechaVencimiento,
                EstadoVerificacion = documento.EstadoVerificacion,
                Observacion = documento.Observacion,
                FechaRegistro = documento.FechaRegistro
            });

            if (!string.IsNullOrWhiteSpace(filtros.Buscar))
            {
                string buscar = filtros.Buscar.Trim();
                documentosViewModel = documentosViewModel.Where(item =>
                    item.Conductor.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    item.TipoDocumento.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    item.NumeroDocumento.Contains(buscar, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filtros.EstadoVerificacion))
            {
                documentosViewModel = documentosViewModel.Where(item => item.EstadoVerificacion == filtros.EstadoVerificacion);
            }

            if (filtros.SoloVencidos)
            {
                documentosViewModel = documentosViewModel.Where(item =>
                    item.FechaVencimiento != null &&
                    item.FechaVencimiento.Value.Date < DateTime.Today);
            }

            filtros.Estados = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
            {
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Todos", string.Empty),
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Pendiente", "Pendiente"),
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Aprobado", "Aprobado"),
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Rechazado", "Rechazado")
            };
            filtros.Documentos = documentosViewModel
                .OrderByDescending(item => item.EstadoVerificacion == "Pendiente")
                .ThenBy(item => item.FechaVencimiento ?? DateTime.MaxValue)
                .ToList();
            filtros.TotalDocumentos = documentos.Count;
            filtros.TotalPendientes = documentos.Count(item => item.EstadoVerificacion == "Pendiente");
            filtros.TotalAprobados = documentos.Count(item => item.EstadoVerificacion == "Aprobado");
            filtros.TotalRechazados = documentos.Count(item => item.EstadoVerificacion == "Rechazado");
            filtros.TotalVencidos = documentos.Count(item => item.FechaVencimiento != null && item.FechaVencimiento.Value.Date < DateTime.Today);

            return View(filtros);
        }

        public IActionResult Detalle(int id)
        {
            var viewModel = CrearRevisionDocumentoViewModel(id);

            if (viewModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        public IActionResult Aprobar(int id)
        {
            var viewModel = CrearRevisionDocumentoViewModel(id);

            if (viewModel == null || viewModel.EstadoVerificacion != "Pendiente")
            {
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarAprobacion(RevisionDocumentoConductorViewModel viewModel)
        {
            var datosDocumento = CrearRevisionDocumentoViewModel(viewModel.IdDocumentoConductor);

            if (datosDocumento == null)
            {
                return RedirectToAction(nameof(Index));
            }

            viewModel.Conductor = datosDocumento.Conductor;
            viewModel.TipoDocumento = datosDocumento.TipoDocumento;
            viewModel.NumeroDocumento = datosDocumento.NumeroDocumento;
            viewModel.UrlArchivo = datosDocumento.UrlArchivo;
            viewModel.FechaVencimiento = datosDocumento.FechaVencimiento;
            viewModel.EstadoVerificacion = datosDocumento.EstadoVerificacion;
            viewModel.Observacion = datosDocumento.Observacion;
            viewModel.EstaVencido = datosDocumento.EstaVencido;

            if (!ModelState.IsValid)
            {
                return View("Aprobar", viewModel);
            }

            try
            {
                documentoConductorService.AprobarDocumento(
                    viewModel.IdDocumentoConductor,
                    viewModel.ObservacionAprobacion);

                auditoriaAdministrativaService.Registrar(
                    "Verificacion",
                    "Documento aprobado",
                    "DocumentoConductor",
                    viewModel.IdDocumentoConductor,
                    ObtenerUsuario(),
                    $"{viewModel.TipoDocumento} aprobado para {viewModel.Conductor}. {viewModel.ObservacionAprobacion}");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Aprobar", viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Rechazar(int id)
        {
            var documento = documentoConductorService.ListarDocumentos()
                .FirstOrDefault(item => item.IdDocumentoConductor == id);

            if (documento == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new RechazarDocumentoConductorViewModel
            {
                IdDocumentoConductor = documento.IdDocumentoConductor,
                Conductor = ObtenerNombreConductor(documento.IdConductor),
                TipoDocumento = documento.TipoDocumento,
                NumeroDocumento = documento.NumeroDocumento
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarRechazo(RechazarDocumentoConductorViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.MotivoRechazo) || viewModel.MotivoRechazo.Trim().Length < 10)
            {
                ModelState.AddModelError(string.Empty, "Debe ingresar un motivo de rechazo valido.");
                return View("Rechazar", viewModel);
            }

            try
            {
                documentoConductorService.RechazarDocumento(
                    viewModel.IdDocumentoConductor,
                    viewModel.MotivoRechazo);

                auditoriaAdministrativaService.Registrar(
                    "Verificacion",
                    "Documento rechazado",
                    "DocumentoConductor",
                    viewModel.IdDocumentoConductor,
                    ObtenerUsuario(),
                    viewModel.MotivoRechazo);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Rechazar", viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        private RevisionDocumentoConductorViewModel? CrearRevisionDocumentoViewModel(int idDocumentoConductor)
        {
            var documento = documentoConductorService.ListarDocumentos()
                .FirstOrDefault(item => item.IdDocumentoConductor == idDocumentoConductor);

            if (documento == null)
            {
                return null;
            }

            return new RevisionDocumentoConductorViewModel
            {
                IdDocumentoConductor = documento.IdDocumentoConductor,
                Conductor = ObtenerNombreConductor(documento.IdConductor),
                TipoDocumento = documento.TipoDocumento,
                NumeroDocumento = documento.NumeroDocumento,
                UrlArchivo = documento.UrlArchivo,
                FechaVencimiento = documento.FechaVencimiento,
                EstadoVerificacion = documento.EstadoVerificacion,
                Observacion = documento.Observacion,
                EstaVencido = documento.FechaVencimiento != null && documento.FechaVencimiento.Value.Date < DateTime.Today
            };
        }

        private string ObtenerNombreConductor(int idConductor)
        {
            return conductorService.ListarConductores()
                .FirstOrDefault(item => item.IdConductor == idConductor)?.NombreCompleto
                ?? "Conductor no identificado";
        }

        private string ObtenerUsuario()
        {
            return User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "Admin PARABA";
        }
    }
}
