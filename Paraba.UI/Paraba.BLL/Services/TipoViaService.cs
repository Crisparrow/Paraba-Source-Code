using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class TipoViaService
    {
        private readonly TipoViaRepository tipoViaRepository = new TipoViaRepository();

        public List<TipoVia> ListarTiposVia()
        {
            return tipoViaRepository.Listar();
        }
    }
}
