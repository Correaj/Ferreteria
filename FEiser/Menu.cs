using System;
using System.Windows.Forms;
using System.Drawing;

namespace FEiser
{
    public partial class Menu : Form
    {
        private Panel panelContenedor; // Panel donde se mostrarán los formularios

        public Menu()
        {
            InitializeComponent();
            CrearMenu();  // Crear el menú en la parte superior
            CrearPanelContenedor();  // Crear el panel debajo del menú
            this.WindowState = FormWindowState.Maximized; // Maximizar la ventana

        }

        private void Form2_Load(object sender, EventArgs e)
        {
        }

        private void CrearMenu()
        {
            // Crear un menú en la parte superior con fuente de 14 en negrita
            MenuStrip menuStrip = new MenuStrip
            {
                Font = new Font("Arial", 14, FontStyle.Bold)
            };

            // Crear el menú "Menú" con fuente de 14 en negrita
            ToolStripMenuItem menuArchivo = new ToolStripMenuItem("Menú")
            {
                Font = new Font("Arial", 14, FontStyle.Bold)
            };

            // Opción para abrir Form1 dentro del panel con fuente de 12 en negrita
            ToolStripMenuItem abrirForm1 = new ToolStripMenuItem("Facturación")
            {
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            abrirForm1.Click += (sender, e) => MostrarFormularioEnPanel(new Form1());

            // Opción para abrir AddProducto dentro del panel con fuente de 12 en negrita
            ToolStripMenuItem abrirAddProducto = new ToolStripMenuItem("Añadir Producto")
            {
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            abrirAddProducto.Click += (sender, e) => MostrarFormularioEnPanel(new AddProductos());

            // Opción para abrir AddProducto dentro del panel con fuente de 12 en negrita
            ToolStripMenuItem Historial = new ToolStripMenuItem("Historial")
            {
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            Historial.Click += (sender, e) => MostrarFormularioEnPanel(new Historial());

            // Agregar opciones al menú
            menuArchivo.DropDownItems.Add(abrirForm1);
            menuArchivo.DropDownItems.Add(abrirAddProducto); 
            menuArchivo.DropDownItems.Add(Historial);

            // Agregar menú a la barra de menú
            menuStrip.Items.Add(menuArchivo);

            // Configurar el menú en el formulario
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }


        private void CrearPanelContenedor()
        {
            // Crear y configurar el panel contenedor
            panelContenedor = new Panel
            {
                Dock = DockStyle.Fill, // Ocupa todo el espacio disponible debajo del menú
                BackColor = System.Drawing.Color.LightGray
            };

            this.Controls.Add(panelContenedor);
        }

        private void MostrarFormularioEnPanel(Form formulario)
        {
            // Limpiar el panel antes de agregar un nuevo formulario
            panelContenedor.Controls.Clear();

            // Configurar el formulario para que se adapte al panel
            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Dock = DockStyle.Fill;

            // Agregar el formulario al panel y mostrarlo
            panelContenedor.Controls.Add(formulario);
            formulario.Show();
        }
    }
}
