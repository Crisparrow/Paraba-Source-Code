namespace Paraba.ENTITY.Models;

public sealed class RutaMicrobus
{
    public int IdRutaMicrobus { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Origen { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public string Recorrido { get; set; } = string.Empty;
    public decimal TarifaPasajeBs { get; set; }
    public decimal SuscripcionMensualChoferUsd { get; set; }
    public int ChoferesSuscritos { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public sealed class AsociacionMototaxi
{
    public int IdAsociacionMototaxi { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Parada { get; set; } = string.Empty;
    public decimal CostoMensualUsd { get; set; }
    public int CuposTotales { get; set; }
    public int CuposOcupados { get; set; }
    public int CuposDisponibles => Math.Max(CuposTotales - CuposOcupados, 0);
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; }
}
