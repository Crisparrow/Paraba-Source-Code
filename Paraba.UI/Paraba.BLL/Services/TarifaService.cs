using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class TarifaService
    {
        private readonly TarifaRepository tarifaRepository = new TarifaRepository();

        public List<Tarifa> ListarTarifas()
        {
            return tarifaRepository.Listar();
        }
    }
}
