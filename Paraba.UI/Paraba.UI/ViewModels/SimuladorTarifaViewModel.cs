using Microsoft.AspNetCore.Mvc.Rendering;

namespace Paraba.UI.ViewModels
{
    public class SimuladorTarifaViewModel
    {
        public int IdTipoServicio { get; set; }

        public int IdZona { get; set; }

        public int IdTipoVia { get; set; }

        public decimal DistanciaKilometros { get; set; }

        public int TiempoMinutos { get; set; }

        public bool AplicaLluvia { get; set; }

        public bool AplicaAltaDemanda { get; set; }

        public bool AplicaHorarioNocturno { get; set; }

        public decimal TarifaBase { get; set; }

        public decimal CostoDistancia { get; set; }

        public decimal CostoTiempo { get; set; }

        public decimal IncrementoReglas { get; set; }

        public decimal IncrementoTipoVia { get; set; }

        public decimal TarifaEstimada { get; set; }

        public bool TieneResultado { get; set; }

        public List<SelectListItem> TiposServicio { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> Zonas { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> TiposVia { get; set; } = new List<SelectListItem>();
    }
}
