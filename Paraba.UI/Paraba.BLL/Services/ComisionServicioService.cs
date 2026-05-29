using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class ComisionServicioService
    {
        private readonly ComisionServicioRepository comisionServicioRepository = new ComisionServicioRepository();

        public List<ComisionServicio> ListarComisiones()
        {
            return comisionServicioRepository.Listar();
        }

        public decimal ObtenerPorcentajeActivo(int idTipoServicio)
        {
            return comisionServicioRepository.Listar()
                .Where(item => item.IdTipoServicio == idTipoServicio && item.Estado)
                .OrderByDescending(item => item.FechaInicioVigencia)
                .FirstOrDefault()?.PorcentajeComision ?? 0;
        }

        public void ActualizarPorcentaje(int idComisionServicio, decimal porcentajeComision)
        {
            if (porcentajeComision <= 0)
            {
                throw new ArgumentException("La comision debe ser mayor a cero.");
            }

            comisionServicioRepository.ActualizarPorcentaje(idComisionServicio, porcentajeComision);
        }
    }
}
