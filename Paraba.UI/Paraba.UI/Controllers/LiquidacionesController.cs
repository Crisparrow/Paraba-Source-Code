using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Authorize(Roles = "SuperAdmin,Finanzas")]
    public class LiquidacionesController : Controller
    {
        private readonly ViajeService viajeService = new ViajeService();
        private readonly ConductorService conductorService = new ConductorService();
        private readonly PasajeroService pasajeroService = new PasajeroService();
        private readonly TipoServicioService tipoServicioService = new TipoServicioService();
        private readonly LiquidacionConductorService liquidacionConductorService = new LiquidacionConductorService();
        private readonly ComisionServicioService comisionServicioService = new ComisionServicioService();
        private readonly AuditoriaAdministrativaService auditoriaAdministrativaService = new AuditoriaAdministrativaService();

        public IActionResult Index(LiquidacionesViewModel filtros)
        {
            PrepararLiquidacionesViewModel(filtros);

            return View(filtros);
        }

        public IActionResult ConfirmarCierre(LiquidacionesViewModel filtros)
        {
            PrepararLiquidacionesViewModel(filtros);

            if (filtros.IdConductor == null)
            {
                TempData["MensajeLiquidacion"] = "Debe seleccionar un conductor para cerrar la liquidacion.";
                return RedirectToAction(nameof(Index), filtros);
            }

            if (filtros.FechaDesde == null || filtros.FechaHasta == null)
            {
                TempData["MensajeLiquidacion"] = "Debe seleccionar fecha desde y fecha hasta.";
                return RedirectToAction(nameof(Index), filtros);
            }

            if (!filtros.Liquidaciones.Any())
            {
                TempData["MensajeLiquidacion"] = "No hay viajes finalizados para cerrar esta liquidacion.";
                return RedirectToAction(nameof(Index), filtros);
            }

            var conductor = conductorService.ListarConductores()
                .FirstOrDefault(item => item.IdConductor == filtros.IdConductor.Value);

            var viewModel = new ConfirmarCierreLiquidacionViewModel
            {
                FechaDesde = filtros.FechaDesde,
                FechaHasta = filtros.FechaHasta,
                IdConductor = filtros.IdConductor,
                Conductor = conductor?.NombreCompleto ?? "Conductor no identificado",
                TotalViajesFinalizados = filtros.TotalViajesFinalizados,
                TotalBruto = filtros.TotalBruto,
                TotalComisionParaba = filtros.TotalComisionParaba,
                TotalNetoConductor = filtros.TotalNetoConductores,
                Liquidaciones = filtros.Liquidaciones
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CerrarConfirmado(ConfirmarCierreLiquidacionViewModel viewModel)
        {
            var filtros = new LiquidacionesViewModel
            {
                FechaDesde = viewModel.FechaDesde,
                FechaHasta = viewModel.FechaHasta,
                IdConductor = viewModel.IdConductor
            };

            PrepararLiquidacionesViewModel(filtros);

            viewModel.Conductor = conductorService.ListarConductores()
                .FirstOrDefault(item => item.IdConductor == viewModel.IdConductor)?.NombreCompleto ?? "Conductor no identificado";
            viewModel.TotalViajesFinalizados = filtros.TotalViajesFinalizados;
            viewModel.TotalBruto = filtros.TotalBruto;
            viewModel.TotalComisionParaba = filtros.TotalComisionParaba;
            viewModel.TotalNetoConductor = filtros.TotalNetoConductores;
            viewModel.Liquidaciones = filtros.Liquidaciones;

            if (!ModelState.IsValid)
            {
                return View("ConfirmarCierre", viewModel);
            }

            if (viewModel.IdConductor == null || viewModel.FechaDesde == null || viewModel.FechaHasta == null)
            {
                ModelState.AddModelError(string.Empty, "Debe seleccionar conductor y rango de fechas.");
                return View("ConfirmarCierre", viewModel);
            }

            if (!viewModel.Liquidaciones.Any())
            {
                ModelState.AddModelError(string.Empty, "No hay viajes finalizados para cerrar esta liquidacion.");
                return View("ConfirmarCierre", viewModel);
            }

            try
            {
                string usuarioCierre = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "Admin PARABA";

                int idLiquidacion = liquidacionConductorService.CerrarLiquidacion(
                    viewModel.IdConductor.Value,
                    viewModel.FechaDesde.Value,
                    viewModel.FechaHasta.Value,
                    filtros.PorcentajeComision,
                    usuarioCierre,
                    viajeService.ListarViajes(),
                    viewModel.ObservacionCierre);
                auditoriaAdministrativaService.Registrar("Finanzas", "Liquidacion cerrada", "LiquidacionConductor", idLiquidacion, usuarioCierre, $"Neto conductor: {viewModel.TotalNetoConductor:0.00}");

                TempData["MensajeLiquidacion"] = "Liquidacion cerrada correctamente.";
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("ConfirmarCierre", viewModel);
            }

            return RedirectToAction(nameof(Historial));
        }

        [Obsolete("Usar ConfirmarCierre y CerrarConfirmado para evitar cierres directos.")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cerrar(LiquidacionesViewModel filtros)
        {
            return RedirectToAction(nameof(ConfirmarCierre), filtros);
        }

        public IActionResult Historial(HistorialLiquidacionesViewModel filtros)
        {
            var conductores = conductorService.ListarConductores();

            var liquidaciones = liquidacionConductorService.ListarLiquidaciones()
                .Select(item => new LiquidacionCerradaViewModel
                {
                    IdLiquidacionConductor = item.IdLiquidacionConductor,
                    Conductor = conductores.FirstOrDefault(conductor => conductor.IdConductor == item.IdConductor)?.NombreCompleto ?? "Conductor no identificado",
                    FechaDesde = item.FechaDesde,
                    FechaHasta = item.FechaHasta,
                    PorcentajeComision = item.PorcentajeComision,
                    TotalBruto = item.TotalBruto,
                    TotalComisionParaba = item.TotalComisionParaba,
                    TotalNetoConductor = item.TotalNetoConductor,
                    Estado = item.Estado,
                    UsuarioCierre = item.UsuarioCierre,
                    FechaCierre = item.FechaCierre,
                    FechaPago = item.FechaPago,
                    Observacion = item.Observacion
                });

            if (!string.IsNullOrWhiteSpace(filtros.Estado))
            {
                liquidaciones = liquidaciones.Where(item => item.Estado == filtros.Estado);
            }

            if (filtros.FechaDesde != null)
            {
                liquidaciones = liquidaciones.Where(item => item.FechaCierre.Date >= filtros.FechaDesde.Value.Date);
            }

            if (filtros.FechaHasta != null)
            {
                liquidaciones = liquidaciones.Where(item => item.FechaCierre.Date <= filtros.FechaHasta.Value.Date);
            }

            var todas = liquidacionConductorService.ListarLiquidaciones();

            filtros.Estados = new List<SelectListItem>
            {
                new SelectListItem("Todos", string.Empty),
                new SelectListItem("Cerrada", "Cerrada"),
                new SelectListItem("Pagada", "Pagada"),
                new SelectListItem("Anulada", "Anulada")
            };
            filtros.Liquidaciones = liquidaciones.ToList();
            filtros.TotalLiquidaciones = todas.Count;
            filtros.TotalCerradas = todas.Count(item => item.Estado == "Cerrada");
            filtros.TotalPagadas = todas.Count(item => item.Estado == "Pagada");
            filtros.TotalAnuladas = todas.Count(item => item.Estado == "Anulada");
            filtros.TotalNetoPendientePago = todas
                .Where(item => item.Estado == "Cerrada")
                .Sum(item => item.TotalNetoConductor);
            filtros.TotalComisionParaba = todas
                .Where(item => item.Estado != "Anulada")
                .Sum(item => item.TotalComisionParaba);

            return View(filtros);
        }

        private void PrepararLiquidacionesViewModel(LiquidacionesViewModel filtros)
        {
            var conductores = conductorService.ListarConductores();
            var pasajeros = pasajeroService.ListarPasajeros();
            var tiposServicio = tipoServicioService.ListarTiposServicio();
            var comisiones = comisionServicioService.ListarComisiones();
            var idsViajesLiquidados = liquidacionConductorService.ListarIdsViajesLiquidados();

            var viajesFinalizados = viajeService.ListarViajes()
                .Where(item => item.IdEstadoViaje == 4 &&
                    item.FechaFin != null &&
                    !idsViajesLiquidados.Contains(item.IdViaje))
                .AsEnumerable();

            if (filtros.FechaDesde != null)
            {
                viajesFinalizados = viajesFinalizados.Where(item => item.FechaFin!.Value.Date >= filtros.FechaDesde.Value.Date);
            }

            if (filtros.FechaHasta != null)
            {
                viajesFinalizados = viajesFinalizados.Where(item => item.FechaFin!.Value.Date <= filtros.FechaHasta.Value.Date);
            }

            if (filtros.IdConductor != null)
            {
                viajesFinalizados = viajesFinalizados.Where(item => item.IdConductor == filtros.IdConductor.Value);
            }

            var viajes = viajesFinalizados
                .OrderByDescending(item => item.FechaFin)
                .ToList();

            filtros.Conductores = new List<SelectListItem>
            {
                new SelectListItem("Todos", string.Empty)
            };
            filtros.Conductores.AddRange(conductores.Select(item => new SelectListItem(item.NombreCompleto, item.IdConductor.ToString())));

            filtros.Liquidaciones = viajes.Select(viaje =>
            {
                decimal porcentajeComision = comisiones
                    .Where(item => item.IdTipoServicio == viaje.IdTipoServicio && item.Estado)
                    .OrderByDescending(item => item.FechaInicioVigencia)
                    .FirstOrDefault()?.PorcentajeComision ?? 0;
                decimal comision = Math.Round(viaje.TarifaFinal * (porcentajeComision / 100), 2);
                decimal netoConductor = viaje.TarifaFinal - comision;

                return new LiquidacionItemViewModel
                {
                    IdViaje = viaje.IdViaje,
                    Conductor = conductores.FirstOrDefault(item => item.IdConductor == viaje.IdConductor)?.NombreCompleto ?? "Conductor no identificado",
                    Pasajero = pasajeros.FirstOrDefault(item => item.IdPasajero == viaje.IdPasajero)?.NombreCompleto ?? "Pasajero no identificado",
                    TipoServicio = tiposServicio.FirstOrDefault(item => item.IdTipoServicio == viaje.IdTipoServicio)?.Nombre ?? "Tipo no identificado",
                    Ruta = $"{viaje.Origen} -> {viaje.Destino}",
                    TarifaFinal = viaje.TarifaFinal,
                    PorcentajeComision = porcentajeComision,
                    ComisionParaba = comision,
                    NetoConductor = netoConductor,
                    FechaFin = viaje.FechaFin!.Value
                };
            }).ToList();

            filtros.TotalViajesFinalizados = filtros.Liquidaciones.Count;
            filtros.TotalBruto = filtros.Liquidaciones.Sum(item => item.TarifaFinal);
            filtros.TotalComisionParaba = filtros.Liquidaciones.Sum(item => item.ComisionParaba);
            filtros.TotalNetoConductores = filtros.Liquidaciones.Sum(item => item.NetoConductor);
        }

        public IActionResult Detalle(int id)
        {
            var viewModel = CrearDetalleLiquidacionViewModel(id);

            if (viewModel == null)
            {
                return RedirectToAction(nameof(Historial));
            }

            return View(viewModel);
        }

        public IActionResult MarcarPagada(int id)
        {
            var viewModel = CrearIntervencionLiquidacionViewModel(id, "Marcar pagada");

            if (viewModel == null)
            {
                return RedirectToAction(nameof(Historial));
            }

            return View("Intervencion", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarPago(IntervencionLiquidacionViewModel viewModel)
        {
            return ConfirmarIntervencionLiquidacion(viewModel, "Marcar pagada");
        }

        public IActionResult Anular(int id)
        {
            var viewModel = CrearIntervencionLiquidacionViewModel(id, "Anular");

            if (viewModel == null)
            {
                return RedirectToAction(nameof(Historial));
            }

            return View("Intervencion", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarAnulacion(IntervencionLiquidacionViewModel viewModel)
        {
            return ConfirmarIntervencionLiquidacion(viewModel, "Anular");
        }

        private IActionResult ConfirmarIntervencionLiquidacion(IntervencionLiquidacionViewModel viewModel, string accion)
        {
            var datos = CrearIntervencionLiquidacionViewModel(viewModel.IdLiquidacionConductor, accion);

            if (datos == null)
            {
                return RedirectToAction(nameof(Historial));
            }

            viewModel.Conductor = datos.Conductor;
            viewModel.Estado = datos.Estado;
            viewModel.TotalNetoConductor = datos.TotalNetoConductor;
            viewModel.Accion = datos.Accion;

            if (!ModelState.IsValid)
            {
                return View("Intervencion", viewModel);
            }

            try
            {
                if (accion == "Marcar pagada")
                {
                    liquidacionConductorService.MarcarPagada(viewModel.IdLiquidacionConductor, viewModel.Observacion);
                    auditoriaAdministrativaService.Registrar("Finanzas", "Liquidacion pagada", "LiquidacionConductor", viewModel.IdLiquidacionConductor, User.Identity?.Name ?? "Admin PARABA", viewModel.Observacion);
                }
                else
                {
                    liquidacionConductorService.Anular(viewModel.IdLiquidacionConductor, viewModel.Observacion);
                    auditoriaAdministrativaService.Registrar("Finanzas", "Liquidacion anulada", "LiquidacionConductor", viewModel.IdLiquidacionConductor, User.Identity?.Name ?? "Admin PARABA", viewModel.Observacion);
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Intervencion", viewModel);
            }

            return RedirectToAction(nameof(Historial));
        }

        private IntervencionLiquidacionViewModel? CrearIntervencionLiquidacionViewModel(int idLiquidacionConductor, string accion)
        {
            var liquidacion = liquidacionConductorService.ListarLiquidaciones()
                .FirstOrDefault(item => item.IdLiquidacionConductor == idLiquidacionConductor);

            if (liquidacion == null || liquidacion.Estado != "Cerrada")
            {
                return null;
            }

            var conductor = conductorService.ListarConductores()
                .FirstOrDefault(item => item.IdConductor == liquidacion.IdConductor);

            return new IntervencionLiquidacionViewModel
            {
                IdLiquidacionConductor = liquidacion.IdLiquidacionConductor,
                Conductor = conductor?.NombreCompleto ?? "Conductor no identificado",
                Estado = liquidacion.Estado,
                TotalNetoConductor = liquidacion.TotalNetoConductor,
                Accion = accion
            };
        }

        private DetalleLiquidacionViewModel? CrearDetalleLiquidacionViewModel(int idLiquidacionConductor)
        {
            var liquidacion = liquidacionConductorService.ListarLiquidaciones()
                .FirstOrDefault(item => item.IdLiquidacionConductor == idLiquidacionConductor);

            if (liquidacion == null)
            {
                return null;
            }

            var conductores = conductorService.ListarConductores();
            var pasajeros = pasajeroService.ListarPasajeros();
            var tiposServicio = tipoServicioService.ListarTiposServicio();
            var viajes = viajeService.ListarViajes();
            var detalles = liquidacionConductorService.ListarDetalles(idLiquidacionConductor)
                .Select(detalle =>
                {
                    var viaje = viajes.FirstOrDefault(item => item.IdViaje == detalle.IdViaje);

                    return new LiquidacionDetalleItemViewModel
                    {
                        IdViaje = detalle.IdViaje,
                        Pasajero = viaje == null
                            ? "Pasajero no identificado"
                            : pasajeros.FirstOrDefault(item => item.IdPasajero == viaje.IdPasajero)?.NombreCompleto ?? "Pasajero no identificado",
                        TipoServicio = viaje == null
                            ? "Tipo no identificado"
                            : tiposServicio.FirstOrDefault(item => item.IdTipoServicio == viaje.IdTipoServicio)?.Nombre ?? "Tipo no identificado",
                        Ruta = viaje == null ? "-" : $"{viaje.Origen} -> {viaje.Destino}",
                        TarifaFinal = detalle.TarifaFinal,
                        ComisionParaba = detalle.ComisionParaba,
                        NetoConductor = detalle.NetoConductor,
                        FechaRegistro = detalle.FechaRegistro
                    };
                })
                .ToList();

            return new DetalleLiquidacionViewModel
            {
                IdLiquidacionConductor = liquidacion.IdLiquidacionConductor,
                Conductor = conductores.FirstOrDefault(item => item.IdConductor == liquidacion.IdConductor)?.NombreCompleto ?? "Conductor no identificado",
                FechaDesde = liquidacion.FechaDesde,
                FechaHasta = liquidacion.FechaHasta,
                PorcentajeComision = liquidacion.PorcentajeComision,
                TotalBruto = liquidacion.TotalBruto,
                TotalComisionParaba = liquidacion.TotalComisionParaba,
                TotalNetoConductor = liquidacion.TotalNetoConductor,
                Estado = liquidacion.Estado,
                UsuarioCierre = liquidacion.UsuarioCierre,
                FechaCierre = liquidacion.FechaCierre,
                FechaPago = liquidacion.FechaPago,
                Observacion = liquidacion.Observacion,
                Detalles = detalles
            };
        }
    }
}
