using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Globalization;

namespace DeclineR.Pantalla_Principal
{
    public partial class CargadorDeArchivos : Form
    {
        protected string direccionArchivo, nombreArchivo, extencionArchivo;
        protected int posicionUltimoContraSlash;
        protected int columnaFechas, columnaDatos;

        protected List<DateTime> listaFechas = new List<DateTime>();
        protected List<double> listaDatos = new List<double>();
        protected string separadorDecimal = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
        protected string separadorDeMiles;
        protected char separadorColumnas;
        protected string formatoFecha;
        protected int celdaInicioDatos = 0;
        protected bool titulos = false;

        CargadorDeDatosDelDataGrid cargadorDeDatosDelDataGrid = new CargadorDeDatosDelDataGrid();

        public EventHandler<DatosCargadosEventArgs> datosCargados;


        protected void comboBoxSeparadorDecimal_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxSeparadorDecimal.SelectedIndex)
            {
                case 0:
                    separadorDecimal = ",";
                    break;
                case 1:
                    separadorDecimal = ".";
                    break;
                case 2:
                    separadorDecimal = "";
                    break;
                default:
                    break;
            }

            if (Convert.ToString(separadorColumnas) == separadorDecimal && separadorDecimal != "")
            {
                MessageBox.Show("Los separadores no pueden ser iguales", "Error separadores", MessageBoxButtons.OK, MessageBoxIcon.Error);
                separadorDecimal = "";
                comboBoxSeparadorDecimal.SelectedIndex = 2;
            }

            if (separadorDeMiles == separadorDecimal && separadorDecimal != "")
            {
                MessageBox.Show("Los separadores no pueden ser iguales", "Error separadores", MessageBoxButtons.OK, MessageBoxIcon.Error);
                separadorDeMiles = "";
                comboBoxSeparadorDeMiles.SelectedIndex = 3;
            }
            buttonAceptar.Enabled = false;
        }

        protected void comboBoxSeparadorDeMiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxSeparadorDeMiles.SelectedIndex)
            {
                case 0:
                    separadorDeMiles = ",";
                    break;
                case 1:
                    separadorDeMiles = " ";
                    break;
                case 2:
                    separadorDeMiles = ".";
                    break;
                default:
                    separadorDeMiles = "";
                    break;
            }

            if (separadorDeMiles == separadorDecimal && separadorDecimal != "")
            {
                MessageBox.Show("Los separadores no pueden ser iguales", "Error separadores", MessageBoxButtons.OK, MessageBoxIcon.Error);
                separadorDeMiles = "";
                comboBoxSeparadorDeMiles.SelectedIndex = 3;
            }

            if (Convert.ToString(separadorColumnas) == separadorDeMiles && separadorDeMiles != "")
            {
                MessageBox.Show("Los separadores no pueden ser iguales", "Error separadores", MessageBoxButtons.OK, MessageBoxIcon.Error);
                separadorDeMiles = "";
                comboBoxSeparadorDeMiles.SelectedIndex = 3;
            }
            buttonAceptar.Enabled = false;
        }

        protected void comboBoxFormatoDeFecha_SelectedIndexChanged(object sender, EventArgs e)
        {
            formatoFecha = comboBoxFormatoDeFecha.SelectedItem.ToString();
            buttonAceptar.Enabled = false;
        }

        protected void buttonAñadirRenglon_Click(object sender, EventArgs e)
        {
            if (dataGridViewCargadorDeDatos.RowCount > 0)
            {
                int renglon = dataGridViewCargadorDeDatos.CurrentCell.RowIndex;
                dataGridViewCargadorDeDatos.Rows.Insert(renglon + 1);
            }
            else
            {
                dataGridViewCargadorDeDatos.RowCount = 1;
            }
            buttonAceptar.Enabled = false;
        }

        protected void buttonEliminarRenglon_Click(object sender, EventArgs e)
        {
            if (dataGridViewCargadorDeDatos.RowCount > 1)
            {
                int renglon = dataGridViewCargadorDeDatos.CurrentCell.RowIndex;
                dataGridViewCargadorDeDatos.Rows.RemoveAt(renglon);
            }
            buttonAceptar.Enabled = false;
        }

        protected void buttonCargar_Click(object sender, EventArgs e) //Guarda el texto de las columnas seleccionadas en listas de fecha y datos segun corresponda;
        {
            listaDatos.Clear();
            listaFechas.Clear();

            if(comboBoxColumnaFechas.SelectedIndex != -1 && comboBoxColumnaDatos.SelectedIndex != -1)
            {
                cargadorDeDatosDelDataGrid.Proceso(dataGridViewCargadorDeDatos, columnaFechas, columnaDatos, separadorDecimal, formatoFecha, separadorDeMiles, listaFechas, listaDatos);
                buttonAceptar.Enabled = true;
            }
            else
            {
                MessageBox.Show("Debe seleccionar las columnas correspondientes a fecha y datos", "Error al cargar datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        protected void comboBoxSeparadorColumnas_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxSeparadorColumnas.SelectedIndex)
            {
                case 0:
                    separadorColumnas = '\t';
                    textBoxOtroSeparadorColumnas.Visible = false;
                    labelOtroSeparadorColumnas.Visible = false;
                    break;
                case 1:
                    separadorColumnas = ' ';
                    textBoxOtroSeparadorColumnas.Visible = false;
                    labelOtroSeparadorColumnas.Visible = false;
                    break;
                case 2:
                    separadorColumnas = ',';
                    textBoxOtroSeparadorColumnas.Visible = false;
                    labelOtroSeparadorColumnas.Visible = false;
                    break;
                case 3:
                    separadorColumnas = ';';
                    textBoxOtroSeparadorColumnas.Visible = false;
                    labelOtroSeparadorColumnas.Visible = false;
                    break;
                case 4:
                    textBoxOtroSeparadorColumnas.Visible = true;
                    labelOtroSeparadorColumnas.Visible = true;
                    textBoxOtroSeparadorColumnas.Text = "";
                    separadorColumnas = ' ';
                    break;
            }

            if (Convert.ToString(separadorColumnas) == separadorDecimal && separadorDecimal != "")
            {
                MessageBox.Show("Los separadores no pueden ser iguales", "Error separadores", MessageBoxButtons.OK, MessageBoxIcon.Error);
                separadorDecimal = "";
                comboBoxSeparadorDecimal.SelectedIndex = 2;
            }

            if (Convert.ToString(separadorColumnas) == separadorDeMiles && separadorDeMiles != "")
            {
                MessageBox.Show("Los separadores no pueden ser iguales", "Error separadores", MessageBoxButtons.OK, MessageBoxIcon.Error);
                separadorDeMiles = "";
                comboBoxSeparadorDeMiles.SelectedIndex = 3;
            }

            buttonAceptar.Enabled = false;
        }

        protected void textBoxOtroSeparadorColumnas_TextChanged(object sender, EventArgs e)
        {
            
            if(textBoxOtroSeparadorColumnas.Text.Length > 1)
            {
                MessageBox.Show("El separador de columnas debe ser de solo un caractér", "Error separador", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxOtroSeparadorColumnas.Text = textBoxOtroSeparadorColumnas.Text[0].ToString();
                separadorColumnas = Convert.ToChar(textBoxOtroSeparadorColumnas.Text);
            }
            else
            {
                separadorColumnas = Convert.ToChar(textBoxOtroSeparadorColumnas.Text);
            }
            buttonAceptar.Enabled = false;

        }

        protected void checkBoxTitulos_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxTitulos.Checked)
            {
                celdaInicioDatos = 1;
                titulos = true;
            }
            else
            {
                celdaInicioDatos = 0;
                titulos = false;
            }
            buttonAceptar.Enabled = false;
        }

        protected void buttonBorrarDatos_Click(object sender, EventArgs e)
        {
            dataGridViewCargadorDeDatos.RowCount = 0;
            dataGridViewCargadorDeDatos.RowCount = 1;
            listaDatos.Clear();
            listaFechas.Clear();
            buttonAceptar.Enabled = false;
        }

        public CargadorDeArchivos()
        {
            InitializeComponent();
        }

        protected void buttonAceptar_Click(object sender, EventArgs e) //Crea el evento para pasar los datos como argumentos al formulario que llamo al cargador de archivos.
        {
            EventHandler<DatosCargadosEventArgs> manejadorDeEvento = datosCargados;
            manejadorDeEvento(this, new DatosCargadosEventArgs() { listaDatos = listaDatos, listaFechas = listaFechas, longitudCadenaDeFecha = formatoFecha.Length});
            this.Close();
        }

        protected void CargadorDeArchivos_Load(object sender, EventArgs e)
        {
            dataGridViewCargadorDeDatos.RowCount = 1;
            dataGridViewCargadorDeDatos.ColumnCount = 1;
            dataGridViewCargadorDeDatos.Columns[0].Name = "1";

            if (separadorDecimal == ",")
            {
                comboBoxSeparadorDecimal.SelectedIndex = 0;
            }
            else
            {
                comboBoxSeparadorDecimal.SelectedIndex = 1;
            }

            comboBoxFormatoDeFecha.SelectedIndex = 0;
            comboBoxSeparadorDeMiles.SelectedIndex = 3;
            comboBoxSeparadorColumnas.SelectedIndex = 0;
            separadorColumnas = '\t';
        }

        protected void comboBoxColumnaFechas_SelectedIndexChanged(object sender, EventArgs e)
        {
            columnaFechas = comboBoxColumnaFechas.SelectedIndex;

            if (comboBoxColumnaFechas.SelectedIndex == comboBoxColumnaDatos.SelectedIndex)
            {
                MessageBox.Show("No puede seleccionar la misma columna para fechas y para datos", "Error de seleccion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxColumnaFechas.SelectedItem = false;
                comboBoxColumnaFechas.SelectedIndex = -1;
                comboBoxColumnaFechas.Text = "";
            }
            buttonAceptar.Enabled = false;
        }

        protected void comboBoxColumnaDatos_SelectedIndexChanged(object sender, EventArgs e)
        {
            columnaDatos = comboBoxColumnaDatos.SelectedIndex;

            if (comboBoxColumnaFechas.SelectedIndex == comboBoxColumnaDatos.SelectedIndex)
            {
                MessageBox.Show("No puede seleccionar la misma columna para fechas y para datos", "Error de seleccion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxColumnaDatos.SelectedItem = false;
                comboBoxColumnaDatos.SelectedIndex = -1;
                comboBoxColumnaDatos.Text = "";
            }

            buttonAceptar.Enabled = false;
        }

        protected void buttonVisualizar_Click(object sender, EventArgs e)
        {
            comboBoxColumnaDatos.Text = "";
            comboBoxColumnaDatos.Items.Clear();
            comboBoxColumnaDatos.SelectedItem = false;
            comboBoxColumnaDatos.SelectedIndex = -1;

            comboBoxColumnaFechas.Text = "";
            comboBoxColumnaFechas.Items.Clear();
            comboBoxColumnaFechas.SelectedItem = false;
            comboBoxColumnaFechas.SelectedIndex = -1;

            int indiceSeparadorColumnas = comboBoxSeparadorColumnas.SelectedIndex;
            string otroSeparadorColumnas = textBoxOtroSeparadorColumnas.Text;
            bool titulosExistentes = checkBoxTitulos.Checked;

            if (indiceSeparadorColumnas != 4 || otroSeparadorColumnas.Length != 0) //Si hay un separador de columnas especificado, carga el archivo.
            {
                DesplegadorDeArchivoEnDataGridView desplegadorDeArchivoEnDataGridView = new DesplegadorDeArchivoEnDataGridView();
                desplegadorDeArchivoEnDataGridView.Proceso(dataGridViewCargadorDeDatos, direccionArchivo, titulosExistentes, separadorColumnas);

                comboBoxColumnaDatos.Items.Clear();
                comboBoxColumnaFechas.Items.Clear();
                columnaDatos = 0;
                columnaFechas = 0;
                for(int contador = 0; contador < dataGridViewCargadorDeDatos.ColumnCount; contador++) //Pone los títulos de las columnas en los comboBox para que el usuario pueda elegir.
                {
                    comboBoxColumnaDatos.Items.Add(dataGridViewCargadorDeDatos.Columns[contador].Name);
                    comboBoxColumnaFechas.Items.Add(dataGridViewCargadorDeDatos.Columns[contador].Name);
                }

                if (dataGridViewCargadorDeDatos.ColumnCount >3) //Si hay muchas columnas, asegura un ancho determinado para que se puedan ver.
                {
                    dataGridViewCargadorDeDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                    for (int i = 0; i < dataGridViewCargadorDeDatos.ColumnCount; i++)
                    {
                        dataGridViewCargadorDeDatos.Columns[i].Width = dataGridViewCargadorDeDatos.Width / 3;
                    }
                }
                else
                {
                    dataGridViewCargadorDeDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }

            }
            else
            {
                MessageBox.Show("Debe Existir un separador de columnas", "Error separador", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            buttonAceptar.Enabled = false;

        }

        protected void buttonBuscarArchivo_Click(object sender, EventArgs e)
        {
            buttonVisualizar.Enabled = false;
            openFileDialogBuscarArchivo.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Inicializa el buscador de archivos en la carpeta mis documentos.
            openFileDialogBuscarArchivo.Filter = "Archivos de texto (*.txt)|*.txt|Archivos csv (*.csv)|*.csv"; //Muestra solo los archivos .txt o .csv segun se especifique.
            openFileDialogBuscarArchivo.FilterIndex = 0; //Inicia mostrando archivos .txt

            if (openFileDialogBuscarArchivo.ShowDialog() == DialogResult.OK)
            {
                direccionArchivo = openFileDialogBuscarArchivo.FileName;
                nombreArchivo = Path.GetFileName(direccionArchivo);

                if(nombreArchivo.Length > 17)
                {
                    labelNombreArchivo.Text = "Archivo acual: " + nombreArchivo.Substring(0, 14) + "..." + Path.GetExtension(direccionArchivo);
                }
                else
                {
                    labelNombreArchivo.Text = "Archivo actual: " + nombreArchivo;
                }
                buttonVisualizar.Enabled = true;
            }

            buttonAceptar.Enabled = false;
        }

        public class DatosCargadosEventArgs : EventArgs //Permite crear un evento que pase los datos como argumentos.
        {
            public List<DateTime> listaFechas;
            public List<double> listaDatos;
            public int longitudCadenaDeFecha;
        }
    }
}
