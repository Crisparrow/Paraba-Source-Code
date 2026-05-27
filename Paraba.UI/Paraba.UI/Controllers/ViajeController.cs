using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Paraba.BLL.Services;
using Paraba.ENTITY.Models;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones,Soporte")]
    public class ViajeController : Controller
    {
        private readonly ViajeService viajeService = new ViajeService();
        private readonly PasajeroService pasajeroService = new PasajeroService();
        private readonly ConductorService conductorService = new ConductorService();
        private readonly VehiculoService vehiculoService = new VehiculoService();
        private readonly TipoServicioService tipoServicioService = new TipoServicioService();
        private readonly TarifaService tarifaService = new TarifaService();

        public IActionResult Index()
        {
            var viajes = viajeService.ListarViajes();
            var viajesViewModel = viajes.Select(viaje => new ViajeViewModel
            {
                IdViaje = viaje.IdViaje,
                Pasajero = ObtenerNombrePasajero(viaje.IdPasajero),
                Conductor = ObtenerNombreConductor(viaje.IdConductor),
                Vehiculo = ObtenerDescripcionVehiculo(viaje.IdVehiculo),
                TipoServicio = ObtenerTipoServicio(viaje.IdTipoServicio),
                Origen = viaje.Origen,
                Destino = viaje.Destino,
                TarifaEstimada = viaje.TarifaEstimada,
                TarifaFinal = viaje.TarifaFinal,
                TarifaSugerida = viaje.TarifaSugerida,
                TarifaOfertada = viaje.TarifaOfertada,
                TarifaContraoferta = viaje.TarifaContraoferta,
                TarifaAceptada = viaje.TarifaAceptada,
                EstadoViaje = ObtenerEstadoViaje(viaje.IdEstadoViaje),
                FechaSolicitud = viaje.FechaSolicitud
            }).ToList();

            return View(viajesViewModel);
        }

        [HttpGet]
        public IActionResult CrearSolicitud()
        {
            var viewModel = CrearSolicitudViewModelBase();
            viewModel.DistanciaKilometros = 5;
            viewModel.TiempoMinutos = 12;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearSolicitud(SolicitudViajeViewModel viewModel)
        {
            var resultado = CrearSolicitudViewModelBase();
            resultado.IdPasajero = viewModel.IdPasajero;
            resultado.IdConductor = viewModel.IdConductor;
            resultado.IdVehiculo = viewModel.IdVehiculo;
            resultado.IdTipoServicio = viewModel.IdTipoServicio;
            resultado.Origen = viewModel.Origen;
            resultado.Destino = viewModel.Destino;
            resultado.DistanciaKilometros = viewModel.DistanciaKilometros;
            resultado.TiempoMinutos = viewModel.TiempoMinutos;
            resultado.TarifaOfertada = viewModel.TarifaOfertada;
            string accion = Request.Form["accion"].ToString();

            Tarifa? tarifa = tarifaService.ListarTarifas()
                .FirstOrDefault(item => item.IdTipoServicio == viewModel.IdTipoServicio && item.Estado);

            if (tarifa == null)
            {
                ModelState.AddModelError(string.Empty, "No existe una tarifa activa para el tipo de servicio seleccionado.");
                return View(resultado);
            }

            resultado.TarifaSugerida = viajeService.CalcularTarifaSugerida(
                tarifa,
                viewModel.DistanciaKilometros,
                viewModel.TiempoMinutos);

            if (viewModel.TarifaOfertada <= 0)
            {
                resultado.TarifaOfertada = resultado.TarifaSugerida;
            }

            if (accion == "Calcular")
            {
                return View(resultado);
            }

            if (!ModelState.IsValid)
            {
                return View(resultado);
            }

            try
            {
                viajeService.RegistrarSolicitud(new Viaje
                {
                    IdPasajero = viewModel.IdPasajero,
                    IdConductor = viewModel.IdConductor,
                    IdVehiculo = viewModel.IdVehiculo,
                    IdTipoServicio = viewModel.IdTipoServicio,
                    Origen = viewModel.Origen,
                    Destino = viewModel.Destino,
                    TarifaSugerida = resultado.TarifaSugerida,
                    TarifaOfertada = resultado.TarifaOfertada
                });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(resultado);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Contraoferta(int id)
        {
            Viaje? viaje = viajeService.ListarViajes().FirstOrDefault(item => item.IdViaje == id);

            if (viaje == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (viaje.IdEstadoViaje != 1)
            {
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ContraofertaViajeViewModel
            {
                IdViaje = viaje.IdViaje,
                Pasajero = ObtenerNombrePasajero(viaje.IdPasajero),
                Conductor = ObtenerNombreConductor(viaje.IdConductor),
                Ruta = $"{viaje.Origen} -> {viaje.Destino}",
                TarifaSugerida = viaje.TarifaSugerida,
                TarifaOfertada = viaje.TarifaOfertada,
                TarifaContraofertaActual = viaje.TarifaContraoferta,
                NuevaContraoferta = viaje.TarifaContraoferta ?? viaje.TarifaOfertada
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contraoferta(ContraofertaViajeViewModel viewModel)
        {
            try
            {
                viajeService.RegistrarContraoferta(viewModel.IdViaje, viewModel.NuevaContraoferta);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ResponderContraoferta(int id)
        {
            Viaje? viaje = viajeService.ListarViajes().FirstOrDefault(item => item.IdViaje == id);

            if (viaje == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (viaje.IdEstadoViaje == 4 || viaje.IdEstadoViaje == 5)
            {
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ResponderContraofertaViewModel
            {
                IdViaje = viaje.IdViaje,
                Pasajero = ObtenerNombrePasajero(viaje.IdPasajero),
                Conductor = ObtenerNombreConductor(viaje.IdConductor),
                Ruta = $"{viaje.Origen} -> {viaje.Destino}",
                TarifaSugerida = viaje.TarifaSugerida,
                TarifaOfertada = viaje.TarifaOfertada,
                TarifaContraoferta = viaje.TarifaContraoferta,
                TarifaAceptada = viaje.TarifaAceptada
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AceptarContraoferta(int idViaje)
        {
            try
            {
                viajeService.AceptarContraoferta(idViaje);
            }
            catch (ArgumentException)
            {
                return RedirectToAction(nameof(ResponderContraoferta), new { id = idViaje });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Iniciar(int idViaje)
        {
            try
            {
                viajeService.IniciarViaje(idViaje);
            }
            catch (ArgumentException)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Finalizar(int idViaje)
        {
            try
            {
                viajeService.FinalizarViaje(idViaje);
            }
            catch (ArgumentException)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancelar(int idViaje)
        {
            try
            {
                viajeService.CancelarViaje(idViaje);
            }
            catch (ArgumentException)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        private SolicitudViajeViewModel CrearSolicitudViewModelBase()
        {
            return new SolicitudViajeViewModel
            {
                Pasajeros = pasajeroService.ListarPasajeros()
                    .Where(item => item.Estado)
                    .Select(item => new SelectListItem(item.NombreCompleto, item.IdPasajero.ToString()))
                    .ToList(),
                Conductores = conductorService.ListarConductores()
                    .Where(item => item.Estado && item.Verificado && item.Disponible)
                    .Select(item => new SelectListItem($"{item.NombreCompleto} - disponible", item.IdConductor.ToString()))
                    .ToList(),
                Vehiculos = vehiculoService.ListarVehiculos()
                    .Where(item => item.Estado && item.Verificado)
                    .Select(item => new SelectListItem($"{item.Marca} {item.Modelo} - {item.Placa} | Conductor {item.IdConductor} | Servicio {item.IdTipoServicio}", item.IdVehiculo.ToString()))
                    .ToList(),
                TiposServicio = tipoServicioService.ListarTiposServicio()
                    .Where(item => item.Estado)
                    .Select(item => new SelectListItem(item.Nombre, item.IdTipoServicio.ToString()))
                    .ToList()
            };
        }

        private string ObtenerNombrePasajero(int idPasajero)
        {
            return pasajeroService.ListarPasajeros()
                .FirstOrDefault(item => item.IdPasajero == idPasajero)?.NombreCompleto
                ?? "Pasajero no identificado";
        }

        private string ObtenerNombreConductor(int idConductor)
        {
            return conductorService.ListarConductores()
                .FirstOrDefault(item => item.IdConductor == idConductor)?.NombreCompleto
                ?? "Conductor no identificado";
        }

        private string ObtenerDescripcionVehiculo(int idVehiculo)
        {
            var vehiculo = vehiculoService.ListarVehiculos()
                .FirstOrDefault(item => item.IdVehiculo == idVehiculo);

            return vehiculo == null
                ? "Vehiculo no identificado"
                : $"{vehiculo.Marca} {vehiculo.Modelo} - {vehiculo.Placa}";
        }

        private string ObtenerTipoServicio(int idTipoServicio)
        {
            return tipoServicioService.ListarTiposServicio()
                .FirstOrDefault(item => item.IdTipoServicio == idTipoServicio)?.Nombre
                ?? "Tipo no identificado";
        }

        private static string ObtenerEstadoViaje(int idEstadoViaje)
        {
            return idEstadoViaje switch
            {
                1 => "Solicitado",
                2 => "Aceptado",
                3 => "En curso",
                4 => "Finalizado",
                5 => "Cancelado",
                _ => "Estado no identificado"
            };
        }
    }
}
