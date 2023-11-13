using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Linea
    {

        private CD_Linea ObjCapaDatos = new CD_Linea();
        public List<Linea> Listar()
        {
            return ObjCapaDatos.Listar();
        }
        public int Registrar(Linea obj, out string Mensaje, out Linea objDevolucion)
        {
            Mensaje = string.Empty;
            objDevolucion = null;
            return ObjCapaDatos.Registrar(obj, out Mensaje, out objDevolucion);
        }
        public bool Editar(Linea obj, out string Mensaje, out Linea objDevolucion)
        {
            Mensaje = string.Empty;
            objDevolucion = null;
            return ObjCapaDatos.Editar(obj, out Mensaje, out objDevolucion);

        }
        //Elimina
        public bool Eliminar(int id, out string Mensaje)
        {
            return ObjCapaDatos.Eliminar(id, out Mensaje);
        }
        // Buscar Una Linea por el nombre (Devuelve un objeto)
        public Linea BusquedaFiltroLinea(string nombre)
        {
            return ObjCapaDatos.BusquedaFiltroLinea(nombre);
        }
        // Buscar Una Linea por el nombre (Devuelve una List<objeto>)

        public List<string> ListarNombreDeLineas(string linea)
        {
            return ObjCapaDatos.ListarNombreDeLineas(linea);
        }
        // Buscar Una Linea por el ID (Devuelve un objeto)
        public Linea ListarPorIdLineas(int Id)
        {
            return ObjCapaDatos.BusquedaIDLinea(Id);
        }


        // Buscar la ultimo modificacion de la linea
        public Linea UltimoRegistro()
        {
            return ObjCapaDatos.UltimoRegistro();
        }
        //PRUEBAS AUTOCOMPLETADO
        public List<Linea> PruebasAutoCompletado()
        {
            return ObjCapaDatos.PruebasAutoCompletado();
        }

        /// <summary>
        /// COUNT Registros
        /// </summary>
        /// <returns> Total de registros </returns>
        public int COUNT_PruebasAutoCompletado(string linea)
        {
            return ObjCapaDatos.COUNT_PruebasAutoCompletado(linea);
        }

        public List<string> PaginacionPRUEBA(string linea, int pagina, int siguientes)
        {
            return ObjCapaDatos.PaginacionPRUEBA(linea, pagina,  siguientes);
        }
        ////////////////
        /////PRUEBA PAGINADO TABLA
        /////////////////

        public int COUNT_Tabla()
        {
            return ObjCapaDatos.COUNT_Tabla();
        }
        public List<Linea> ListarPrueba(int pagina,string tipoOrden, int siguientes)
        {
            return ObjCapaDatos.ListarPrueba(pagina, tipoOrden,  siguientes);
        }

        public List<Linea> ListarPruebaWhere(int pagina, string tipoOrden, int siguientes, string where, string preguntaWhere) {
            return ObjCapaDatos.ListarPruebaWhere(pagina, tipoOrden, siguientes,  where,  preguntaWhere);

        }
        public int COUNT_TablaWhere(string where, string preguntaWhere)
        {
            return ObjCapaDatos.COUNT_TablaWhere(where, preguntaWhere);
        }

        ////////////////
        /////PRUEBA PAGINADO TABLA FIN
        /////////////////

    }
}

