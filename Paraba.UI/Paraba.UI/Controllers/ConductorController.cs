using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Verificador,Soporte")]
    public class ConductorController : Controller
    {
        private readonly ConductorService conductorService = new ConductorService();
        private readonly DocumentoConductorService documentoConductorService = new DocumentoConductorService();

        public IActionResult Index()
        {
            var conductores = conductorService.ListarConductores();

            return View(conductores);
        }

        public IActionResult Pendientes()
        {
            var documentos = documentoConductorService.ListarDocumentos();

            var conductoresPendientes = conductorService.ListarConductores()
                .Where(conductor => conductor.Estado && !conductor.Verificado)
                .Select(conductor =>
                {
                    var documentosConductor = documentos
                        .Where(documento => documento.IdConductor == conductor.IdConductor)
                        .ToList();

                    return new ConductorPendienteViewModel
                    {
                        IdConductor = conductor.IdConductor,
                        NombreCompleto = conductor.NombreCompleto,
                        Telefono = conductor.Telefono,
                        Correo = conductor.Correo,
                        Disponible = conductor.Disponible,
                        Verificado = conductor.Verificado,
                        Estado = conductor.Estado,
                        DocumentosPendientes = documentosConductor.Count(documento => documento.EstadoVerificacion == "Pendiente"),
                        DocumentosAprobados = documentosConductor.Count(documento => documento.EstadoVerificacion == "Aprobado"),
                        DocumentosRechazados = documentosConductor.Count(documento => documento.EstadoVerificacion == "Rechazado"),
                        TotalDocumentos = documentosConductor.Count
                    };
                })
                .ToList();

            return View(conductoresPendientes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Suspender(int id)
        {
            conductorService.SuspenderConductor(id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reactivar(int id)
        {
            conductorService.ReactivarConductor(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
