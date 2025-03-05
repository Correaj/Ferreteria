using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ferreteria
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection conexion = ConexionBD.ObtenerConexion();
        private void Activar_Click(object sender, EventArgs e)
        {
            string connectionString = "server=LAPTOP-LGA37B6V; database=Factura;integrated security=true";

            string inputPassword = Microsoft.VisualBasic.Interaction.InputBox(
                "Por favor, introduce la contraseña de administrador:",
                "Autenticación"
            );

            if (string.IsNullOrWhiteSpace(inputPassword))
            {
                MessageBox.Show("Debes ingresar una contraseña.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM pass WHERE contraseña COLLATE SQL_Latin1_General_CP1_CS_AS = @password";


                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@password", SqlDbType.NVarChar).Value = inputPassword;

                        int count = Convert.ToInt32(command.ExecuteScalar());

                        if (count > 0)
                        {
                            id.Visible = true;
                            label6.Visible = true;
                            update.Visible = true;
                            delete.Visible = true;
                            ADD.Visible = true;
                            MessageBox.Show("Acceso concedido, elementos visibles.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Contraseña incorrecta, acceso denegado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error en la conexión: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

namespace Microsoft.VisualBasic
{
    class Interaction
    {
        internal static string InputBox(string v1, string v2)
        {
            throw new NotImplementedException();
        }
    }
}