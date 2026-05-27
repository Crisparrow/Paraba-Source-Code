using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services
{
    public class CalculadoraTarifaService
    {
        public decimal CalcularSubtotal(Tarifa tarifa, decimal distanciaKilometros, int tiempoMinutos)
        {
            return (tarifa.CostoPorKilometro * distanciaKilometros)
                + (tarifa.CostoPorMinuto * tiempoMinutos);
        }

        public decimal CalcularIncrementoPorcentual(decimal montoBase, decimal porcentaje)
        {
            return montoBase * (porcentaje / 100);
        }

        public decimal AplicarTarifaMinima(decimal monto, decimal tarifaMinima)
        {
            return monto < tarifaMinima ? tarifaMinima : monto;
        }
    }
}
