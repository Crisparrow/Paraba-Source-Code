using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class VehiculoService
    {
        private readonly VehiculoRepository vehiculoRepository = new VehiculoRepository();

        public List<Vehiculo> ListarVehiculos()
        {
            return vehiculoRepository.Listar();
        }
    }
}
