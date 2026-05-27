using Microsoft.AspNetCore.Mvc.Rendering;

namespace Paraba.UI.ViewModels
{
    public class CrearUsuarioAdminViewModel
    {
        public string NombreCompleto { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int IdRolAdmin { get; set; }

        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
    }
}
