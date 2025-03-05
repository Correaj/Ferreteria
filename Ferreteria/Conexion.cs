using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferreteria
{
    internal class Conexion
    {
    }
    public static class ConexionBD
    {
        private static string connectionString = "server=LAPTOP-LGA37B6V; database=Factura;integrated security=true";

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(connectionString);
        }
    }
}
