using Paraba.ENTITY.Models;

namespace Paraba.UI.ViewModels
{
    public class SolicitudConductorViewModel
    {
        public SolicitudRegistroConductor Solicitud { get; set; } = new SolicitudRegistroConductor();

        public List<SolicitudRegistroConductorDocumento> Documentos { get; set; } = new List<SolicitudRegistroConductorDocumento>();

        public bool DatosConductorCompletos { get; set; }

        public bool DatosVehiculoCompletos { get; set; }

        public bool DocumentosCompletos { get; set; }
    }
}
