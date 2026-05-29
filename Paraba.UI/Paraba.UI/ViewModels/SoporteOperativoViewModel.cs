namespace Paraba.UI.ViewModels
{
    public class SoporteOperativoViewModel
    {
        public int ViajesCancelados { get; set; }

        public int CalificacionesBajas { get; set; }

        public int ViajesSinCierre { get; set; }

        public int ConductoresSuspendidos { get; set; }

        public List<SoporteCasoViewModel> Casos { get; set; } = new List<SoporteCasoViewModel>();
    }
}
