using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult Index(ViajeHistorialViewModel filtros)
        {
            var viajes = viajeAdminService.ListarViajes()
                .Select(MapViajeViewModel)
                .AsEnumerable();

            if (filtros.FechaDesde.HasValue)
            {
                DateTime fechaDesde = filtros.FechaDesde.Value.Date;
                viajes = viajes.Where(viaje => viaje.FechaSolicitud.Date >= fechaDesde);
            }

            if (filtros.FechaHasta.HasValue)
            {
                DateTime fechaHasta = filtros.FechaHasta.Value.Date;
                viajes = viajes.Where(viaje => viaje.FechaSolicitud.Date <= fechaHasta);
            }

            if (filtros.IdEstadoViaje.HasValue)
            {
                viajes = viajes.Where(viaje => viaje.IdEstadoViaje == filtros.IdEstadoViaje.Value);
            }

            if (filtros.IdTipoServicio.HasValue)
            {
                viajes = viajes.Where(viaje => viaje.IdTipoServicio == filtros.IdTipoServicio.Value);
            }

            if (filtros.IdConductor.HasValue)
            {
                viajes = viajes.Where(viaje => viaje.IdConductor == filtros.IdConductor.Value);
            }

            List<ViajeViewModel> viajesFiltrados = viajes
                .OrderByDescending(viaje => viaje.FechaSolicitud)
                .ToList();

            filtros.Viajes = viajesFiltrados;
            filtros.TotalViajes = viajesFiltrados.Count;
            filtros.ViajesActivos = viajesFiltrados.Count(viaje => viaje.IdEstadoViaje is 1 or 2 or 3 or 6 or 7);
            filtros.ViajesFinalizados = viajesFiltrados.Count(viaje => viaje.IdEstadoViaje == 4);
            filtros.ViajesCancelados = viajesFiltrados.Count(viaje => viaje.IdEstadoViaje == 5);
            filtros.ViajesContraofertados = viajesFiltrados.Count(viaje => viaje.IdEstadoViaje == 6 || viaje.TarifaContraoferta.HasValue);
            filtros.IngresosFinalizados = viajesFiltrados
                .Where(viaje => viaje.IdEstadoViaje == 4)
                .Sum(viaje => viaje.TarifaFinal);
            filtros.EstadosViaje = CrearEstadosViajeSelectList(filtros.IdEstadoViaje);
            filtros.TiposServicio = CrearTiposServicioSelectList(filtros.IdTipoServicio);
            filtros.Conductores = CrearConductoresSelectList(filtros.IdConductor);

            return View(filtros);
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
                FechaCancelacion = viaje.FechaCancelacion,
                MotivoCancelacion = viaje.MotivoCancelacion,
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

        private ViajeViewModel MapViajeViewModel(Paraba.ENTITY.Models.Viaje viaje)
        {
            return new ViajeViewModel
            {
                IdViaje = viaje.IdViaje,
                IdPasajero = viaje.IdPasajero,
                IdConductor = viaje.IdConductor,
                IdVehiculo = viaje.IdVehiculo,
                IdTipoServicio = viaje.IdTipoServicio,
                IdEstadoViaje = viaje.IdEstadoViaje,
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
                FechaSolicitud = viaje.FechaSolicitud,
                FechaInicio = viaje.FechaInicio,
                FechaFin = viaje.FechaFin,
                FechaCancelacion = viaje.FechaCancelacion,
                MotivoCancelacion = viaje.MotivoCancelacion
            };
        }

        private List<SelectListItem> CrearEstadosViajeSelectList(int? selected)
        {
            List<SelectListItem> estados = new List<SelectListItem>
            {
                new SelectListItem("Todos", string.Empty, selected == null),
                new SelectListItem("Solicitado", "1", selected == 1),
                new SelectListItem("Aceptado", "2", selected == 2),
                new SelectListItem("En curso", "3", selected == 3),
                new SelectListItem("Finalizado", "4", selected == 4),
                new SelectListItem("Cancelado", "5", selected == 5),
                new SelectListItem("Contraofertado", "6", selected == 6),
                new SelectListItem("En camino al pasajero", "7", selected == 7)
            };

            return estados;
        }

        private List<SelectListItem> CrearTiposServicioSelectList(int? selected)
        {
            List<SelectListItem> items = tipoServicioService.ListarTiposServicio()
                .OrderBy(item => item.Nombre)
                .Select(item => new SelectListItem(item.Nombre, item.IdTipoServicio.ToString(), selected == item.IdTipoServicio))
                .ToList();

            items.Insert(0, new SelectListItem("Todos", string.Empty, selected == null));
            return items;
        }

        private List<SelectListItem> CrearConductoresSelectList(int? selected)
        {
            List<SelectListItem> items = conductorService.ListarConductores()
                .OrderBy(item => item.NombreCompleto)
                .Select(item => new SelectListItem(item.NombreCompleto, item.IdConductor.ToString(), selected == item.IdConductor))
                .ToList();

            items.Insert(0, new SelectListItem("Todos", string.Empty, selected == null));
            return items;
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
                6 => "Contraofertado",
                7 => "En camino al pasajero",
                _ => "Estado no identificado"
            };
        }
    }
}

