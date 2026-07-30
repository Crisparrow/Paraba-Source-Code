using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class VehiculoService
    {
        private readonly VehiculoRepository vehiculoRepository = new VehiculoRepository();
        private readonly PerfilConductorRepository perfilConductorRepository = new PerfilConductorRepository();
        private readonly AuditoriaConductorService auditoriaConductorService = new AuditoriaConductorService();

        public List<Vehiculo> ListarVehiculos()
        {
            return vehiculoRepository.Listar();
        }

        public bool AprobarVehiculo(int idVehiculo, string observacion)
        {
            return RevisarVehiculo(idVehiculo, "Aprobado", observacion);
        }

        public bool RechazarVehiculo(int idVehiculo, string observacion)
        {
            if (string.IsNullOrWhiteSpace(observacion) || observacion.Trim().Length < 10)
            {
                throw new ArgumentException("Debe ingresar un motivo de rechazo valido.");
            }

            return RevisarVehiculo(idVehiculo, "Rechazado", observacion);
        }

        private bool RevisarVehiculo(int idVehiculo, string estado, string observacion)
        {
            Vehiculo? vehiculo = vehiculoRepository.Listar().FirstOrDefault(item => item.IdVehiculo == idVehiculo);

            if (vehiculo == null)
            {
                throw new ArgumentException("El vehiculo no existe.");
            }

            if (vehiculo.EstadoVerificacion != "Pendiente")
            {
                throw new ArgumentException("Solo se pueden revisar vehiculos pendientes.");
            }

            string detalle = string.IsNullOrWhiteSpace(observacion)
                ? $"Vehiculo {estado.ToLowerInvariant()} por administracion."
                : observacion.Trim();
            bool actualizado = vehiculoRepository.ActualizarEstadoVerificacion(idVehiculo, estado, detalle);
            perfilConductorRepository.RecalcularAprobacion(vehiculo.IdConductor);
            auditoriaConductorService.RegistrarAuditoria(new AuditoriaConductor
            {
                IdConductor = vehiculo.IdConductor,
                Accion = $"Vehiculo {estado.ToLowerInvariant()}",
                EstadoAnterior = vehiculo.EstadoVerificacion,
                EstadoNuevo = estado,
                UsuarioSistema = "Admin PARABA",
                Observacion = $"{vehiculo.Placa}: {detalle}"
            });
            return actualizado;
        }
    }
}
