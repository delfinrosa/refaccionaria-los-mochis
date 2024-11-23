using Microsoft.Extensions.Configuration;

namespace CapaDatos
{
    public class Conexion
    {
        public static string cn { get; private set; }

        public static void Initialize(IConfiguration configuration)
        {
            cn = configuration.GetConnectionString("Cadena");
        }
    }
}
