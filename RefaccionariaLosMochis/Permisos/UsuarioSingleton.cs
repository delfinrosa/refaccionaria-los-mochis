using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RefaccionariaLosMochis.Permisos
{
    public sealed class UsuarioSingleton
    {
        private static readonly Lazy<UsuarioSingleton> instance = new Lazy<UsuarioSingleton>(() => new UsuarioSingleton());
        public Usuario Usuario { get; set; }

        private UsuarioSingleton() { }

        public static UsuarioSingleton Instance
        {
            get { return instance.Value; }
        }
    }

}