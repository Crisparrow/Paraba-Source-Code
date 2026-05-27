using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Verificador")]
    public class DocumentoConductorController : Controller
    {
        private readonly DocumentoConductorService documentoConductorService = new DocumentoConductorService();
        private readonly ConductorService conductorService = new ConductorService();

        public IActionResult Index()
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
            }).ToList();

            return View(documentosViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Aprobar(int id)
        {
            documentoConductorService.AprobarDocumento(id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
            if (string.IsNullOrWhiteSpace(viewModel.MotivoRechazo))
            {
                ModelState.AddModelError(string.Empty, "Debe ingresar el motivo del rechazo.");
                return View("Rechazar", viewModel);
            }

            documentoConductorService.RechazarDocumento(
                viewModel.IdDocumentoConductor,
                viewModel.MotivoRechazo);

            return RedirectToAction(nameof(Index));
        }

        private string ObtenerNombreConductor(int idConductor)
        {
            return conductorService.ListarConductores()
                .FirstOrDefault(item => item.IdConductor == idConductor)?.NombreCompleto
                ?? "Conductor no identificado";
        }
    }
}
