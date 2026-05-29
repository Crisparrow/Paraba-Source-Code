using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class LiquidacionConductorService
    {
        private readonly LiquidacionConductorRepository liquidacionConductorRepository = new LiquidacionConductorRepository();
        private readonly ComisionServicioService comisionServicioService = new ComisionServicioService();

        public List<LiquidacionConductor> ListarLiquidaciones()
        {
            return liquidacionConductorRepository.Listar();
        }

        public List<int> ListarIdsViajesLiquidados()
        {
            return liquidacionConductorRepository.ListarIdsViajesLiquidados();
        }

        public List<LiquidacionConductorDetalle> ListarDetalles(int idLiquidacionConductor)
        {
            return liquidacionConductorRepository.ListarDetalles(idLiquidacionConductor);
        }

        public void MarcarPagada(int idLiquidacionConductor, string observacion)
        {
            if (idLiquidacionConductor <= 0)
            {
                throw new ArgumentException("Debe seleccionar una liquidacion valida.");
            }

            if (string.IsNullOrWhiteSpace(observacion) || observacion.Trim().Length < 10)
            {
                throw new ArgumentException("Debe ingresar una observacion de pago valida.");
            }

            LiquidacionConductor? liquidacion = ListarLiquidaciones()
                .FirstOrDefault(item => item.IdLiquidacionConductor == idLiquidacionConductor);

            if (liquidacion == null)
            {
                throw new ArgumentException("La liquidacion no existe.");
            }

            if (liquidacion.Estado != "Cerrada")
            {
                throw new ArgumentException("Solo se pueden pagar liquidaciones cerradas.");
            }

            liquidacionConductorRepository.MarcarPagada(idLiquidacionConductor, observacion.Trim());
        }

        public void Anular(int idLiquidacionConductor, string motivo)
        {
            if (idLiquidacionConductor <= 0)
            {
                throw new ArgumentException("Debe seleccionar una liquidacion valida.");
            }

            if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length < 10)
            {
                throw new ArgumentException("Debe ingresar un motivo de anulacion valido.");
            }

            LiquidacionConductor? liquidacion = ListarLiquidaciones()
                .FirstOrDefault(item => item.IdLiquidacionConductor == idLiquidacionConductor);

            if (liquidacion == null)
            {
                throw new ArgumentException("La liquidacion no existe.");
            }

            if (liquidacion.Estado != "Cerrada")
            {
                throw new ArgumentException("Solo se pueden anular liquidaciones cerradas.");
            }

            liquidacionConductorRepository.Anular(idLiquidacionConductor, motivo.Trim());
        }

        public int CerrarLiquidacion(
            int idConductor,
            DateTime fechaDesde,
            DateTime fechaHasta,
            decimal porcentajeComision,
            string usuarioCierre,
            List<Viaje> viajes,
            string observacionCierre = "")
        {
            if (idConductor <= 0)
            {
                throw new ArgumentException("Debe seleccionar un conductor para cerrar la liquidacion.");
            }

            if (fechaDesde == default || fechaHasta == default)
            {
                throw new ArgumentException("Debe seleccionar un rango de fechas.");
            }

            if (fechaDesde.Date > fechaHasta.Date)
            {
                throw new ArgumentException("La fecha desde no puede ser mayor a la fecha hasta.");
            }

            if (porcentajeComision <= 0)
            {
                porcentajeComision = 0;
            }

            var idsViajesLiquidados = liquidacionConductorRepository.ListarIdsViajesLiquidados();

            var viajesLiquidables = viajes
                .Where(item => item.IdConductor == idConductor &&
                    item.IdEstadoViaje == 4 &&
                    item.FechaFin != null &&
                    item.FechaFin.Value.Date >= fechaDesde.Date &&
                    item.FechaFin.Value.Date <= fechaHasta.Date &&
                    !idsViajesLiquidados.Contains(item.IdViaje))
                .ToList();

            if (!viajesLiquidables.Any())
            {
                throw new ArgumentException("No hay viajes finalizados para cerrar esta liquidacion.");
            }

            var comisiones = comisionServicioService.ListarComisiones();

            var detalles = viajesLiquidables.Select(viaje =>
            {
                decimal porcentajeViaje = comisiones
                    .Where(item => item.IdTipoServicio == viaje.IdTipoServicio && item.Estado)
                    .OrderByDescending(item => item.FechaInicioVigencia)
                    .FirstOrDefault()?.PorcentajeComision ?? porcentajeComision;
                decimal comision = Math.Round(viaje.TarifaFinal * (porcentajeViaje / 100), 2);

                return new LiquidacionConductorDetalle
                {
                    IdViaje = viaje.IdViaje,
                    TarifaFinal = viaje.TarifaFinal,
                    ComisionParaba = comision,
                    NetoConductor = viaje.TarifaFinal - comision,
                    FechaRegistro = DateTime.Now
                };
            }).ToList();

            var liquidacion = new LiquidacionConductor
            {
                IdConductor = idConductor,
                FechaDesde = fechaDesde.Date,
                FechaHasta = fechaHasta.Date,
                PorcentajeComision = detalles.Count == 0 ? 0 : Math.Round(detalles.Average(item => item.TarifaFinal == 0 ? 0 : (item.ComisionParaba / item.TarifaFinal) * 100), 2),
                TotalBruto = detalles.Sum(item => item.TarifaFinal),
                TotalComisionParaba = detalles.Sum(item => item.ComisionParaba),
                TotalNetoConductor = detalles.Sum(item => item.NetoConductor),
                Estado = "Cerrada",
                UsuarioCierre = string.IsNullOrWhiteSpace(usuarioCierre) ? "Admin PARABA" : usuarioCierre,
                FechaCierre = DateTime.Now,
                Observacion = string.IsNullOrWhiteSpace(observacionCierre) ? "Liquidacion cerrada por administracion." : observacionCierre.Trim()
            };

            return liquidacionConductorRepository.Crear(liquidacion, detalles);
        }
    }
}
