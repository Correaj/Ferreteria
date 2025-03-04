using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEiser
{
    internal class Conexion
    {
    }
    public static class ConexionBD
    {
        private static string connectionString = "Data source=ESTACION1\\MSSQLSERVERCIOR;Initial Catalog=Factura;User id=sa; password=abcd@1234";

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(connectionString);
        }
    }
    //Data source=ESTACION1\\MSSQLSERVERCIOR;Initial Catalog=Factura;User id=sa; password=abcd@1234
}
