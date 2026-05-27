using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class ZonaService
    {
        private readonly ZonaRepository zonaRepository = new ZonaRepository();

        public List<Zona> ListarZonas()
        {
            return zonaRepository.Listar();
        }
    }
}
