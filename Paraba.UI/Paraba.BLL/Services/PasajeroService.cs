using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class PasajeroService
    {
        private readonly PasajeroRepository pasajeroRepository = new PasajeroRepository();

        public List<Pasajero> ListarPasajeros()
        {
            return pasajeroRepository.Listar();
        }
    }
}
