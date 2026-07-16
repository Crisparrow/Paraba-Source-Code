using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class ViajeAppService
    {
        private readonly ViajeRepository viajeRepository = new ViajeRepository();
        private readonly CalculadoraTarifaService calculadoraTarifaService = new CalculadoraTarifaService();
        private readonly AuditoriaViajeService auditoriaViajeService = new AuditoriaViajeService();
        private readonly ConductorService conductorService = new ConductorService();
        private readonly VehiculoService vehiculoService = new VehiculoService();

        public List<Viaje> ListarViajes()
        {
            return viajeRepository.Listar();
        }

        public ResumenOperacionConductor ObtenerResumenOperacion(int idConductor)
        {
            ValidarIdConductor(idConductor);

            ResumenOperacionConductor? resumen = viajeRepository.ObtenerResumenOperacion(idConductor);

            if (resumen == null)
            {
                throw new ArgumentException("Conductor no encontrado.");
            }

            return resumen;
        }

        public List<Viaje> ListarViajesDisponibles(int idConductor)
        {
            ValidarIdConductor(idConductor);

            return viajeRepository.ListarDisponiblesPorConductor(idConductor);
        }

        public List<Viaje> ListarViajesActivos(int idConductor)
        {
            ValidarIdConductor(idConductor);

            return viajeRepository.ListarActivosPorConductor(idConductor);
        }

        public decimal CalcularTarifaSugerida(Tarifa tarifa, decimal distanciaKilometros, int tiempoMinutos)
        {
            decimal subtotal = calculadoraTarifaService.CalcularSubtotal(tarifa, distanciaKilometros, tiempoMinutos);

            return calculadoraTarifaService.AplicarTarifaMinima(subtotal, tarifa.TarifaMinima);
        }

        public void RegistrarSolicitud(Viaje viaje)
        {
            if (viaje.IdPasajero <= 0)
            {
                throw new ArgumentException("Debe seleccionar un pasajero.");
            }

            if (viaje.IdConductor <= 0)
            {
                throw new ArgumentException("Debe seleccionar un conductor.");
            }

            if (viaje.IdVehiculo <= 0)
            {
                throw new ArgumentException("Debe seleccionar un vehiculo.");
            }

            if (viaje.IdTipoServicio <= 0)
            {
                throw new ArgumentException("Debe seleccionar un tipo de servicio.");
            }

            Conductor? conductor = conductorService.ListarConductores()
                .FirstOrDefault(item => item.IdConductor == viaje.IdConductor);

            if (conductor == null || !conductor.Estado)
            {
                throw new ArgumentException("El conductor seleccionado no existe o no esta activo.");
            }

            if (!conductor.Verificado)
            {
                throw new ArgumentException("El conductor seleccionado todavia no esta verificado.");
            }

            if (!conductor.Disponible)
            {
                throw new ArgumentException("El conductor seleccionado no esta disponible.");
            }

            Vehiculo? vehiculo = vehiculoService.ListarVehiculos()
                .FirstOrDefault(item => item.IdVehiculo == viaje.IdVehiculo);

            if (vehiculo == null || !vehiculo.Estado)
            {
                throw new ArgumentException("El vehiculo seleccionado no existe o no esta activo.");
            }

            if (!vehiculo.Verificado)
            {
                throw new ArgumentException("El vehiculo seleccionado todavia no esta verificado.");
            }

            if (vehiculo.IdConductor != viaje.IdConductor)
            {
                throw new ArgumentException("El vehiculo seleccionado no pertenece al conductor.");
            }

            if (vehiculo.IdTipoServicio != viaje.IdTipoServicio)
            {
                throw new ArgumentException("El vehiculo seleccionado no corresponde al tipo de servicio.");
            }

            if (string.IsNullOrWhiteSpace(viaje.Origen))
            {
                throw new ArgumentException("Debe ingresar el origen.");
            }

            if (string.IsNullOrWhiteSpace(viaje.Destino))
            {
                throw new ArgumentException("Debe ingresar el destino.");
            }

            if (viaje.TarifaSugerida <= 0)
            {
                throw new ArgumentException("La tarifa sugerida debe ser mayor a cero.");
            }

            if (viaje.TarifaOfertada <= 0)
            {
                viaje.TarifaOfertada = viaje.TarifaSugerida;
            }

            viaje.IdEstadoViaje = 1;
            viaje.TarifaEstimada = viaje.TarifaSugerida;
            viaje.TarifaFinal = 0;
            viaje.TarifaAceptada = null;
            viaje.FechaSolicitud = DateTime.Now;
            viaje.FechaInicio = null;
            viaje.FechaFin = null;

            viajeRepository.RegistrarSolicitud(viaje);
            RegistrarAuditoriaUltimoViaje("Solicitud creada", "Nuevo", "Solicitado", null, viaje.TarifaOfertada, "Solicitud registrada desde el panel administrativo.");
        }

        public void RegistrarContraoferta(int idViaje, decimal tarifaContraoferta)
        {
            if (idViaje <= 0)
            {
                throw new ArgumentException("Debe seleccionar un viaje valido.");
            }

            if (tarifaContraoferta <= 0)
            {
                throw new ArgumentException("La contraoferta debe ser mayor a cero.");
            }

            Viaje viaje = ObtenerViajeValido(idViaje);

            if (viaje.IdEstadoViaje != 1)
            {
                throw new ArgumentException("Solo se puede contraofertar un viaje solicitado.");
            }

            viajeRepository.RegistrarContraoferta(idViaje, tarifaContraoferta);
            RegistrarAuditoria(idViaje, "Contraoferta", ObtenerNombreEstado(viaje.IdEstadoViaje), "Solicitado", viaje.TarifaContraoferta, tarifaContraoferta, "El conductor registro una contraoferta.");
        }

        public void AceptarViaje(int idConductor, int idViaje)
        {
            ValidarIdConductor(idConductor);
            ValidarIdViaje(idViaje);
            EjecutarOperacionConductor(() => viajeRepository.AceptarViaje(idConductor, idViaje));
        }

        public int CrearSolicitudDemo(int idConductor, int? idTipoServicio)
        {
            ValidarIdConductor(idConductor);

            if (idTipoServicio != null && idTipoServicio <= 0)
            {
                throw new ArgumentException("Debe seleccionar un tipo de servicio valido.");
            }

            return EjecutarOperacionConductorConResultado(() => viajeRepository.CrearSolicitudDemo(idConductor, idTipoServicio));
        }

        public void RegistrarContraoferta(int idConductor, int idViaje, decimal tarifaContraoferta)
        {
            ValidarIdConductor(idConductor);
            ValidarIdViaje(idViaje);

            if (tarifaContraoferta <= 0)
            {
                throw new ArgumentException("La contraoferta debe ser mayor a cero.");
            }

            EjecutarOperacionConductor(() => viajeRepository.RegistrarContraoferta(idConductor, idViaje, tarifaContraoferta));
        }

        public void AceptarContraofertaPasajeroDemo(int idConductor, int idViaje)
        {
            ValidarIdConductor(idConductor);
            ValidarIdViaje(idViaje);

            Viaje? viaje = ListarViajes().FirstOrDefault(item => item.IdViaje == idViaje);

            if (viaje == null)
            {
                throw new ArgumentException("El viaje no existe.");
            }

            if (viaje.IdConductor != idConductor)
            {
                throw new ArgumentException("El viaje no pertenece al conductor.");
            }

            EjecutarOperacionConductor(() => viajeRepository.AceptarContraofertaPasajero(idViaje));
        }

        public void AceptarContraoferta(int idViaje)
        {
            if (idViaje <= 0)
            {
                throw new ArgumentException("Debe seleccionar un viaje valido.");
            }

            Viaje? viaje = ListarViajes().FirstOrDefault(item => item.IdViaje == idViaje);

            if (viaje == null)
            {
                throw new ArgumentException("El viaje no existe.");
            }

            if (viaje.TarifaContraoferta == null)
            {
                throw new ArgumentException("El viaje no tiene contraoferta para aceptar.");
            }

            if (viaje.IdEstadoViaje != 1)
            {
                throw new ArgumentException("Solo se puede aceptar una contraoferta de un viaje solicitado.");
            }

            viajeRepository.AceptarContraoferta(idViaje);
            RegistrarAuditoria(idViaje, "Contraoferta aceptada", ObtenerNombreEstado(viaje.IdEstadoViaje), "Aceptado", viaje.TarifaAceptada, viaje.TarifaContraoferta, "El pasajero acepto la contraoferta del conductor.");
        }

        public void IniciarViaje(int idViaje)
        {
            Viaje viaje = ObtenerViajeValido(idViaje);

            if (viaje.IdEstadoViaje != 2)
            {
                throw new ArgumentException("Solo se puede iniciar un viaje aceptado.");
            }

            viajeRepository.IniciarViaje(idViaje);
            RegistrarAuditoria(idViaje, "Viaje iniciado", "Aceptado", "En curso", viaje.TarifaAceptada, viaje.TarifaAceptada, "El viaje paso a estado en curso.");
        }

        public void IniciarViaje(int idConductor, int idViaje)
        {
            ValidarIdConductor(idConductor);
            ValidarIdViaje(idViaje);
            EjecutarOperacionConductor(() => viajeRepository.IniciarViaje(idConductor, idViaje));
        }

        public void FinalizarViaje(int idViaje)
        {
            Viaje viaje = ObtenerViajeValido(idViaje);

            if (viaje.IdEstadoViaje != 3)
            {
                throw new ArgumentException("Solo se puede finalizar un viaje en curso.");
            }

            viajeRepository.FinalizarViaje(idViaje);
            RegistrarAuditoria(idViaje, "Viaje finalizado", "En curso", "Finalizado", viaje.TarifaAceptada, viaje.TarifaAceptada, "El viaje fue finalizado.");
        }

        public void FinalizarViaje(int idConductor, int idViaje)
        {
            ValidarIdConductor(idConductor);
            ValidarIdViaje(idViaje);
            EjecutarOperacionConductor(() => viajeRepository.FinalizarViaje(idConductor, idViaje));
        }

        public void CancelarViaje(int idViaje, string motivo)
        {
            Viaje viaje = ObtenerViajeValido(idViaje);

            if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length < 10)
            {
                throw new ArgumentException("Debe ingresar un motivo administrativo valido.");
            }

            if (viaje.IdEstadoViaje == 4 || viaje.IdEstadoViaje == 5)
            {
                throw new ArgumentException("No se puede cancelar un viaje finalizado o ya cancelado.");
            }

            viajeRepository.CancelarViaje(idViaje);
            RegistrarAuditoria(idViaje, "Viaje cancelado", ObtenerNombreEstado(viaje.IdEstadoViaje), "Cancelado", viaje.TarifaAceptada, viaje.TarifaAceptada, motivo.Trim());
        }

        public void CancelarViaje(int idConductor, int idViaje, string motivo)
        {
            ValidarIdConductor(idConductor);
            ValidarIdViaje(idViaje);

            if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length < 10)
            {
                throw new ArgumentException("Debe ingresar un motivo de cancelacion de al menos 10 caracteres.");
            }

            EjecutarOperacionConductor(() => viajeRepository.CancelarViaje(idConductor, idViaje, motivo.Trim()));
        }

        private Viaje ObtenerViajeValido(int idViaje)
        {
            if (idViaje <= 0)
            {
                throw new ArgumentException("Debe seleccionar un viaje valido.");
            }

            Viaje? viaje = ListarViajes().FirstOrDefault(item => item.IdViaje == idViaje);

            if (viaje == null)
            {
                throw new ArgumentException("El viaje no existe.");
            }

            return viaje;
        }

        private static void ValidarIdConductor(int idConductor)
        {
            if (idConductor <= 0)
            {
                throw new ArgumentException("Debe seleccionar un conductor valido.");
            }
        }

        private static void ValidarIdViaje(int idViaje)
        {
            if (idViaje <= 0)
            {
                throw new ArgumentException("Debe seleccionar un viaje valido.");
            }
        }

        private static void EjecutarOperacionConductor(Action operacion)
        {
            try
            {
                operacion();
            }
            catch (Exception ex) when (ex.GetType().Name == "SqlException")
            {
                throw new ArgumentException(ex.Message, ex);
            }
        }

        private static T EjecutarOperacionConductorConResultado<T>(Func<T> operacion)
        {
            try
            {
                return operacion();
            }
            catch (Exception ex) when (ex.GetType().Name == "SqlException")
            {
                throw new ArgumentException(ex.Message, ex);
            }
        }

        private void RegistrarAuditoria(
            int idViaje,
            string accion,
            string estadoAnterior,
            string estadoNuevo,
            decimal? tarifaAnterior,
            decimal? tarifaNueva,
            string observacion)
        {
            auditoriaViajeService.RegistrarAuditoria(new AuditoriaViaje
            {
                IdViaje = idViaje,
                Accion = accion,
                EstadoAnterior = estadoAnterior,
                EstadoNuevo = estadoNuevo,
                TarifaAnterior = tarifaAnterior,
                TarifaNueva = tarifaNueva,
                UsuarioSistema = "Admin PARABA",
                Observacion = observacion
            });
        }

        private void RegistrarAuditoriaUltimoViaje(
            string accion,
            string estadoAnterior,
            string estadoNuevo,
            decimal? tarifaAnterior,
            decimal? tarifaNueva,
            string observacion)
        {
            Viaje? viaje = ListarViajes().OrderByDescending(item => item.IdViaje).FirstOrDefault();

            if (viaje == null)
            {
                return;
            }

            RegistrarAuditoria(viaje.IdViaje, accion, estadoAnterior, estadoNuevo, tarifaAnterior, tarifaNueva, observacion);
        }

        private static string ObtenerNombreEstado(int idEstadoViaje)
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
                _ => "Desconocido"
            };
        }
    }
}
