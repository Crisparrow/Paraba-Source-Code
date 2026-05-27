namespace Paraba.UI.ViewModels
{
    public class ConductorPendienteViewModel
    {
        public int IdConductor { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public bool Disponible { get; set; }

        public bool Verificado { get; set; }

        public bool Estado { get; set; }

        public int DocumentosPendientes { get; set; }

        public int DocumentosAprobados { get; set; }

        public int DocumentosRechazados { get; set; }

        public int TotalDocumentos { get; set; }
    }
}
