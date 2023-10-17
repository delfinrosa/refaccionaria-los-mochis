using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapaNegocio;
namespace RefaccionariaLosMochis.Permisos
{
    public class PermisosRolAttribute : ActionFilterAttribute
    {
            private string rol;


            public PermisosRolAttribute(string _rol)
            {

                rol = _rol;
            }


        
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (HttpContext.Current.Session["Usuario"] != null)
                {
                    Usuario usuario = HttpContext.Current.Session["Usuario"] as Usuario;

                    if (usuario.Tipo != "A")
                    {

                        if (usuario.Tipo != this.rol )
                        {

                            filterContext.Result = new RedirectResult("~/Home/index");

                        }
                    }


                }



                base.OnActionExecuting(filterContext);
            }


        }
}