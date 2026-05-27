using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class EstadoViajeService
    {
        private readonly EstadoViajeRepository estadoViajeRepository = new EstadoViajeRepository();

        public List<EstadoViaje> ListarEstadosViaje()
        {
            return estadoViajeRepository.Listar();
        }
    }
}
