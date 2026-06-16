using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;
using System.Security.Claims;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Verificador,Soporte")]
    public class ConductorController : Controller
    {
        private readonly ConductorService conductorService = new ConductorService();
        private readonly DocumentoConductorService documentoConductorService = new DocumentoConductorService();
        private readonly VehiculoService vehiculoService = new VehiculoService();
        private readonly TipoServicioService tipoServicioService = new TipoServicioService();
        private readonly CalificacionService calificacionService = new CalificacionService();
        private readonly PasajeroService pasajeroService = new PasajeroService();
        private readonly AuditoriaConductorService auditoriaConductorService = new AuditoriaConductorService();
        private readonly AuditoriaAdministrativaService auditoriaAdministrativaService = new AuditoriaAdministrativaService();

        public IActionResult Index()
        {
            var conductores = conductorService.ListarConductores();

            return View(conductores);
        }

        public IActionResult Detalle(int id)
        {
            var conductor = conductorService.ListarConductores().FirstOrDefault(item => item.IdConductor == id);

            if (conductor == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var vehiculo = vehiculoService.ListarVehiculos().FirstOrDefault(item => item.IdConductor == conductor.IdConductor);
            var tipoServicio = vehiculo == null
                ? "Sin modalidad"
                : tipoServicioService.ListarTiposServicio().FirstOrDefault(item => item.IdTipoServicio == vehiculo.IdTipoServicio)?.Nombre ?? "Tipo no identificado";
            var pasajeros = pasajeroService.ListarPasajeros();

            var documentos = documentoConductorService.ListarDocumentos()
                .Where(item => item.IdConductor == conductor.IdConductor)
                .Select(item => new DocumentoConductorViewModel
                {
                    IdDocumentoConductor = item.IdDocumentoConductor,
                    Conductor = conductor.NombreCompleto,
                    TipoDocumento = item.TipoDocumento,
                    NumeroDocumento = item.NumeroDocumento,
                    UrlArchivo = item.UrlArchivo,
                    FechaVencimiento = item.FechaVencimiento,
                    EstadoVerificacion = item.EstadoVerificacion,
                    Observacion = item.Observacion,
                    FechaRegistro = item.FechaRegistro
                })
                .ToList();

            var calificaciones = calificacionService.ListarCalificaciones()
                .Where(item => item.IdConductor == conductor.IdConductor)
                .OrderByDescending(item => item.FechaRegistro)
                .Select(item => new CalificacionViewModel
                {
                    IdCalificacion = item.IdCalificacion,
                    IdViaje = item.IdViaje,
                    Pasajero = pasajeros.FirstOrDefault(pasajero => pasajero.IdPasajero == item.IdPasajero)?.NombreCompleto ?? "Pasajero no identificado",
                    Conductor = conductor.NombreCompleto,
                    Puntaje = item.Puntaje,
                    Comentario = item.Comentario,
                    Estado = item.Estado,
                    FechaRegistro = item.FechaRegistro
                })
                .ToList();

            var auditorias = auditoriaConductorService.ListarAuditoriaConductores()
                .Where(item => item.IdConductor == conductor.IdConductor)
                .OrderByDescending(item => item.FechaRegistro)
                .Select(item => new AuditoriaConductorViewModel
                {
                    IdAuditoriaConductor = item.IdAuditoriaConductor,
                    IdConductor = item.IdConductor,
                    Conductor = conductor.NombreCompleto,
                    Accion = item.Accion,
                    EstadoAnterior = item.EstadoAnterior,
                    EstadoNuevo = item.EstadoNuevo,
                    UsuarioSistema = item.UsuarioSistema,
                    Observacion = item.Observacion,
                    FechaRegistro = item.FechaRegistro
                })
                .ToList();

            var viewModel = new DetalleConductorViewModel
            {
                IdConductor = conductor.IdConductor,
                NombreCompleto = conductor.NombreCompleto,
                DocumentoIdentidad = conductor.DocumentoIdentidad,
                Telefono = conductor.Telefono,
                Correo = conductor.Correo,
                LicenciaConducir = conductor.LicenciaConducir,
                FechaVencimientoLicencia = conductor.FechaVencimientoLicencia,
                Disponible = conductor.Disponible,
                Verificado = conductor.Verificado,
                Estado = conductor.Estado,
                FechaRegistro = conductor.FechaRegistro,
                TipoServicio = tipoServicio,
                Vehiculo = vehiculo == null ? "Sin vehiculo registrado" : $"{vehiculo.Marca} {vehiculo.Modelo}",
                Placa = vehiculo?.Placa ?? "-",
                Color = vehiculo?.Color ?? "-",
                VehiculoVerificado = vehiculo?.Verificado ?? false,
                PromedioCalificacion = calificaciones.Count == 0 ? 0 : Math.Round((decimal)calificaciones.Average(item => item.Puntaje), 2),
                Documentos = documentos,
                Calificaciones = calificaciones,
                Auditorias = auditorias
            };

            return View(viewModel);
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

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Verificador")]
        public IActionResult Suspender(int id)
        {
            var viewModel = CrearIntervencionConductorViewModel(id, "Suspender");

            if (viewModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View("Intervencion", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Verificador")]
        public IActionResult ConfirmarSuspension(IntervencionConductorViewModel viewModel)
        {
            return ConfirmarIntervencion(viewModel, "Suspender");
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Verificador")]
        public IActionResult Reactivar(int id)
        {
            var viewModel = CrearIntervencionConductorViewModel(id, "Reactivar");

            if (viewModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View("Intervencion", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Verificador")]
        public IActionResult ConfirmarReactivacion(IntervencionConductorViewModel viewModel)
        {
            return ConfirmarIntervencion(viewModel, "Reactivar");
        }

        private IActionResult ConfirmarIntervencion(IntervencionConductorViewModel viewModel, string accion)
        {
            var datosConductor = CrearIntervencionConductorViewModel(viewModel.IdConductor, accion);

            if (datosConductor == null)
            {
                return RedirectToAction(nameof(Index));
            }

            viewModel.Conductor = datosConductor.Conductor;
            viewModel.DocumentoIdentidad = datosConductor.DocumentoIdentidad;
            viewModel.EstadoActual = datosConductor.EstadoActual;
            viewModel.Accion = datosConductor.Accion;

            if (!ModelState.IsValid)
            {
                return View("Intervencion", viewModel);
            }

            try
            {
                if (accion == "Suspender")
                {
                    conductorService.SuspenderConductor(viewModel.IdConductor, viewModel.Motivo);
                    auditoriaAdministrativaService.Registrar("Conductores", "Conductor suspendido", "Conductor", viewModel.IdConductor, ObtenerUsuario(), viewModel.Motivo);
                }
                else
                {
                    conductorService.ReactivarConductor(viewModel.IdConductor, viewModel.Motivo);
                    auditoriaAdministrativaService.Registrar("Conductores", "Conductor reactivado", "Conductor", viewModel.IdConductor, ObtenerUsuario(), viewModel.Motivo);
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Intervencion", viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        private IntervencionConductorViewModel? CrearIntervencionConductorViewModel(int idConductor, string accion)
        {
            var conductor = conductorService.ListarConductores().FirstOrDefault(item => item.IdConductor == idConductor);

            if (conductor == null)
            {
                return null;
            }

            if (accion == "Suspender" && !conductor.Estado)
            {
                return null;
            }

            if (accion == "Reactivar" && conductor.Estado)
            {
                return null;
            }

            return new IntervencionConductorViewModel
            {
                IdConductor = conductor.IdConductor,
                Conductor = conductor.NombreCompleto,
                DocumentoIdentidad = conductor.DocumentoIdentidad,
                EstadoActual = conductor.Estado ? "Activo" : "Suspendido",
                Accion = accion
            };
        }

        private string ObtenerUsuario()
        {
            return User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "Admin PARABA";
        }
    }
}
