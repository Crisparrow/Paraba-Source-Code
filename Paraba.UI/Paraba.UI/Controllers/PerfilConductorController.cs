using Microsoft.AspNetCore.Mvc;
using Paraba.BLL.Services;
using Paraba.UI.ViewModels;

namespace Paraba.UI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Verificador,Soporte")]
    public class PerfilConductorController : Controller
    {
        private readonly ConductorService conductorService = new ConductorService();
        private readonly VehiculoService vehiculoService = new VehiculoService();
        private readonly CalificacionService calificacionService = new CalificacionService();

        public IActionResult Index()
        {
            var conductores = conductorService.ListarConductores();
            var vehiculos = vehiculoService.ListarVehiculos();
            var calificaciones = calificacionService.ListarCalificaciones();

            var perfiles = conductores.Select(conductor =>
            {
                var vehiculo = vehiculos.FirstOrDefault(item => item.IdConductor == conductor.IdConductor);
                var calificacionesConductor = calificaciones
                    .Where(item => item.IdConductor == conductor.IdConductor)
                    .ToList();

                return new PerfilConductorViewModel
                {
                    IdConductor = conductor.IdConductor,
                    NombreCompleto = conductor.NombreCompleto,
                    Telefono = conductor.Telefono,
                    Correo = conductor.Correo,
                    TipoServicio = ObtenerTipoServicio(vehiculo?.IdTipoServicio ?? 0),
                    Vehiculo = vehiculo == null ? "Sin vehiculo registrado" : $"{vehiculo.Marca} {vehiculo.Modelo}",
                    Placa = vehiculo?.Placa ?? "Sin placa",
                    ConductorVerificado = conductor.Verificado,
                    VehiculoVerificado = vehiculo?.Verificado ?? false,
                    Disponible = conductor.Disponible,
                    PromedioCalificacion = CalcularPromedio(calificacionesConductor),
                    TotalCalificaciones = calificacionesConductor.Count
                };
            }).ToList();

            return View(perfiles);
        }

        private static string ObtenerTipoServicio(int idTipoServicio)
        {
            return idTipoServicio switch
            {
                1 => "Taxi",
                2 => "Moto taxi",
                _ => "Sin modalidad"
            };
        }

        private static decimal CalcularPromedio(List<ENTITY.Models.Calificacion> calificaciones)
        {
            if (calificaciones.Count == 0)
            {
                return 0;
            }

            return Math.Round((decimal)calificaciones.Average(item => item.Puntaje), 2);
        }
    }
}
