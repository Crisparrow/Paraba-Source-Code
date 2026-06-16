using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class ViajeAdminService
    {
        private readonly ViajeRepository viajeRepository = new ViajeRepository();
        private readonly AuditoriaViajeService auditoriaViajeService = new AuditoriaViajeService();

        public List<Viaje> ListarViajes()
        {
            return viajeRepository.Listar();
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
            RegistrarAuditoria(
                idViaje,
                "Viaje cancelado",
                ObtenerNombreEstado(viaje.IdEstadoViaje),
                "Cancelado",
                viaje.TarifaAceptada,
                viaje.TarifaAceptada,
                motivo.Trim());
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

        private static string ObtenerNombreEstado(int idEstadoViaje)
        {
            return idEstadoViaje switch
            {
                1 => "Solicitado",
                2 => "Aceptado",
                3 => "En curso",
                4 => "Finalizado",
                5 => "Cancelado",
                _ => "Desconocido"
            };
        }
    }
}
