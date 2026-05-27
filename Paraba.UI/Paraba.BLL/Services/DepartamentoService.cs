using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class DepartamentoService
    {
        private readonly DepartamentoRepository departamentoRepository = new DepartamentoRepository();

        public List<Departamento> ListarDepartamentos()
        {
            return departamentoRepository.Listar();
        }
    }
}
