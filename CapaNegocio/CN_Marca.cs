using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;
namespace CapaNegocio
{
    public class CN_Marca
    {

        private CD_Marca ObjCapaDatos = new CD_Marca();
        public List<Marca> Listar()
        {
            return ObjCapaDatos.Listar();
        }
        public int Registrar(Marca obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            return ObjCapaDatos.Registrar(obj, out Mensaje);
        }
        public bool Editar(Marca obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            return ObjCapaDatos.Editar(obj, out Mensaje);
        }
        public bool Eliminar(int id, out string Mensaje)
        {
            return ObjCapaDatos.Eliminar(id, out Mensaje);
        }
        ////pruebas
        public Marca BusquedaFiltroMarca(string nombre)
        {
            return ObjCapaDatos.BuscarPorNombre(nombre);
        }

        public List<string> ListarNombresDeMarcas(string nombre)
        {
            return ObjCapaDatos.ListarNombresDeMarcas(nombre);
        }

        public Marca BuscarPorId(int id)
        {
            return ObjCapaDatos.BuscarPorId(id);
        }

        public Marca UltimoRegistroMarca()
        {
            return ObjCapaDatos.UltimoRegistro();
        }

        public List<Marca> PruebasAutoCompletado()
        {
            return ObjCapaDatos.PruebasAutoCompletado();
        }

        public int COUNT_PruebasAutoCompletadoMarca(string nombre)
        {
            return ObjCapaDatos.ContarPruebasAutoCompletado(nombre);
        }

        public List<string> PaginacionPRUEBAMarca(string nombre, int pagina)
        {
            return ObjCapaDatos.PaginacionPruebasAutoCompletado(nombre, pagina);
        }

        public int COUNT_TablaMarca()
        {
            return ObjCapaDatos.ContarMarcas();
        }

        public List<Marca> ListarPruebaMarca(int pagina)
        {
            return ObjCapaDatos.ListarMarcas(pagina);
        }

    }
}
