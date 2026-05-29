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

        public void ActualizarOperacion(Zona zona)
        {
            if (zona.IdZona <= 0)
            {
                throw new ArgumentException("Debe seleccionar una zona valida.");
            }

            zona.ObservacionOperativa = zona.ObservacionOperativa?.Trim() ?? string.Empty;
            zonaRepository.ActualizarOperacion(zona);
        }
    }
}
