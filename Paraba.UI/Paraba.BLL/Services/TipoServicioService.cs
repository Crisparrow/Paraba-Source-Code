using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class TipoServicioService
    {
        private readonly TipoServicioRepository tipoServicioRepository = new TipoServicioRepository();

        public List<TipoServicio> ListarTiposServicio()
        {
            return tipoServicioRepository.Listar();
        }
    }
}
