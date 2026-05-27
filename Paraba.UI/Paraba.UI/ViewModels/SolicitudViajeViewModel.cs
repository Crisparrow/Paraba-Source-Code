using Microsoft.AspNetCore.Mvc.Rendering;

namespace Paraba.UI.ViewModels
{
    public class SolicitudViajeViewModel
    {
        public int IdPasajero { get; set; }

        public int IdConductor { get; set; }

        public int IdVehiculo { get; set; }

        public int IdTipoServicio { get; set; }

        public string Origen { get; set; } = string.Empty;

        public string Destino { get; set; } = string.Empty;

        public decimal DistanciaKilometros { get; set; }

        public int TiempoMinutos { get; set; }

        public decimal TarifaSugerida { get; set; }

        public decimal TarifaOfertada { get; set; }

        public List<SelectListItem> Pasajeros { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> Conductores { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> Vehiculos { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> TiposServicio { get; set; } = new List<SelectListItem>();
    }
}
