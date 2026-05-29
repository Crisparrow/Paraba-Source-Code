using Microsoft.AspNetCore.Mvc.Rendering;

namespace Paraba.UI.ViewModels
{
    public class DocumentoConductorFiltroViewModel
    {
        public string Buscar { get; set; } = string.Empty;

        public string EstadoVerificacion { get; set; } = string.Empty;

        public bool SoloVencidos { get; set; }

        public int TotalDocumentos { get; set; }

        public int TotalPendientes { get; set; }

        public int TotalAprobados { get; set; }

        public int TotalRechazados { get; set; }

        public int TotalVencidos { get; set; }

        public List<SelectListItem> Estados { get; set; } = new List<SelectListItem>();

        public List<DocumentoConductorViewModel> Documentos { get; set; } = new List<DocumentoConductorViewModel>();
    }
}
