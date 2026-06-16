using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;
using System.Security.Claims;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones,Soporte")]
    public class ViajeController : Controller
    {
        private readonly ViajeAdminService viajeAdminService = new ViajeAdminService();
        private readonly PasajeroService pasajeroService = new PasajeroService();
        private readonly ConductorService conductorService = new ConductorService();
        private readonly VehiculoService vehiculoService = new VehiculoService();
        private readonly TipoServicioService tipoServicioService = new TipoServicioService();
        private readonly AuditoriaViajeService auditoriaViajeService = new AuditoriaViajeService();
        private readonly AuditoriaAdministrativaService auditoriaAdministrativaService = new AuditoriaAdministrativaService();

        public IActionResult Index()
        {
            var viajes = viajeAdminService.ListarViajes();
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

        public IActionResult Detalle(int id)
        {
            var viaje = viajeAdminService.ListarViajes().FirstOrDefault(item => item.IdViaje == id);

            if (viaje == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var pasajero = pasajeroService.ListarPasajeros().FirstOrDefault(item => item.IdPasajero == viaje.IdPasajero);
            var conductor = conductorService.ListarConductores().FirstOrDefault(item => item.IdConductor == viaje.IdConductor);
            var vehiculo = vehiculoService.ListarVehiculos().FirstOrDefault(item => item.IdVehiculo == viaje.IdVehiculo);
            var auditorias = auditoriaViajeService.ListarAuditoriaViajes()
                .Where(item => item.IdViaje == viaje.IdViaje)
                .OrderByDescending(item => item.FechaRegistro)
                .Select(item => new AuditoriaViajeViewModel
                {
                    IdAuditoriaViaje = item.IdAuditoriaViaje,
                    IdViaje = item.IdViaje,
                    Accion = item.Accion,
                    EstadoAnterior = item.EstadoAnterior,
                    EstadoNuevo = item.EstadoNuevo,
                    TarifaAnterior = item.TarifaAnterior,
                    TarifaNueva = item.TarifaNueva,
                    UsuarioSistema = item.UsuarioSistema,
                    Observacion = item.Observacion,
                    FechaRegistro = item.FechaRegistro
                })
                .ToList();

            var viewModel = new DetalleViajeViewModel
            {
                IdViaje = viaje.IdViaje,
                EstadoViaje = ObtenerEstadoViaje(viaje.IdEstadoViaje),
                TipoServicio = ObtenerTipoServicio(viaje.IdTipoServicio),
                Origen = viaje.Origen,
                Destino = viaje.Destino,
                TarifaSugerida = viaje.TarifaSugerida,
                TarifaOfertada = viaje.TarifaOfertada,
                TarifaContraoferta = viaje.TarifaContraoferta,
                TarifaAceptada = viaje.TarifaAceptada,
                TarifaFinal = viaje.TarifaFinal,
                FechaSolicitud = viaje.FechaSolicitud,
                FechaInicio = viaje.FechaInicio,
                FechaFin = viaje.FechaFin,
                Pasajero = pasajero?.NombreCompleto ?? "Pasajero no identificado",
                DocumentoPasajero = pasajero?.DocumentoIdentidad ?? "-",
                TelefonoPasajero = pasajero?.Telefono ?? "-",
                CorreoPasajero = pasajero?.Correo ?? "-",
                Conductor = conductor?.NombreCompleto ?? "Conductor no identificado",
                DocumentoConductor = conductor?.DocumentoIdentidad ?? "-",
                TelefonoConductor = conductor?.Telefono ?? "-",
                CorreoConductor = conductor?.Correo ?? "-",
                Vehiculo = vehiculo == null ? "Vehiculo no identificado" : $"{vehiculo.Marca} {vehiculo.Modelo}",
                Placa = vehiculo?.Placa ?? "-",
                Color = vehiculo?.Color ?? "-",
                PuedeCancelar = viaje.IdEstadoViaje != 4 && viaje.IdEstadoViaje != 5,
                Auditorias = auditorias
            };

            return View(viewModel);
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones")]
        public IActionResult Cancelar(int id)
        {
            var viewModel = CrearIntervencionViajeViewModel(id);

            if (viewModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Operaciones")]
        public IActionResult ConfirmarCancelacion(IntervencionViajeViewModel viewModel)
        {
            var datosViaje = CrearIntervencionViajeViewModel(viewModel.IdViaje);

            if (datosViaje == null)
            {
                return RedirectToAction(nameof(Index));
            }

            viewModel.Pasajero = datosViaje.Pasajero;
            viewModel.Conductor = datosViaje.Conductor;
            viewModel.Ruta = datosViaje.Ruta;
            viewModel.EstadoViaje = datosViaje.EstadoViaje;

            if (!ModelState.IsValid)
            {
                return View("Cancelar", viewModel);
            }

            try
            {
                viajeAdminService.CancelarViaje(viewModel.IdViaje, viewModel.Motivo);
                auditoriaAdministrativaService.Registrar("Operaciones", "Viaje cancelado por administracion", "Viaje", viewModel.IdViaje, ObtenerUsuario(), viewModel.Motivo);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Cancelar", viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        private IntervencionViajeViewModel? CrearIntervencionViajeViewModel(int idViaje)
        {
            var viaje = viajeAdminService.ListarViajes().FirstOrDefault(item => item.IdViaje == idViaje);

            if (viaje == null || viaje.IdEstadoViaje == 4 || viaje.IdEstadoViaje == 5)
            {
                return null;
            }

            return new IntervencionViajeViewModel
            {
                IdViaje = viaje.IdViaje,
                Pasajero = ObtenerNombrePasajero(viaje.IdPasajero),
                Conductor = ObtenerNombreConductor(viaje.IdConductor),
                Ruta = $"{viaje.Origen} -> {viaje.Destino}",
                EstadoViaje = ObtenerEstadoViaje(viaje.IdEstadoViaje)
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

        private string ObtenerUsuario()
        {
            return User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "Admin PARABA";
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

