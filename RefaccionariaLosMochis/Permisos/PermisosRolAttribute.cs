using CapaEntidad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RefaccionariaLosMochis.Permisos
{
    public class PermisosRolAttribute : ActionFilterAttribute
    {
        private readonly string _rol;

        public PermisosRolAttribute(string rol)
        {
            _rol = rol;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var usuario = context.HttpContext.Session.GetObject<Usuario>("Usuario"); // Método que debes implementar para obtener el objeto de la sesión

            if (usuario != null)
            {
                if (usuario.Tipo != "A" && usuario.Tipo != _rol)
                {
                    context.Result = new RedirectToActionResult("Index", "Acceso", null);
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
