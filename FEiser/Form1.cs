using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FEiser
{
    public partial class Form1 : Form
    {
        private PrintDocument printDocument = new PrintDocument();
        private float startY;
        private int ultimaFactura = 0;
        // Variable para el temporizador
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public Form1()
        {
            InitializeComponent();
            string facturaGenerada = GenerarFactura();
            printDocument.PrintPage += PrintDocument_PrintPage;

            timer.Interval = 300;
            timer.Tick += Timer_Tick;
        }
        SqlConnection conexion = ConexionBD.ObtenerConexion();
        public string GenerarFactura()
        {
            string[] letrasMes = { "E", "F", "M", "A", "M", "JN", "JL", "A", "S", "O", "N", "D" };

            int mesActual = DateTime.Now.Month;
            int añoActual = DateTime.Now.Year;

            string ultimaFactura = UltimaFactura();
            int ultimoNumero = 0;

            if (!string.IsNullOrEmpty(ultimaFactura))
            {
                string numeroExtraido = ultimaFactura.Substring(4);
                int.TryParse(numeroExtraido, out ultimoNumero);
                ultimoNumero++;
            }
            else
            {
                ultimoNumero = 1;
            }
            string numeroFactura = ultimoNumero.ToString().PadLeft(5, '0');

            // Construir la factura en el formato: LetraMes + Año + Número
            string codigoFactura = $"{letrasMes[mesActual - 1]}{añoActual}{numeroFactura}";

            return codigoFactura;
        }
        public string UltimaFactura()
        {
            string ultimaFactura = "";

            try
            {
                conexion.Open();

                string query = "SELECT TOP 1 Factura FROM movimientos ORDER BY ID DESC";

                SqlCommand command = new SqlCommand(query, conexion);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    ultimaFactura = reader["factura"].ToString();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener la última factura: " + ex.Message);
            }
            finally
            {
                conexion.Close();
            }

            return ultimaFactura;
        }

        private void imprimir_Click(object sender, EventArgs e)
        {
            /*if (!string.IsNullOrEmpty(devuelta.Text))
            {
                try
                {
                    printDocument.Print();
                    RegistrarVenta();

                    totalPagar.Clear();
                    pagoCon.Clear();
                    devuelta.Clear();
                    numericUpDown1.Value = 1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al imprimir: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Introduce la cantidad pagada.");
            }*/

            if (!string.IsNullOrEmpty(devuelta.Text))
            {
                PrintPreviewDialog previewDialog = new PrintPreviewDialog
                {
                    Document = printDocument
                };

                DialogResult result = previewDialog.ShowDialog();

                if (result == DialogResult.OK || result == DialogResult.None)
                {
                    PrintDialog printDialog = new PrintDialog
                    {
                        Document = printDocument
                    };

                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        printDocument.Print();
                    }
                }
                RegistrarVenta();
                totalPagar.Clear();
                pagoCon.Clear();
                devuelta.Clear();
                numericUpDown1.Value = 1;
            }
            else
            {
                MessageBox.Show("Introduce la cantidad pagada.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string Precio = precioP.Text;
            string Descrip = descrip20.Text;
            string cant = numericUpDown1.Text;

            if (!string.IsNullOrWhiteSpace(Precio) &&
                !string.IsNullOrWhiteSpace(Descrip) &&
                !string.IsNullOrWhiteSpace(Descrip))
            {
                decimal precio = 0;
                bool existe_Producto = false;
                bool existe_Descripcion = false;



                if (!decimal.TryParse(precioP.Text, out precio))
                {
                    MessageBox.Show("Precio - Formato moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                if (!existe_Producto && !existe_Descripcion)
                {
                    dataGridView1.Rows.Add(new object[]
                    {
                        numericUpDown1.Value.ToString(),
                        descrip.Text,
                        precio.ToString("N0"),
                        (numericUpDown1.Value * precio).ToString("N2")

                    });
                }

                precioTotal();
                limpiar();
            }
            else
            {
                MessageBox.Show("Ingresa un producto con su precio.");
            }
        }
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Generar la factura
            string facturaGenerada = GenerarFactura();

            float yPos = 0; // Comenzamos desde el principio
            float leftMargin = 0; // Usamos márgenes pequeños
            float rightMargin = 0; // Márgenes pequeños para aprovechar todo el ancho
            float lineHeight = 12; // Espacio entre líneas ajustado para texto más pequeño
            Font font = new Font("Arial", 7); // Fuente ajustada un punto más grande
            float pageWidth = 320; // Ancho de la impresora térmica (80mm en píxeles aprox. a 200 ppp)

            float borderThickness = 1f;
            float cornerRadius = 5f;
            float padding = 5f;
            float boxWidth = pageWidth - (2 * leftMargin);
            float boxHeight = yPos + (lineHeight * 4.8f) - yPos;
            float boxX = leftMargin;
            float boxY = yPos + lineHeight;

            // Fuente para el título
            Font titleFont = new Font("Arial", 8, FontStyle.Bold);
            string title = "Ferretería José Muñoz";
            float titleWidth = e.Graphics.MeasureString(title, titleFont).Width;
            float xPos = (pageWidth - titleWidth) / 2;
            lineHeight = titleFont.GetHeight(e.Graphics);
            e.Graphics.DrawString(title, titleFont, Brushes.Black, xPos, yPos);
            yPos += lineHeight * 1.2f;

            Font otherTitleFont = new Font("Arial", 6, FontStyle.Bold);
            string otherTitle = "ARTICULOS FERRETEROS EN GENERAL";
            float otherTitleWidth = e.Graphics.MeasureString(otherTitle, otherTitleFont).Width;
            xPos = (pageWidth - otherTitleWidth) / 2;
            e.Graphics.DrawString(otherTitle, otherTitleFont, Brushes.Black, xPos, yPos);
            yPos += lineHeight * 1.2f;

            Font addressFont = new Font("Arial", 6, FontStyle.Regular);

            // Dividimos la dirección en dos líneas
            string addressLine1 = "Residencial La Ortaliza, #124, Carretera La Victoria,";
            string addressLine2 = "Villa Mella, Santo Domingo Norte, R.D";

            float addressLine1Width = e.Graphics.MeasureString(addressLine1, addressFont).Width;
            xPos = (pageWidth - addressLine1Width) / 2;
            e.Graphics.DrawString(addressLine1, addressFont, Brushes.Black, xPos, yPos);
            yPos += lineHeight;

            float addressLine2Width = e.Graphics.MeasureString(addressLine2, addressFont).Width;
            xPos = (pageWidth - addressLine2Width) / 2;
            e.Graphics.DrawString(addressLine2, addressFont, Brushes.Black, xPos, yPos);
            yPos += lineHeight * 1.2f;

            string phone = "Tel: 829-594-0228 / 829-465-0651";
            float phoneWidth = e.Graphics.MeasureString(phone, addressFont).Width;
            xPos = (pageWidth - phoneWidth) / 2;
            e.Graphics.DrawString(phone, addressFont, Brushes.Black, xPos, yPos);
            yPos += lineHeight * 1.5f;

            // Factura a la izquierda y fecha a la derecha
            Font infoFont = new Font("Arial", 6, FontStyle.Bold);
            string invoiceText = "Factura: " + facturaGenerada;
            string dateText = dateTimePicker1.Value.ToString("dddd, dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES"));

            float invoiceX = (leftMargin + padding) * 1.05f; // 5% más centrado
            float dateX = (pageWidth - e.Graphics.MeasureString(dateText, infoFont).Width - rightMargin - padding) * 0.80f;
            float textY = yPos;

            e.Graphics.DrawString(invoiceText, infoFont, Brushes.Black, invoiceX, textY);
            e.Graphics.DrawString(dateText, infoFont, Brushes.Black, dateX, textY);

            yPos += lineHeight * 1.5f;

            float adjustedLeftMargin = leftMargin + (pageWidth * 0.1f); // Márgenes ajustados
            Font dataFont = new Font("Arial", 6, FontStyle.Regular);
            float labelWidth = pageWidth * 0.3f;
            float valueStartX = adjustedLeftMargin + labelWidth;

            string clientText = "Cliente:";
            e.Graphics.DrawString(clientText, dataFont, Brushes.Black, adjustedLeftMargin, yPos);
            e.Graphics.DrawString(txtCliente.Text, dataFont, Brushes.Black, valueStartX, yPos);
            yPos += lineHeight;

            string addressText = "Dirección:";
            e.Graphics.DrawString(addressText, dataFont, Brushes.Black, adjustedLeftMargin, yPos);
            e.Graphics.DrawString(txtDireccion.Text, dataFont, Brushes.Black, valueStartX, yPos);
            yPos += lineHeight;

            string phoneText = "Teléfono:";
            e.Graphics.DrawString(phoneText, dataFont, Brushes.Black, adjustedLeftMargin, yPos);
            e.Graphics.DrawString(txtTelefono.Text, dataFont, Brushes.Black, valueStartX, yPos);
            yPos += lineHeight * 1.5f;

            // Separador de asteriscos
            string separator0 = new string('*', 80); // Menos caracteres para ajustarse mejor
            Font separatorFont0 = new Font("Arial", 6);
            float separatorWidth0 = e.Graphics.MeasureString(separator0, separatorFont0).Width;
            float separatorX0 = (pageWidth - separatorWidth0) / 2 + leftMargin;
            e.Graphics.DrawString(separator0, separatorFont0, Brushes.Black, separatorX0, yPos);
            yPos += separatorFont0.GetHeight(e.Graphics) * 1.5f;

            // Datos de la tabla de productos
            Font columnFont = new Font("Arial", 6, FontStyle.Bold); // Tamaño mayor y negrita para columnas
            float columnStartX = adjustedLeftMargin;
            float columnWidth = (pageWidth - adjustedLeftMargin - rightMargin) / dataGridView1.Columns.Count;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                e.Graphics.DrawString(column.HeaderText, columnFont, Brushes.Black, columnStartX, yPos);
                columnStartX += columnWidth;
            }
            yPos += lineHeight;

            float totalPrecioUnitario = 0f;
            float totalValorFinal = 0f;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    float currentX = adjustedLeftMargin;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        string cellValue = cell.Value?.ToString() ?? "";
                        e.Graphics.DrawString(cellValue, font, Brushes.Black, currentX, yPos);

                        if (cell.OwningColumn.Name == "precioU")
                        {
                            if (float.TryParse(cellValue, out float precioUnitario))
                            {
                                totalPrecioUnitario += precioUnitario;
                            }
                        }
                        else if (cell.OwningColumn.Name == "valor")
                        {
                            if (float.TryParse(cellValue, out float valorFinal))
                            {
                                totalValorFinal += valorFinal;
                            }
                        }

                        currentX += columnWidth;
                    }
                    yPos += lineHeight;
                }
            }

            // Totales
            yPos += lineHeight * 1.5f;

            Font totalFont = new Font("Arial", 6, FontStyle.Bold);
            string totalText = "Totales:";
            e.Graphics.DrawString(totalText, totalFont, Brushes.Black, adjustedLeftMargin, yPos);

            float totalValuesX = adjustedLeftMargin + columnWidth * (dataGridView1.Columns["precioU"].Index);
            e.Graphics.DrawString(totalPrecioUnitario.ToString("N2"), totalFont, Brushes.Black, totalValuesX, yPos);

            totalValuesX = adjustedLeftMargin + columnWidth * (dataGridView1.Columns["valor"].Index);
            e.Graphics.DrawString(totalValorFinal.ToString("N2"), totalFont, Brushes.Black, totalValuesX, yPos);

            yPos += lineHeight * 1.5f;

            // Pago y cambio
            string capitalText = "Pago con:";
            e.Graphics.DrawString(capitalText, totalFont, Brushes.Black, adjustedLeftMargin, yPos);
            e.Graphics.DrawString(pagoCon.Text, totalFont, Brushes.Black, totalValuesX, yPos);
            yPos += lineHeight;

            string cambioText = "Cambio:";
            e.Graphics.DrawString(cambioText, totalFont, Brushes.Black, adjustedLeftMargin, yPos);
            e.Graphics.DrawString(devuelta.Text, totalFont, Brushes.Black, totalValuesX, yPos);
            yPos += lineHeight * 1.5f;

            // Separador de asteriscos
            string separator = new string('*', 80);
            Font separatorFont = new Font("Arial", 6);
            float separatorWidth = e.Graphics.MeasureString(separator, separatorFont).Width;
            float separatorX = (pageWidth - separatorWidth) / 2 + leftMargin;
            e.Graphics.DrawString(separator, separatorFont, Brushes.Black, separatorX, yPos);
            yPos += separatorFont.GetHeight(e.Graphics) * 2.0f;

            // Enviado por y Recibido por
            string enviadoPorText = "Enviado por:__________________________________";
            e.Graphics.DrawString(enviadoPorText, dataFont, Brushes.Black, adjustedLeftMargin, yPos);
            yPos += lineHeight * 3.0f;

            string recibidoPorText = "Recibido por:_________________________________";
            e.Graphics.DrawString(recibidoPorText, dataFont, Brushes.Black, adjustedLeftMargin, yPos);
            //yPos += lineHeight * 1.5f;


        }
        private void RegistrarVenta()
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                SqlTransaction transaccion = null;

                try
                {
                    conexion.Open();
                    transaccion = conexion.BeginTransaction();

                    string numeroFactura = GenerarFactura();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            // Leer datos del DataGridView
                            int cantidadVendida = Convert.ToInt32(row.Cells["cantidad"].Value);
                            string nombreProducto = row.Cells["name"].Value?.ToString() ?? "";
                            decimal precioUnitario = Convert.ToDecimal(row.Cells["precioU"].Value);

                            // Obtener ID del producto según el nombre
                            int idProducto = ObtenerIdProducto(nombreProducto, conexion, transaccion);
                            if (idProducto == 0)
                            {
                                MessageBox.Show($"Producto '{nombreProducto}' no encontrado en la base de datos.");
                                continue;
                            }

                            // Registrar movimiento de salida
                            string insertMovimiento = @"INSERT INTO movimientos 
                                                 (ID_Producto, Fecha, Cantidad, Tipo, Motivo, Factura) 
                                                 VALUES (@ID_Producto, GETDATE(), @Cantidad, @Tipo, @Motivo, @Factura);";

                            using (SqlCommand cmdInsert = new SqlCommand(insertMovimiento, conexion, transaccion))
                            {
                                cmdInsert.Parameters.AddWithValue("@ID_Producto", idProducto);
                                cmdInsert.Parameters.AddWithValue("@Cantidad", cantidadVendida);
                                cmdInsert.Parameters.AddWithValue("@Tipo", "salida");
                                cmdInsert.Parameters.AddWithValue("@Motivo", "Venta de producto");
                                cmdInsert.Parameters.AddWithValue("@Factura", numeroFactura);
                                cmdInsert.ExecuteNonQuery();
                            }

                            // Actualizar stock
                            string updateStock = @"UPDATE productos 
                                            SET CantidadEnStock = CantidadEnStock - @Cantidad 
                                            WHERE ID = @ID_Producto AND CantidadEnStock >= @Cantidad;";

                            using (SqlCommand cmdUpdate = new SqlCommand(updateStock, conexion, transaccion))
                            {
                                cmdUpdate.Parameters.AddWithValue("@ID_Producto", idProducto);
                                cmdUpdate.Parameters.AddWithValue("@Cantidad", cantidadVendida);

                                int filasAfectadas = cmdUpdate.ExecuteNonQuery();
                                if (filasAfectadas == 0)
                                {
                                    throw new Exception($"Stock insuficiente para el producto '{nombreProducto}'.");
                                }
                            }
                        }
                    }
                    // Verificar stock bajo después de actualizar el stock
                    VerificarStockBajo(conexion, transaccion);

                    transaccion.Commit();
                    MessageBox.Show("Venta registrada y stock actualizado correctamente.");
                }
                catch (Exception ex)
                {
                    transaccion?.Rollback();
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
        private void VerificarStockBajo(SqlConnection conexion, SqlTransaction transaccion)
        {
            string consultaStockBajo = @"SELECT Nombre, CantidadEnStock 
                                 FROM productos 
                                 WHERE CantidadEnStock <= 40";

            using (SqlCommand cmdStockBajo = new SqlCommand(consultaStockBajo, conexion, transaccion))
            {
                using (SqlDataReader reader = cmdStockBajo.ExecuteReader())
                {
                    StringBuilder mensaje = new StringBuilder();
                    while (reader.Read())
                    {
                        string nombreProducto = reader["Nombre"].ToString();
                        int cantidadEnStock = Convert.ToInt32(reader["CantidadEnStock"]);
                        mensaje.AppendLine($"{nombreProducto}: {cantidadEnStock} unidades en stock");
                    }

                    if (mensaje.Length > 0)
                    {
                        MessageBox.Show($"¡Alerta de stock bajo!\n{mensaje.ToString()}", "Alerta de Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private int ObtenerIdProducto(string nombreProducto, SqlConnection conexion, SqlTransaction transaccion)
        {
            string query = "SELECT ID FROM productos WHERE Nombre = @Nombre";
            using (SqlCommand cmd = new SqlCommand(query, conexion, transaccion))
            {
                cmd.Parameters.AddWithValue("@Nombre", nombreProducto);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
        private void cambioo()
        {
            if (totalPagar.Text.Trim() == "")
            {
                MessageBox.Show("No hay productos para la venta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            decimal pagacon;
            decimal total = Convert.ToDecimal(totalPagar.Text);

            if (pagoCon.Text.Trim() == "")
            {
                devuelta.Text = "N2";
                return;
            }

            if (decimal.TryParse(pagoCon.Text.Trim(), out pagacon))
            {
                if (pagacon < total)
                {
                    MessageBox.Show("Esta cantidad es menol al total");
                    devuelta.Text = "0.00";
                }
                else
                {
                    decimal cambio = pagacon - total;
                    devuelta.Text = cambio.ToString("N2");
                }
            }
            else
            {
                devuelta.Text = "0.00";
            }

            // Verificar que el contenido sea numérico antes de aplicar el formato
            if (decimal.TryParse(pagoCon.Text, out decimal number))
            {
                pagoCon.Text = number.ToString("N2"); // Formato con dos decimales
            }
            else if (!string.IsNullOrWhiteSpace(pagoCon.Text))
            {
                MessageBox.Show("Ingrese un número válido.");
                pagoCon.Text = string.Empty;
            }
        }
        public void precioTotal()
        {
            decimal total = 0;
            if (dataGridView1.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                    total += Convert.ToDecimal(row.Cells["valor"].Value.ToString());
            }
            totalPagar.Text = total.ToString("N2");
        }

        private void totalPagar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (precioP.Text.Trim().Length > 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void pagoCon_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                cambioo();
            }
        }

        private void pagoCon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (pagoCon.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    if (char.IsControl(e.KeyChar) && e.KeyChar.ToString() == ".")
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (index >= 0)
            {
                dataGridView1.Rows.RemoveAt(index);
                precioTotal();
            }
        }

        private void Buscar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {

                conexion.Open();

                string consulta = "select Nombre, Precio from productos where Codigo=" + textBox1.Text + "";

                // Crea un comando SQL para ejecutar la consulta
                SqlCommand comando = new SqlCommand(consulta, conexion);


                SqlDataReader lector = comando.ExecuteReader();


                if (lector.Read())
                {
                    string nombreProducto = lector["Nombre"].ToString();
                    string precio = lector["Precio"].ToString();

                    string product = nombreProducto;
                    string prec = precio;

                    descrip.Text = product;
                    precioP.Text = precio;

                }
                else
                {

                    MessageBox.Show("No se encontro un producto con este codigo.");
                }


                lector.Close();
                conexion.Close();
            }
            else
            {
                MessageBox.Show("Favor introducir un codigo valido.");
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                {

                    conexion.Open();

                    string consulta = "select Nombre, Precio from productos where Codigo=" + textBox1.Text + "";

                    // Crea un comando SQL para ejecutar la consulta
                    SqlCommand comando = new SqlCommand(consulta, conexion);


                    SqlDataReader lector = comando.ExecuteReader();


                    if (lector.Read())
                    {
                        string nombreProducto = lector["Nombre"].ToString();
                        string precio = lector["Precio"].ToString();

                        string product = nombreProducto;
                        string prec = precio;

                        descrip.Text = product;
                        precioP.Text = precio;

                    }
                    else
                    {

                        MessageBox.Show("No se encontro un producto con este codigo.");
                    }


                    lector.Close();
                    conexion.Close();
                }
                else
                {
                    MessageBox.Show("Favor introducir un codigo valido.");
                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (textBox1.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    if (char.IsControl(e.KeyChar) && e.KeyChar.ToString() == ".")
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
        }
        public void limpiar()
        {
            textBox1.Text = "";
            descrip.Text = "";
            precioP.Text = "";
            numericUpDown1.Value = 1;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Detiene el Timer para evitar múltiples ejecuciones
            timer.Stop();

            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                try
                {
                    conexion.Open();
                    string consulta = "SELECT Nombre, Precio FROM productos WHERE Codigo = @Codigo";

                    SqlCommand comando = new SqlCommand(consulta, conexion);
                    comando.Parameters.AddWithValue("@Codigo", textBox2.Text);

                    SqlDataReader lector = comando.ExecuteReader();

                    if (lector.Read())
                    {
                        descrip.Text = lector["Nombre"].ToString();
                        precioP.Text = lector["Precio"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("No se encontró un producto con este código.");
                    }

                    lector.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    conexion.Close();
                }
            }
            else
            {
                MessageBox.Show("Por favor, introduzca un código válido.");
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Start();
        }
    }
}
