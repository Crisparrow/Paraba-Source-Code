using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class CiudadService
    {
        private readonly CiudadRepository ciudadRepository = new CiudadRepository();

        public List<Ciudad> ListarCiudades()
        {
            return ciudadRepository.Listar();
        }
    }
}
