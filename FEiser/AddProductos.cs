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
using Microsoft.VisualBasic;


namespace FEiser
{
    public partial class AddProductos : Form
    {
        public AddProductos()
        {
            InitializeComponent();
        }
        SqlConnection conexion = ConexionBD.ObtenerConexion();
        public void Actualizar()
        {
            string consulta = "select * from productos;";

            SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);

            DataTable dt = new DataTable();
            adaptador.Fill(dt);
            dataGridView2.DataSource = dt;
        }

        private void AddProductos_Load(object sender, EventArgs e)
        {
            Actualizar();
        }

        private void Activar_Click(object sender, EventArgs e)
        {
            string connectionString = "Data source=ESTACION1\\MSSQLSERVERCIOR;Initial Catalog=Factura;User id=sa; password=abcd@1234";

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
                            label4.Visible = true;
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

        private void ADD_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) ||
                string.IsNullOrEmpty(numericUpDown1.Value.ToString()) ||
                string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            conexion.Open();

            // Validar si el producto ya existe según el Código
            string consultaValidacion = "SELECT COUNT(*) FROM productos WHERE Codigo = @Codigo";
            SqlCommand comandoValidacion = new SqlCommand(consultaValidacion, conexion);
            comandoValidacion.Parameters.AddWithValue("@Codigo", Code.Text);

            int existeProducto = Convert.ToInt32(comandoValidacion.ExecuteScalar());

            if (existeProducto > 0)
            {
                MessageBox.Show("El producto con el código especificado ya existe.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                conexion.Close();
                return;
            }

            string consulta = "INSERT INTO productos VALUES (@Valor1, @Valor2, @Valor3, @Valor4, @Valor5)";
            SqlCommand comando = new SqlCommand(consulta, conexion);
            comando.Parameters.AddWithValue("@Valor1", textBox1.Text);
            comando.Parameters.AddWithValue("@Valor2", Code.Text);
            comando.Parameters.AddWithValue("@Valor3", numericUpDown1.Value.ToString());
            comando.Parameters.AddWithValue("@Valor4", textBox2.Text);
            comando.Parameters.AddWithValue("@Valor5", id.Text);

            comando.ExecuteNonQuery();

            conexion.Close();

            textBox1.Text = "";
            textBox2.Text = "";
            id.Text = "";

            MessageBox.Show("Producto agregado correctamente.");
            Actualizar();
        }

        private void update_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(id.Text) ||
                string.IsNullOrEmpty(textBox1.Text) ||
                string.IsNullOrEmpty(Code.Text) ||
                string.IsNullOrEmpty(numericUpDown1.Value.ToString()) ||
                string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            int idProducto;
            if (!int.TryParse(id.Text, out idProducto))
            {
                MessageBox.Show("El ID del producto debe ser un número válido.");
                return;
            }

            try
            {
                conexion.Open();
                string consulta = @"UPDATE productos 
                            SET Nombre = @Nombre, 
                                Codigo = @Codigo, 
                                CantidadEnStock = @CantidadEnStock, 
                                Precio = @Precio, 
                                Categoria = @Categoria 
                            WHERE ID = @ID";

                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@ID", idProducto);
                comando.Parameters.AddWithValue("@Nombre", textBox1.Text);
                comando.Parameters.AddWithValue("@Codigo", Code.Text);
                comando.Parameters.AddWithValue("@CantidadEnStock", numericUpDown1.Value);
                comando.Parameters.AddWithValue("@Precio", Convert.ToDecimal(textBox2.Text));
                comando.Parameters.AddWithValue("@Categoria", id.Text);

                int filasActualizadas = comando.ExecuteNonQuery();

                if (filasActualizadas > 0)
                {
                    MessageBox.Show("Producto actualizado correctamente.");
                }
                else
                {
                    MessageBox.Show("No se encontró un producto con el ID especificado.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar el producto: {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }

            Actualizar();
        }

        private void delete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Code.Text))
            {
                MessageBox.Show("Por favor, introduce el código del producto a eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idd;
            if (!int.TryParse(id.Text, out idd))
            {
                MessageBox.Show("El ID debe ser un número válido.");
                return;
            }

            conexion.Open();

            // Eliminar movimientos relacionados
            string eliminarMovimientos = "DELETE FROM movimientos WHERE ID_Producto = @IDProducto";
            SqlCommand cmdEliminarMovimientos = new SqlCommand(eliminarMovimientos, conexion);
            cmdEliminarMovimientos.Parameters.AddWithValue("@IDProducto", idd);
            cmdEliminarMovimientos.ExecuteNonQuery();

            // Eliminar producto
            string eliminarProducto = "DELETE FROM productos WHERE ID = @IDProducto";
            SqlCommand cmdEliminarProducto = new SqlCommand(eliminarProducto, conexion);
            cmdEliminarProducto.Parameters.AddWithValue("@IDProducto", idd);
            cmdEliminarProducto.ExecuteNonQuery();

            conexion.Close();

            MessageBox.Show("Producto y movimientos relacionados eliminados correctamente.");
            Actualizar();

            textBox1.Clear();
            textBox2.Clear();
            id.Clear();
            id.Clear();
            Code.Clear();
            numericUpDown1.Value = 1;
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (id.Visible)
            {
                id.Text = dataGridView2.SelectedCells[0].Value.ToString();
                textBox1.Text = dataGridView2.SelectedCells[1].Value.ToString();
                Code.Text = dataGridView2.SelectedCells[2].Value.ToString();
                numericUpDown1.Text = dataGridView2.SelectedCells[3].Value.ToString();
                textBox2.Text = dataGridView2.SelectedCells[4].Value.ToString();
            }
        }
    }
}
