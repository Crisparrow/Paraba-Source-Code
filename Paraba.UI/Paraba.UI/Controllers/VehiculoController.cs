using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Verificador,Soporte")]
    public class VehiculoController : Controller
    {
        private readonly VehiculoService vehiculoService = new VehiculoService();
        private readonly ConductorService conductorService = new ConductorService();
        private readonly TipoServicioService tipoServicioService = new TipoServicioService();

        public IActionResult Index()
        {
            var conductores = conductorService.ListarConductores();
            var tiposServicio = tipoServicioService.ListarTiposServicio();
            var vehiculos = vehiculoService.ListarVehiculos();
            var vehiculosViewModel = vehiculos.Select(vehiculo => new VehiculoViewModel
            {
                IdVehiculo = vehiculo.IdVehiculo,
                Conductor = conductores.FirstOrDefault(item => item.IdConductor == vehiculo.IdConductor)?.NombreCompleto ?? "Conductor no identificado",
                TipoServicio = tiposServicio.FirstOrDefault(item => item.IdTipoServicio == vehiculo.IdTipoServicio)?.Nombre ?? "Tipo no identificado",
                Placa = vehiculo.Placa,
                Marca = vehiculo.Marca,
                Modelo = vehiculo.Modelo,
                Color = vehiculo.Color,
                Anio = vehiculo.Anio,
                Verificado = vehiculo.Verificado,
                EstadoVerificacion = vehiculo.EstadoVerificacion,
                Observacion = vehiculo.Observacion,
                Estado = vehiculo.Estado
            }).ToList();

            return View(vehiculosViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Verificador")]
        public IActionResult Revisar(int idVehiculo, string estado, string observacion)
        {
            try
            {
                if (estado == "Aprobado")
                {
                    vehiculoService.AprobarVehiculo(idVehiculo, observacion ?? string.Empty);
                    TempData["Success"] = "Vehiculo aprobado correctamente.";
                }
                else if (estado == "Rechazado")
                {
                    vehiculoService.RechazarVehiculo(idVehiculo, observacion ?? string.Empty);
                    TempData["Success"] = "Vehiculo rechazado. La app mostrara la observacion al conductor.";
                }
                else
                {
                    throw new ArgumentException("Estado de revision invalido.");
                }
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
