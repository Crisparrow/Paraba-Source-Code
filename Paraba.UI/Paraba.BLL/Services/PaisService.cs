using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class PaisService
    {
        private readonly PaisRepository paisRepository = new PaisRepository();

        public List<Pais> ListarPaises()
        {
            return paisRepository.Listar();
        }
    }
}
