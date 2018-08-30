using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Globalization;

namespace DeclineR.Pantalla_Principal
{
    public partial class CargadorDeDatosDeExcel : Form
    {
        protected List<DateTime> listaFechas = new List<DateTime>();
        protected List<double> listaDatos = new List<double>();
        protected string separadorDecimal = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
        protected string separadorDeMiles;
        protected string formatoFecha;
        protected DateTime desdeFecha, hastaFecha, fechaTemporal;
        protected int columnaFechas = 0, columnaDatos = 1, longitudCadenaDeFecha;
        protected double intervaloDeTiempo=1;
        protected bool intervaloEnMeses;

        CargadorDeDatosDelDataGrid cargadorDeDatosDelDataGrid = new CargadorDeDatosDelDataGrid();
        CopiadorDeDatosDeExcelADataGrid copiadorDeDatosDeExcelADataGrid = new CopiadorDeDatosDeExcelADataGrid();
        ManejadorDeDatos manejadorDeDatos = new ManejadorDeDatos();

        public EventHandler<DatosCargadosEventArgs> datosCargados;


        protected void cargadorDeDatosDeExcel_Load(object sender, EventArgs e)
        {
            dataGridViewCargadorDeDatos.RowCount = 2;
            dataGridViewCargadorDeDatos.ColumnCount = 2;
            dataGridViewCargadorDeDatos.Columns[0].Name = "Fecha";
            dataGridViewCargadorDeDatos.Columns[1].Name = "Produccion (STB/dia)";

            if (separadorDecimal == ",")
            {
                comboBoxSeparadorDecimal.SelectedIndex = 0;
            }
            else
            {
                comboBoxSeparadorDecimal.SelectedIndex = 1;
            }
            
            comboBoxFormatoDeFecha.SelectedIndex = 0;
            formatoFecha = comboBoxFormatoDeFecha.SelectedItem.ToString();
            longitudCadenaDeFecha = formatoFecha.Length;
            comboBoxSeparadorDeMiles.SelectedIndex = 3;
            comboBoxIntervaloDeTiempo.SelectedIndex = 0;
        }

        protected void buttonCargar_Click(object sender, EventArgs e) //Llama los métodos para guardar el texto del dataGridView en las listas de fecha y datos.
        {
            
            listaDatos.Clear();
            listaFechas.Clear();
            longitudCadenaDeFecha = formatoFecha.Length;

            cargadorDeDatosDelDataGrid.Proceso(dataGridViewCargadorDeDatos, columnaFechas, columnaDatos, separadorDecimal, formatoFecha, separadorDeMiles, listaFechas, listaDatos);
            manejadorDeDatos.DesplegarDatosAlDataGrid(dataGridViewCargadorDeDatos, columnaFechas, columnaDatos, listaFechas, listaDatos, longitudCadenaDeFecha);

            buttonAceptar.Enabled = true;
        }

        public CargadorDeDatosDeExcel()
        {
            InitializeComponent();
        }
        
        protected void dataGridViewCargadorDeDatos_KeyDown(object sender, KeyEventArgs e) //Captura el evento "ctrl + v" y llama el método necesario para analizar los datos y desplegarlos en el dataGridView.
        {
            if (e.Control && e.KeyCode == Keys.C) // Captura el evento Ctrl + c del teclado, y copia los datos del grid
            {
                copiadorDeDatosDeExcelADataGrid.Copiar(sender, dataGridViewCargadorDeDatos, checkBoxDefinirRangoDeFechas);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.V) // Captura el evento Ctrl + v del teclado, y pega los datos en el grid
            {
                copiadorDeDatosDeExcelADataGrid.Pegar(sender, dataGridViewCargadorDeDatos, checkBoxDefinirRangoDeFechas);
            }
        }
        
        protected void checkBoxDefinirRangoDeFechas_CheckedChanged(object sender, EventArgs e)
        {
            buttonAceptar.Enabled = false;

            if (checkBoxDefinirRangoDeFechas.Checked)
            {
                radioButtonIntervaloEnDias.Enabled = true;
                radioButtonIntervaloEnMeses.Enabled = true;
                labelIntervaloDeTiempo.Enabled = true;
                comboBoxIntervaloDeTiempo.Enabled = true;
                buttonDesde.Enabled = true;
                labelDesdeFecha.Enabled = true;
                buttonHasta.Enabled = true;
                labelHastaFecha.Enabled = true;
                buttonAñadirRangoDeFechas.Enabled = true;
                comboBoxFormatoDeFecha.Enabled = false;
                labelFormatoDeFecha.Enabled = false;
                dataGridViewCargadorDeDatos.Columns[0].ReadOnly = true;
            }
            else
            {
                radioButtonIntervaloEnDias.Enabled = false;
                radioButtonIntervaloEnMeses.Enabled = false;
                labelIntervaloDeTiempo.Enabled = false;
                comboBoxIntervaloDeTiempo.Enabled = false;
                buttonDesde.Enabled = false;
                labelDesdeFecha.Enabled = false;
                buttonHasta.Enabled = false;
                labelHastaFecha.Enabled = false;
                buttonAñadirRangoDeFechas.Enabled = false;
                comboBoxFormatoDeFecha.Enabled = true;
                labelFormatoDeFecha.Enabled = true;
                dataGridViewCargadorDeDatos.Columns[0].ReadOnly = false;

            }
        }

        protected void buttonAñadirRenglon_Click(object sender, EventArgs e)
        {
            buttonAceptar.Enabled = false;

            if (dataGridViewCargadorDeDatos.RowCount > 0)
            {
                int renglon = dataGridViewCargadorDeDatos.CurrentCell.RowIndex;
                dataGridViewCargadorDeDatos.Rows.Insert(renglon + 1);
            }
            else
            {
                dataGridViewCargadorDeDatos.RowCount = 1;
            }
        }

        protected void buttonEliminarRenglon_Click(object sender, EventArgs e)
        {
            buttonAceptar.Enabled = false;

            if (dataGridViewCargadorDeDatos.RowCount > 1)
            {
                int renglon = dataGridViewCargadorDeDatos.CurrentCell.RowIndex;
                dataGridViewCargadorDeDatos.Rows.RemoveAt(renglon);
            }

        }

        protected void buttonBorrarDatos_Click(object sender, EventArgs e)
        {
            buttonAceptar.Enabled = false;

            dataGridViewCargadorDeDatos.RowCount = 0;
            dataGridViewCargadorDeDatos.RowCount = 1;
            listaDatos.Clear();
            listaFechas.Clear();
        }

        protected void buttonHasta_Click(object sender, EventArgs e)//Selecciona la fecha final del rango de fechas que se desea añadir.
        {
            buttonAceptar.Enabled = false;

            DateTime evaluadorFechaInicial = new DateTime();
            
            if (desdeFecha != evaluadorFechaInicial) //Se asegura de que la fecha inicial ya haya sido seleccionada.
            {
                seleccionadorDeFecha seleccionadorDeFecha = new seleccionadorDeFecha(); 
                seleccionadorDeFecha.fechaSeleccionada += seleccionadorDeFecha_fechaSeleccionada;
                seleccionadorDeFecha.ShowDialog(this);

                if (fechaTemporal >= desdeFecha) //Evalua la fecha escogida como fecha final para asegurarse de que sea posterior a la inicial.
                {
                    hastaFecha = fechaTemporal;
                    buttonHasta.Text = hastaFecha.Date.ToString();
                }
                else
                {
                    MessageBox.Show("La fecha final debe ser posterior a la inicial", "Error al seleccionar fecha final", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione primero la fecha inicial", "Fecha inicial no seleccionada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        
        protected void buttonDesde_Click(object sender, EventArgs e) //Selecciona la fecha inicial del rango de fechas que se desea añadir.
        {
            buttonAceptar.Enabled = false;

            seleccionadorDeFecha seleccionadorDeFecha = new seleccionadorDeFecha();
            seleccionadorDeFecha.fechaSeleccionada += seleccionadorDeFecha_fechaSeleccionada;
            seleccionadorDeFecha.ShowDialog(this);
            if (desdeFecha != fechaTemporal)
            {
                desdeFecha = fechaTemporal;
                buttonDesde.Text = desdeFecha.Date.ToString();
            }
        }

        protected void buttonAñadirRangoDeFechas_Click(object sender, EventArgs e)
        {
            buttonAceptar.Enabled = false;

            DateTime evaluadorFechas = new DateTime();
            if (desdeFecha != evaluadorFechas && hastaFecha != evaluadorFechas) //Se asegura de que las fechas hayan sido elegidas.
            {
                listaFechas.Clear();
                fechaTemporal = desdeFecha;

                while (fechaTemporal <= hastaFecha)
                {
                    listaFechas.Add(fechaTemporal);
                    if(intervaloEnMeses)
                    {
                        fechaTemporal = fechaTemporal.AddMonths(Convert.ToInt32(intervaloDeTiempo));
                    }
                    else
                    {
                        fechaTemporal = fechaTemporal.AddDays(intervaloDeTiempo);
                    }
                    
                }

                dataGridViewCargadorDeDatos.RowCount = 0;
                dataGridViewCargadorDeDatos.RowCount = listaFechas.Count();

                int renglon = 0;
                foreach (DateTime fecha in listaFechas)
                {
                    dataGridViewCargadorDeDatos[0, renglon].Value = fecha.Date.ToString().Substring(0, formatoFecha.Length);
                    renglon++;
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione las fechas inicial y final", "Fechas no seleccionadas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected void comboBoxSeparadorDecimal_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonAceptar.Enabled = false;

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

            if (separadorDeMiles == separadorDecimal && separadorDecimal != "")
            {
                MessageBox.Show("Los separadores no pueden ser iguales", "Error separadores", MessageBoxButtons.OK, MessageBoxIcon.Error);
                separadorDeMiles = "";
                comboBoxSeparadorDeMiles.SelectedIndex = 3;
            }

        }

        protected void comboBoxSeparadorDeMiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonAceptar.Enabled = false;

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
        }

        private void buttonPegarDatos_Click(object sender, EventArgs e)
        {
            copiadorDeDatosDeExcelADataGrid.Pegar(sender, dataGridViewCargadorDeDatos, checkBoxDefinirRangoDeFechas);
        }

        private void comboBoxIntervaloDeTiempo_SelectedIndexChanged(object sender, EventArgs e)
        {
            intervaloDeTiempo = comboBoxIntervaloDeTiempo.SelectedIndex + 1;
        }

        private void radioButtonIntervaloEnDias_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButtonIntervaloEnDias.Checked)
            {
                intervaloEnMeses = false;
                comboBoxIntervaloDeTiempo.SelectedIndex = 0;
                comboBoxIntervaloDeTiempo.Enabled = true;
            }
            else
            {
                intervaloEnMeses = true;
                comboBoxIntervaloDeTiempo.SelectedIndex = 0;
                intervaloDeTiempo = 1;
                comboBoxIntervaloDeTiempo.Enabled = false;
            }
        }

        protected void buttonAceptar_Click(object sender, EventArgs e) //Crea el evento para enviar los datos al formulario que lo llamo.
        {
            EventHandler<DatosCargadosEventArgs> manejadorDeEvento = datosCargados;
            manejadorDeEvento(this, new DatosCargadosEventArgs() { listaDatos = listaDatos, listaFechas = listaFechas, longitudCadenaDeFecha = longitudCadenaDeFecha});
            this.Close();
        }

        protected void comboBoxFormatoDeFecha_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonAceptar.Enabled = false;

            formatoFecha = comboBoxFormatoDeFecha.SelectedItem.ToString();
        }

        protected void seleccionadorDeFecha_fechaSeleccionada(object sender, FechaSeleccionadaEvenArgs e)
        {
            fechaTemporal = e.fecha;
        }

        public class DatosCargadosEventArgs : EventArgs //Hace posible crear un evento que contenga los datos los envíe.
        {
            public List<DateTime> listaFechas;
            public List<double> listaDatos;
            public int longitudCadenaDeFecha;
        }
    }
}

