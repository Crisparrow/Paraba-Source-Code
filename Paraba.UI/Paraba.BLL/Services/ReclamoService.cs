using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class ReclamoService
    {
        private readonly ReclamoRepository reclamoRepository = new ReclamoRepository();

        public List<Reclamo> ListarReclamos()
        {
            return reclamoRepository.Listar();
        }

        public void Registrar(Reclamo reclamo)
        {
            if (string.IsNullOrWhiteSpace(reclamo.TipoReclamo))
            {
                throw new ArgumentException("Debe ingresar el tipo de reclamo.");
            }

            if (string.IsNullOrWhiteSpace(reclamo.Descripcion) || reclamo.Descripcion.Trim().Length < 10)
            {
                throw new ArgumentException("Debe ingresar una descripcion valida del reclamo.");
            }

            reclamo.TipoReclamo = reclamo.TipoReclamo.Trim();
            reclamo.Descripcion = reclamo.Descripcion.Trim();
            reclamo.Prioridad = string.IsNullOrWhiteSpace(reclamo.Prioridad) ? "Media" : reclamo.Prioridad.Trim();
            reclamo.UsuarioRegistro = string.IsNullOrWhiteSpace(reclamo.UsuarioRegistro) ? "Admin PARABA" : reclamo.UsuarioRegistro.Trim();

            reclamoRepository.Registrar(reclamo);
        }

        public void Cerrar(int idReclamo, string estado, string usuarioCierre, string observacionCierre)
        {
            if (idReclamo <= 0)
            {
                throw new ArgumentException("Debe seleccionar un reclamo valido.");
            }

            if (string.IsNullOrWhiteSpace(observacionCierre) || observacionCierre.Trim().Length < 10)
            {
                throw new ArgumentException("Debe ingresar una observacion de cierre valida.");
            }

            reclamoRepository.Cerrar(idReclamo, string.IsNullOrWhiteSpace(estado) ? "Cerrado" : estado.Trim(), usuarioCierre, observacionCierre.Trim());
        }
    }
}
