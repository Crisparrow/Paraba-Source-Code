using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class CalificacionService
    {
        private readonly CalificacionRepository calificacionRepository = new CalificacionRepository();

        public List<Calificacion> ListarCalificaciones()
        {
            return calificacionRepository.Listar();
        }
    }
}
