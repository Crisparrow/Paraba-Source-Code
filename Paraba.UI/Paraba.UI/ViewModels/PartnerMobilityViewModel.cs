using Paraba.ENTITY.Models;

namespace Paraba.UI.ViewModels;

public sealed class PartnerMobilityViewModel
{
    public List<RutaMicrobus> RutasMicrobus { get; set; } = new();
    public List<AsociacionMototaxi> AsociacionesMototaxi { get; set; } = new();
    public int TotalRutasActivas => RutasMicrobus.Count(item => item.Estado);
    public int TotalChoferesMicrobus => RutasMicrobus.Sum(item => item.ChoferesSuscritos);
    public int TotalCuposMototaxi => AsociacionesMototaxi.Sum(item => item.CuposTotales);
    public int CuposMototaxiDisponibles => AsociacionesMototaxi.Sum(item => item.CuposDisponibles);
}
