using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class ReglaTarifaService
    {
        private readonly ReglaTarifaRepository reglaTarifaRepository = new ReglaTarifaRepository();

        public List<ReglaTarifa> ListarReglasTarifa()
        {
            return reglaTarifaRepository.Listar();
        }
    }
}
