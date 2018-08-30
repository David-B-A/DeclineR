using System;
using System.Collections.Generic; //Para uso de listas
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using RDotNet; //Para utilizar el lenguaje de programacion R
using System.Globalization; //Para obtener separador decimal y otras cosas de la configuracion del usuario
using System.IO; //Para poder escribir y manejar archivos
using Excel = Microsoft.Office.Interop.Excel; //Para crear un archivo de Excel
using System.Diagnostics; // Para poder guardar archivos

namespace DeclineR.Pantalla_Principal
{
    public partial class DeclineRFormularioPrincipal : Form
    {
        //Las variables que se usan en R internamente seran llamadas de igual forma que sus equivalentes objetos de RDotNet en C#

        protected string direcciónManualDeUsuario = Path.GetFullPath("MANUAL DE USUARIO - DeclineR.pdf");
        protected string separadorDecimal = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
        
        string direccionFuncionParaHallarMejorArimaConAIC = Path.GetFullPath("mejor ARIMA por AIC.R");
        protected int longitudCadenaDeFecha; //Guardará la longitud de la cadena de fecha seleccionada.

        protected List<DateTime> listaFechas = new List<DateTime>();
        protected List<double> listaDatos = new List<double>();
        protected int intervaloDeTiempo = 1, columnaFechas = 0, columnaDatos = 1;
        protected bool intervaloEnMeses = false;
        protected bool serieComprobada = false; //Es modificado (convertido en false) por todos los eventos que están antes de comprobar la serie. Cuando se comprueva su valor cambia a true.

        protected int indiceTipoDeAjuste, posicionDatoMaximo;
        protected int margenVisual = 20, usarDatosDesde = 0, usarDatosHasta = 0;
        protected double[] vectorDatosAjustados;

        protected double datoMaximo;

        ManejadorDeDatos manejadorDeDatos = new ManejadorDeDatos();
        Graficador graficador = new Graficador();

        REngine motorDeR = REngine.GetInstance();
        RDotNet.NumericVector datosParaR; //Guardará los datos seleccionados por el usuario.
        string nombreDelVectorDeDatosEnR = "Producción";

        protected DateTime fechaTemporal;

        #region datosDeTodosLosModelosEmpleados

        string[] vectorModelosUsados;
        int numeroDeModelosUsados = 0;
        string[] vectorAicDeModelosUsados;
        string[] vectorR2DeModelosUsados;
        string[] vectorNDeModelosUsados;
        string[] vectorDiDeModelosUsados;
        string[] vectorProduccionInicialDeModelosUsados;
        string[] vectorFechaDatoInicialDeModelosUsados;
        string[] vectorFechaDatoFinalDeModelosUsados;

        protected void ActualizarVectorModelosUsados(ref string[] vectorModelosUsados, int numeroDeModelosUsados, string modeloUsado)
        {
            string[] vectorLocalModelos = new string[numeroDeModelosUsados];
            if(numeroDeModelosUsados > 1)
            {
                for(int i = 0; i < numeroDeModelosUsados - 1; i++)
                {
                    vectorLocalModelos[i] = vectorModelosUsados[i];
                }
            }
            vectorLocalModelos[numeroDeModelosUsados - 1] = modeloUsado;

            vectorModelosUsados = vectorLocalModelos;
        }

        protected void ActualizarVectorDeParametros(ref string[] vectorDeParametro, int numeroDeModelosUsados, string parametro)
        {
            string[] vectorLocal = new string[numeroDeModelosUsados];
            if (numeroDeModelosUsados > 1)
            {
                for (int i = 0; i < numeroDeModelosUsados - 1; i++)
                {
                    vectorLocal[i] = vectorDeParametro[i];
                }
            }
            vectorLocal[numeroDeModelosUsados - 1] = parametro;

            vectorDeParametro = vectorLocal;
        }

        private void verInformaciónDeModelosUsadosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InformacionDeModelosUsados informacionModelosUsados = new InformacionDeModelosUsados(vectorModelosUsados, vectorFechaDatoInicialDeModelosUsados, vectorFechaDatoFinalDeModelosUsados, vectorAicDeModelosUsados, vectorR2DeModelosUsados, vectorNDeModelosUsados, vectorDiDeModelosUsados, vectorProduccionInicialDeModelosUsados);
            informacionModelosUsados.ShowDialog();
        }

        #endregion


        public DeclineRFormularioPrincipal()
        {
            InitializeComponent();
        }

        private void arimaForecastingInicio_Load(object sender, EventArgs e)
        {
            
            try
            {
                motorDeR.Evaluate("library(forecast)");
                motorDeR.Evaluate("library(TSA)");
                motorDeR.Evaluate("library(tseries)");
            }
            catch
            {
                MessageBox.Show("No se pudo cargar las librerias necesarias.\nPor favor intente nuevamente", "Error de librerias", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            //Para la primera pagina (Datos)

            dataGridViewDatos.RowCount = 2;
            dataGridViewDatos.ColumnCount = 2;
            dataGridViewDatos.Columns[0].Name = "Fecha";
            dataGridViewDatos.Columns[1].Name = "Producción (STB/dia)";
            tabControlArimaInicio.Height = this.ClientSize.Height - menuStrip.Height;
            tabControlArimaInicio.Width = this.ClientSize.Width;
            comboBoxIntervaloDeTiempo.SelectedIndex = 0;
            comboBoxTipoDeAjuste.SelectedIndex = 0;
            comboBoxUsarDatosDesde.SelectedIndex = 0;
            comboBoxUsarDatosHasta.SelectedIndex = 0;
            serieComprobada = false;
            ContinuarConSerieComprobada(serieComprobada);

            //Para la segunda pagina (Modelo)

            comboBoxIntervaloDeTiempo.SelectedIndex = 0;
        }

        private void arimaForecastingInicio_Resize(object sender, EventArgs e)
        {
            if(ClientSize.Width < 1000 || this.ClientSize.Height < 500)
            {
                if (ClientSize.Width < 1000)
                {
                    tabControlArimaInicio.Width = 1000;
                }
                else
                {
                    tabControlArimaInicio.Width = this.ClientSize.Width;
                }

                if (this.ClientSize.Height < 500)
                {
                    tabControlArimaInicio.Height = 500;
                }
                else
                {
                    tabControlArimaInicio.Height = this.ClientSize.Height - menuStrip.Height - 5;
                }
                this.AutoScroll = true;
            }

            else
            {
                tabControlArimaInicio.Height = this.ClientSize.Height - menuStrip.Height;
                tabControlArimaInicio.Width = this.ClientSize.Width;
                this.AutoScroll = false;
            }

            
        }


        #region tabPageDatos

        private void tabPageDatos_SizeChanged(object sender, EventArgs e)
        {
            dataGridViewDatos.Height = tabPageDatos.Height - dataGridViewDatos.Location.Y - margenVisual;
            trackBarUsarDatosDesde.Width = tabPageDatos.Width - trackBarUsarDatosDesde.Location.X - margenVisual;
            pictureBoxGraficaDatos.Width = tabPageDatos.Width - pictureBoxGraficaDatos.Location.X - margenVisual;
            pictureBoxGraficaDatos.Height = tabPageDatos.Height - pictureBoxGraficaDatos.Location.Y - margenVisual - labelFechaFinal.Height - 3;

            labelCeroProduccion.Location = new System.Drawing.Point(labelCeroProduccion.Location.X, pictureBoxGraficaDatos.Location.Y + pictureBoxGraficaDatos.Height - labelCeroProduccion.Height);
            labelFechaInicial.Location = new System.Drawing.Point(labelFechaInicial.Location.X, tabPageDatos.Height - margenVisual - labelFechaInicial.Height);
            labelFechaFinal.Location = new System.Drawing.Point(trackBarUsarDatosDesde.Location.X + trackBarUsarDatosDesde.Width - labelFechaFinal.Width, tabPageDatos.Height - margenVisual - labelFechaFinal.Height);

            if (serieComprobada)
            {
                GraficarTodo(vectorDatosAjustados, usarDatosDesde, usarDatosHasta);
            }
        }

        private void buttonAñadirRenglon_Click(object sender, EventArgs e)
        {
            int renglon = dataGridViewDatos.CurrentCell.RowIndex;
            dataGridViewDatos.Rows.Insert(renglon + 1);
        }

        private void buttonEliminarRenglon_Click(object sender, EventArgs e)
        {
            serieComprobada = false;
            ContinuarConSerieComprobada(serieComprobada);

            if (dataGridViewDatos.RowCount > 1)
            {
                int renglon = dataGridViewDatos.CurrentCell.RowIndex;
                dataGridViewDatos.Rows.RemoveAt(renglon);
                listaDatos.RemoveAt(renglon);
                listaFechas.RemoveAt(renglon);
            }
        }

        private void buttonBorrarDatos_Click(object sender, EventArgs e)
        {
            serieComprobada = false;
            ContinuarConSerieComprobada(serieComprobada);

            dataGridViewDatos.RowCount = 0;
            dataGridViewDatos.RowCount = 1;
            listaDatos.Clear();
            listaFechas.Clear();
        }

        private void comboBoxIntervaloDeTiempo_SelectedIndexChanged(object sender, EventArgs e)
        {
            serieComprobada = false;
            ContinuarConSerieComprobada(serieComprobada);

            switch (comboBoxIntervaloDeTiempo.SelectedIndex)
            {
                case 0:
                    intervaloEnMeses = false;
                    intervaloDeTiempo = 1;
                    labelIntervaloMasDias.Enabled = false;
                    textBoxIntervaloMasDias.Enabled = false;
                    textBoxIntervaloMasDias.Text = "";
                    buttonIntervaloMasTiempo.Enabled = false;
                    break;
                case 1:
                    intervaloEnMeses = false;
                    intervaloDeTiempo = 2;
                    labelIntervaloMasDias.Enabled = false;
                    textBoxIntervaloMasDias.Enabled = false;
                    textBoxIntervaloMasDias.Text = "";
                    buttonIntervaloMasTiempo.Enabled = false;
                    break;
                case 2:
                    intervaloEnMeses = false;
                    intervaloDeTiempo = 7;
                    labelIntervaloMasDias.Enabled = false;
                    textBoxIntervaloMasDias.Enabled = false;
                    textBoxIntervaloMasDias.Text = "";
                    buttonIntervaloMasTiempo.Enabled = false;
                    break;
                case 3:
                    intervaloEnMeses = true;
                    intervaloDeTiempo = 1;
                    labelIntervaloMasDias.Enabled = false;
                    textBoxIntervaloMasDias.Enabled = false;
                    textBoxIntervaloMasDias.Text = "";
                    buttonIntervaloMasTiempo.Enabled = false;
                    break;
                case 4:
                    intervaloEnMeses = false;
                    textBoxIntervaloMasDias.Clear();
                    labelIntervaloMasDias.Enabled = true;
                    textBoxIntervaloMasDias.Enabled = true;
                    buttonIntervaloMasTiempo.Enabled = true;
                    break;
            }
        }

        private void buttonIntervaloMasTiempo_Click(object sender, EventArgs e)
        {
            serieComprobada = false;
            ContinuarConSerieComprobada(serieComprobada);

            try
            {
                intervaloDeTiempo = Convert.ToInt32(textBoxIntervaloMasDias.Text);
                textBoxIntervaloMasDias.Text = Convert.ToString(intervaloDeTiempo);
            }
            catch
            {
                MessageBox.Show("No se puede identificar el intervalo.\nRecuerde que debe ser un entero positivo", "Error intervalo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxIntervaloMasDias.Clear();
                intervaloDeTiempo = 0;
            }
        }

        private void copiarDatosDesdeExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (numeroDeModelosUsados > 0)
            {
                if (MessageBox.Show("Si carga datos nuevos se perderá la información de todos los modelos que usó en esta sesión.\n\n¿Desea continuar?", "Cargar datos", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    serieComprobada = false;
                    ContinuarConSerieComprobada(serieComprobada);

                    CargadorDeDatosDeExcel cargadorDeDatosDeExcel = new CargadorDeDatosDeExcel();
                    cargadorDeDatosDeExcel.datosCargados += cargadorDeDatosDeExcel_DatosCargados;
                    cargadorDeDatosDeExcel.ShowDialog();

                    manejadorDeDatos.DesplegarDatosAlDataGrid(dataGridViewDatos, columnaFechas, columnaDatos, listaFechas, listaDatos, longitudCadenaDeFecha);
                }
            }
            else
            {
                serieComprobada = false;
                ContinuarConSerieComprobada(serieComprobada);

                CargadorDeDatosDeExcel cargadorDeDatosDeExcel = new CargadorDeDatosDeExcel();
                cargadorDeDatosDeExcel.datosCargados += cargadorDeDatosDeExcel_DatosCargados;
                cargadorDeDatosDeExcel.ShowDialog();

                manejadorDeDatos.DesplegarDatosAlDataGrid(dataGridViewDatos, columnaFechas, columnaDatos, listaFechas, listaDatos, longitudCadenaDeFecha);
            }
            
        }

        private void comboBoxUsarDatosHasta_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serieComprobada)
            {
                switch (comboBoxUsarDatosHasta.SelectedIndex)
                {
                    case 0:
                        usarDatosHasta = listaFechas.Count - 1;
                        textBoxUsarDatosHasta.Clear();
                        labelEspecificarValorDeJ.Enabled = false;
                        textBoxUsarDatosHasta.Enabled = false;
                        textBoxUsarDatosHasta.Text = "";
                        labelFechaUltimoDato.Enabled = false;
                        buttonUsarDatosHasta.Enabled = false;
                        try
                        {
                            buttonUsarDatosHasta.Text = listaFechas[usarDatosHasta].Date.ToString().Substring(0, longitudCadenaDeFecha);
                        }
                        catch
                        {
                            buttonUsarDatosDesde.Text = "Fecha último dato";
                        }
                        GraficarTodo(vectorDatosAjustados, usarDatosDesde, usarDatosHasta);

                        PasarDatosAR(vectorDatosAjustados, datosParaR, usarDatosDesde, usarDatosHasta);
                        break;
                    case 1:
                        usarDatosHasta = listaFechas.Count - 1;
                        labelEspecificarValorDeJ.Enabled = true;
                        textBoxUsarDatosHasta.Enabled = true;
                        textBoxUsarDatosHasta.Text = "0";
                        labelFechaUltimoDato.Enabled = true;
                        buttonUsarDatosHasta.Enabled = true;
                        buttonUsarDatosHasta.Text = listaFechas[usarDatosHasta].Date.ToString().Substring(0, longitudCadenaDeFecha);
                        break;
                }
            }
        }

        private void comboBoxTipoDeAjuste_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serieComprobada)
            {
                indiceTipoDeAjuste = comboBoxTipoDeAjuste.SelectedIndex;
                switch (indiceTipoDeAjuste)
                {
                    case 0:

                        double[] vectorLocalDatosAjustados = new double[listaDatos.Count];
                        for (int a = 0; a < listaDatos.Count; a++)
                        {
                            vectorLocalDatosAjustados[a] = listaDatos[a];
                        }
                        vectorDatosAjustados = vectorLocalDatosAjustados;

                        manejadorDeDatos.DesplegarDatosAlDataGrid(dataGridViewDatos, columnaFechas, columnaDatos, listaFechas, vectorDatosAjustados, longitudCadenaDeFecha);
                        GraficarTodo(vectorDatosAjustados, usarDatosDesde, usarDatosHasta);
                        break;
                    case 1:
                        manejadorDeDatos.AjustarDatosPorMediaMovil(listaDatos, ref vectorDatosAjustados);
                        manejadorDeDatos.DesplegarDatosAlDataGrid(dataGridViewDatos, columnaFechas, columnaDatos, listaFechas, vectorDatosAjustados, longitudCadenaDeFecha);
                        GraficarTodo(vectorDatosAjustados, usarDatosDesde, usarDatosHasta);
                        break;
                }

                PasarDatosAR(vectorDatosAjustados, datosParaR, usarDatosDesde, usarDatosHasta);

            }
        }

        private void trackBarUsarDatosDesde_Scroll(object sender, EventArgs e)
        {
            if (serieComprobada)
            {
                try
                {
                    textBoxUsarDatosDesde.Text = trackBarUsarDatosDesde.Value.ToString();
                    usarDatosDesde = trackBarUsarDatosDesde.Value;
                    buttonUsarDatosDesde.Text = listaFechas[usarDatosDesde].Date.ToString().Substring(0, longitudCadenaDeFecha);
                    labelUsarDatosHasta.Enabled = true;
                    comboBoxUsarDatosHasta.Enabled = true;
                    comboBoxUsarDatosHasta.SelectedIndex = 0;

                    comboBoxUsarDatosDesde.Text = listaFechas[usarDatosDesde].ToString().Substring(0, longitudCadenaDeFecha);

                    GraficarTodo(vectorDatosAjustados, usarDatosDesde, usarDatosHasta);
                    PasarDatosAR(vectorDatosAjustados, datosParaR, usarDatosDesde, usarDatosHasta);
                }
                catch
                {
                }
            }
        }

        private void comboBoxUsarDatosDesde_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serieComprobada)
            {
                switch (comboBoxUsarDatosDesde.SelectedIndex)
                {
                    case 0:
                        usarDatosDesde = 0;
                        trackBarUsarDatosDesde.Value = 0;
                        trackBarUsarDatosDesde.Enabled = false;
                        textBoxUsarDatosDesde.Enabled = false;
                        textBoxUsarDatosDesde.Text = "0";
                        buttonUsarDatosDesde.Enabled = false;
                        try
                        {
                            buttonUsarDatosDesde.Text = listaFechas[usarDatosDesde].Date.ToString().Substring(0, longitudCadenaDeFecha);
                        }
                        catch
                        {
                            buttonUsarDatosDesde.Text = "Fecha primer dato";
                        }
                        labelUsarDatosHasta.Enabled = true;
                        comboBoxUsarDatosHasta.Enabled = true;
                        GraficarTodo(vectorDatosAjustados, usarDatosDesde, usarDatosHasta);

                        PasarDatosAR(vectorDatosAjustados, datosParaR, usarDatosDesde, usarDatosHasta);
                        break;
                    case 1:
                        manejadorDeDatos.EncontrarMaximo(vectorDatosAjustados, out datoMaximo, out posicionDatoMaximo);
                        usarDatosDesde = posicionDatoMaximo;
                        trackBarUsarDatosDesde.Value = posicionDatoMaximo;
                        textBoxUsarDatosDesde.Text = posicionDatoMaximo.ToString();
                        trackBarUsarDatosDesde.Enabled = false;
                        textBoxUsarDatosDesde.Enabled = false;
                        buttonUsarDatosDesde.Enabled = false;
                        buttonUsarDatosDesde.Text = listaFechas[usarDatosDesde].Date.ToString().Substring(0, longitudCadenaDeFecha);

                        labelUsarDatosHasta.Enabled = true;
                        comboBoxUsarDatosHasta.Enabled = true;
                        GraficarTodo(vectorDatosAjustados, usarDatosDesde, usarDatosHasta);

                        PasarDatosAR(vectorDatosAjustados, datosParaR, usarDatosDesde, usarDatosHasta);
                        break;
                    case 2:
                        usarDatosDesde = 0;
                        trackBarUsarDatosDesde.Minimum = 0;
                        trackBarUsarDatosDesde.Maximum = vectorDatosAjustados.Length - 2;
                        trackBarUsarDatosDesde.Value = 0;
                        trackBarUsarDatosDesde.Enabled = true;
                        textBoxUsarDatosDesde.Text = "0";
                        buttonUsarDatosDesde.Enabled = true;
                        buttonUsarDatosDesde.Text = listaFechas[usarDatosDesde].Date.ToString().Substring(0, longitudCadenaDeFecha);
                        textBoxUsarDatosDesde.Enabled = true;
                        labelUsarDatosHasta.Enabled = false;
                        comboBoxUsarDatosHasta.Enabled = false;
                        GraficarTodo(vectorDatosAjustados, usarDatosDesde, usarDatosHasta);
                        break;
                }

            }
        }

        private void cargadorDeDatosDeExcel_DatosCargados(Object sender, CargadorDeDatosDeExcel.DatosCargadosEventArgs e)
        {
            listaDatos = e.listaDatos;
            listaFechas = e.listaFechas;
            longitudCadenaDeFecha = e.longitudCadenaDeFecha;
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void funcionDeAutocorrelaciónSimpleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            motorDeR.Evaluate("acf(" + nombreDelVectorDeDatosEnR +")");
        }

        private void funciónDeAutocorrelaciónParcialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            motorDeR.Evaluate("pacf(" + nombreDelVectorDeDatosEnR + ")");
        }

        private void abrirDesdeUnArchivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (numeroDeModelosUsados > 0)
            {
                if (MessageBox.Show("Si carga datos nuevos se perderá la información de todos los modelos que usó en esta sesión.\n\n¿Desea continuar?", "Cargar datos", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    serieComprobada = false;
                    ContinuarConSerieComprobada(serieComprobada);

                    CargadorDeArchivos cargadorDeArchivos = new CargadorDeArchivos();
                    cargadorDeArchivos.datosCargados += cargadorDeArchivos_DatosCargados;
                    cargadorDeArchivos.ShowDialog();

                    manejadorDeDatos.DesplegarDatosAlDataGrid(dataGridViewDatos, columnaFechas, columnaDatos, listaFechas, listaDatos, longitudCadenaDeFecha);
                }
            }
            else
            {
                serieComprobada = false;
                ContinuarConSerieComprobada(serieComprobada);

                CargadorDeArchivos cargadorDeArchivos = new CargadorDeArchivos();
                cargadorDeArchivos.datosCargados += cargadorDeArchivos_DatosCargados;
                cargadorDeArchivos.ShowDialog();

                manejadorDeDatos.DesplegarDatosAlDataGrid(dataGridViewDatos, columnaFechas, columnaDatos, listaFechas, listaDatos, longitudCadenaDeFecha);
            }
        }

        private void buttonComprobarSerie_Click(object sender, EventArgs e)
        {
            usarDatosHasta = 0;
            
            manejadorDeDatos.EvaluarContinuidadDeDatosYDatosNegativos(listaFechas, listaDatos, intervaloDeTiempo, intervaloEnMeses);
            manejadorDeDatos.DesplegarDatosAlDataGrid(dataGridViewDatos, columnaFechas, columnaDatos, listaFechas, listaDatos, longitudCadenaDeFecha);

            usarDatosHasta = listaFechas.Count - 1;
            serieComprobada = true;
            ContinuarConSerieComprobada(serieComprobada);
        }

        protected void cargadorDeArchivos_DatosCargados (Object sender, CargadorDeArchivos.DatosCargadosEventArgs e)
        {
            listaDatos = e.listaDatos;
            listaFechas = e.listaFechas;
            longitudCadenaDeFecha = e.longitudCadenaDeFecha;
        }

        private void GraficarTodo(double[] vectorDatosAcutal, int usarDatosDesde, int usarDatosHasta)
        {
            Graphics papel = pictureBoxGraficaDatos.CreateGraphics();
            Pen lapiz = new Pen(Color.Black);

            //La variable numeroDeDatos equivale a la cantidad de observaciones que se tienen
            //La variable cantidadDeDatosGraficados hace referencia al numero de datos que se puede graficar (ya que el numero de datos que se puede graficar está limitado al numero de pixeles disponible)
            int cantidadDeDatosGraficados = 0, numeroDeDatos = vectorDatosAcutal.Length; 

            double datoMaximo;
            int posicionMaximo = 0;
            manejadorDeDatos.EncontrarMaximo(vectorDatosAjustados, out datoMaximo, out posicionMaximo);

            graficador.BorrarGrafico(pictureBoxGraficaDatos, papel);
            graficador.GraficarDatos(pictureBoxGraficaDatos, papel, lapiz, vectorDatosAcutal, numeroDeDatos, ref cantidadDeDatosGraficados, datoMaximo);

            if (cantidadDeDatosGraficados > 0)
            {
                if(vectorDatosAcutal.Length > pictureBoxGraficaDatos.Width)
                {
                    trackBarUsarDatosDesde.Width = cantidadDeDatosGraficados - 1; 
                    labelFechaFinal.Location = new System.Drawing.Point(trackBarUsarDatosDesde.Location.X + trackBarUsarDatosDesde.Width - labelFechaFinal.Width / 2, tabPageDatos.Height - margenVisual - labelFechaFinal.Height);

                }
                else
                {
                    trackBarUsarDatosDesde.Width = (vectorDatosAcutal.Length - 1) * Convert.ToInt32(pictureBoxGraficaDatos.Width / vectorDatosAcutal.Length);
                    labelFechaFinal.Location = new System.Drawing.Point(trackBarUsarDatosDesde.Location.X + trackBarUsarDatosDesde.Width - labelFechaFinal.Width, tabPageDatos.Height - margenVisual - labelFechaFinal.Height);
                }

                graficador.GraficarLimite(pictureBoxGraficaDatos, numeroDeDatos, papel, lapiz, usarDatosDesde);
                graficador.GraficarLimite(pictureBoxGraficaDatos, numeroDeDatos, papel, lapiz, usarDatosHasta);
            }

            if(datoMaximo > 0)
            {
                labelProduccionMaxima.Text = Convert.ToString(Convert.ToInt32(datoMaximo));
            }
            if(listaFechas.Count > 0)
            {
                labelFechaInicial.Text = listaFechas[0].Date.ToString();
                labelFechaFinal.Text = listaFechas[listaFechas.Count - 1].Date.ToString();
            }
        }

        private void ReiniciarGrafico()
        {
            Graphics papel = pictureBoxGraficaDatos.CreateGraphics();
            graficador.BorrarGrafico(pictureBoxGraficaDatos, papel);

            trackBarUsarDatosDesde.Width = pictureBoxGraficaDatos.Width;
            trackBarUsarDatosDesde.Value = 0;

            labelFechaFinal.Location = new System.Drawing.Point(trackBarUsarDatosDesde.Location.X + trackBarUsarDatosDesde.Width - labelFechaFinal.Width, tabPageDatos.Height - margenVisual - labelFechaFinal.Height);

            labelFechaFinal.Text = "Fecha final";
            labelFechaInicial.Text = "Fecha inicial";
            labelProduccionMaxima.Text = "Producción";
        }

        protected void ContinuarConSerieComprobada(bool serieComprobada)
        {
            if (serieComprobada && listaFechas.Count > 0)
            {
                labelTipoDeAjuste.Enabled = true;
                labelUsarDatosDesde.Enabled = true;
                comboBoxTipoDeAjuste.Enabled = true;
                comboBoxUsarDatosDesde.Enabled = true;
                labelUsarDatosHasta.Enabled = true;
                comboBoxUsarDatosHasta.Enabled = true;
                buttonGraficarDatos.Enabled = true;

                usarDatosHasta = listaDatos.Count - 1;

                double[] vectorLocalDatosAjustados = new double[listaDatos.Count];
                for (int a = 0; a < listaDatos.Count; a++)
                {
                    vectorLocalDatosAjustados[a] = listaDatos[a];
                }
                vectorDatosAjustados = vectorLocalDatosAjustados;

                trackBarUsarDatosDesde.Minimum = 0;
                trackBarUsarDatosDesde.Maximum = vectorDatosAjustados.Length - 2;

                GraficarTodo(vectorDatosAjustados, usarDatosDesde, usarDatosHasta);

                funcionDeAutocorrelaciónSimpleToolStripMenuItem.Enabled = true;
                funciónDeAutocorrelaciónParcialToolStripMenuItem.Enabled = true;

                PasarDatosAR(vectorDatosAjustados, datosParaR, usarDatosDesde, usarDatosHasta);

                //Cosas de la tabPageModelo:

                panelResultados.Enabled = true;
                tabControlModeloAUsar.Enabled = true;
            }
            else
            {
                labelTipoDeAjuste.Enabled = false;
                labelUsarDatosDesde.Enabled = false;
                comboBoxTipoDeAjuste.Enabled = false;
                comboBoxUsarDatosDesde.Enabled = false;
                labelUsarDatosHasta.Enabled = false;
                comboBoxUsarDatosHasta.Enabled = false;
                trackBarUsarDatosDesde.Enabled = false;
                buttonGraficarDatos.Enabled = false;

                vectorDatosAjustados = null;

                comboBoxTipoDeAjuste.SelectedIndex = 0;
                comboBoxUsarDatosDesde.SelectedIndex = 0;
                comboBoxUsarDatosHasta.SelectedIndex = 0;
                usarDatosDesde = 0;
                usarDatosHasta = 0;

                ReiniciarGrafico();

                textBoxUsarDatosHasta.Clear();
                textBoxUsarDatosDesde.Clear();
                labelEspecificarValorDeJ.Enabled = false;
                textBoxUsarDatosHasta.Enabled = false;
                textBoxUsarDatosDesde.Enabled = false;
                buttonUsarDatosDesde.Enabled = false;
                buttonUsarDatosDesde.Text = "Fecha primer dato";
                buttonUsarDatosHasta.Enabled = false;
                buttonUsarDatosHasta.Text = "Fecha último dato";

                funcionDeAutocorrelaciónSimpleToolStripMenuItem.Enabled = false;
                funciónDeAutocorrelaciónParcialToolStripMenuItem.Enabled = false;

                //Cosas de la tabPageModelo:
                ReiniciarPanelesDeResultados();
                dataGridViewDatos.RowCount = 0;
                dataGridViewPronosticoDatos.RowCount = 0;
                Graphics papelPronostico = pictureBoxPronosticoDatos.CreateGraphics();
                graficador.BorrarGrafico(pictureBoxPronosticoDatos, papelPronostico);
                Graphics papelModelo = pictureBoxModelo.CreateGraphics();
                graficador.BorrarGrafico(pictureBoxModelo, papelModelo);


                panelResultados.Enabled = false;
                tabControlModeloAUsar.Enabled = false;
                numeroDeModelosUsados = 0;
                verInformaciónDeModelosUsadosToolStripMenuItem.Enabled = false;
            }
        }

        protected void PasarDatosAR(double[] vectorDatosAjustados, RDotNet.NumericVector datosParaR, int usarDatosDesde, int usarDatosHasta)
        {
            if (vectorDatosAjustados != null && vectorDatosAjustados.Length > 0)
            {
                NumericVector datosAjustadosParaRLocal = motorDeR.CreateNumericVector(usarDatosHasta - usarDatosDesde + 1);
                for (int i = 0; i < usarDatosHasta - usarDatosDesde + 1; i++)
                {
                    datosAjustadosParaRLocal[i] = vectorDatosAjustados[i + usarDatosDesde];
                }
                datosParaR = datosAjustadosParaRLocal;
                motorDeR.SetSymbol(nombreDelVectorDeDatosEnR, datosParaR);
            }
            
        }

        private void buttonGraficarDatos_Click(object sender, EventArgs e)
        {
            if (serieComprobada)
            {
                GraficarTodo(vectorDatosAjustados, usarDatosDesde, usarDatosHasta);
            }
        }


        protected void seleccionadorDeFecha_fechaSeleccionada(object sender, FechaSeleccionadaEvenArgs e)
        {
            fechaTemporal = e.fecha;
        }

        #endregion


        #region tabPageModelo

        //Los paneles de controles y resultados de la tabPageModelo se habilitan arriba en el metodo ContinuarConSerieComprobada, para evitar errores por manipulacion del usuario


        ConstructorDeModelos constructorDeModelos = new ConstructorDeModelos();
        double[] vectorValoresDelModelo;
        double[] vectorAUsar;
        double r2, aic, declinacionInicial, n, valorP, produccionInicial;
        double escalaParaTrackBarNEspecificado = 100;

        string modeloUsado;
        int arimaP, arimaD, arimaQ;
        NumericVector coeficientesArima;



        private void comboBoxHipVariarNHasta_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelHipHastaN.Text = comboBoxHipVariarNHasta.Text;
            trackBarHipNEspecificado.Maximum = Convert.ToInt32(Convert.ToDouble(comboBoxHipVariarNHasta.Text) * escalaParaTrackBarNEspecificado);
        }

        private void trackBarHipNEspecificado_Scroll(object sender, EventArgs e)
        {
            n = trackBarHipNEspecificado.Value / escalaParaTrackBarNEspecificado;
            buttonCalcularModeloHiperbolico_Click(sender, e);

        }

        private void buttonArimaFAS_Click(object sender, EventArgs e)
        {
            motorDeR.Evaluate("acf(" + nombreDelVectorDeDatosEnR + ")");
        }

        private void buttonArimaFAP_Click(object sender, EventArgs e)
        {
            motorDeR.Evaluate("pacf(" + nombreDelVectorDeDatosEnR + ")");
        }

        private void comboBoxHipVariarNDesde_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelHipDesdeN.Text = comboBoxHipVariarNDesde.Text;
            trackBarHipNEspecificado.Minimum = Convert.ToInt32(Convert.ToDouble(comboBoxHipVariarNDesde.Text) * escalaParaTrackBarNEspecificado);
            trackBarHipNEspecificado.Value = trackBarHipNEspecificado.Minimum;
            n = Convert.ToDouble(comboBoxHipVariarNDesde.SelectedIndex.ToString());
        }



        //Con los siguientes metodos se habilita la copia de los datos de los elementos del panel de resultados de la tabPageModelo al portapapeles:

        private void dataGridViewResultadoCoeficientesArima_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                DataObject d = dataGridViewResultadoCoeficientesArima.GetClipboardContent();
                Clipboard.SetDataObject(d);
            }
        }

        private void textBoxResultadoModeloUsado_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                Clipboard.SetText(textBoxResultadoModeloUsado.Text);
            }
        }

        private void textBoxResultadoProduccionInicial_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                Clipboard.SetText(textBoxResultadoProduccionInicial.Text);
            }
        }

        private void textBoxResultadoDi_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                Clipboard.SetText(textBoxResultadoDi.Text);
            }
        }

        private void textBoxResultadoExponenteN_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                Clipboard.SetText(textBoxResultadoExponenteN.Text);
            }
        }

        private void textBoxResultadoRCuadrado_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                Clipboard.SetText(textBoxResultadoRCuadrado.Text);
            }
        }

        private void textBoxResultadoAIC_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                Clipboard.SetText(textBoxResultadoAIC.Text);
            }
        }

        private void textBoxResultadoValorP_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                Clipboard.SetText(textBoxResultadoValorP.Text);
            }
        }



        private void radioButtonArimaOrdenEspecifico_CheckedChanged(object sender, EventArgs e)
        {
            panelPronostico.Enabled = false;
            panelPronosticoResultados.Enabled = true;

            if(radioButtonArimaOrdenEspecifico.Checked)
            {
                labelArimaD.Enabled = true;
                labelArimaP.Enabled = true;
                labelArimaQ.Enabled = true;
                textBoxArimaD.Enabled = true;
                textBoxArimaP.Enabled = true;
                textBoxArimaQ.Enabled = true;

                textBoxArimaD.Text = "1";
                textBoxArimaP.Text = "1";
                textBoxArimaQ.Text = "1";
            }
            else
            {
                labelArimaD.Enabled = false;
                labelArimaP.Enabled = false;
                labelArimaQ.Enabled = false;
                textBoxArimaD.Enabled = false;
                textBoxArimaP.Enabled = false;
                textBoxArimaQ.Enabled = false;

                textBoxArimaD.Text = "1";
                textBoxArimaP.Text = "1";
                textBoxArimaQ.Text = "1";
            }
            checkBoxHipPermitirNAtipico.Enabled = true;
            checkBoxHipPermitirNAtipico.Checked = false;

        }

        private void radioButtonHipEspecificandoDiYN_CheckedChanged(object sender, EventArgs e)//Esta opcion es la unica que permite al usuario introducir la declinacion inicial
        {
            if(radioButtonHipEspecificandoDiYN.Checked)
            {
                labelHipDiEspecificada.Enabled = true;
                textBoxHipDiEspecificada.Enabled = true;
            }
            else
            {
                labelHipDiEspecificada.Enabled = false;
                textBoxHipDiEspecificada.Enabled = false;
            }
            textBoxHipDiEspecificada.Text = "";
            checkBoxHipPermitirNAtipico.Enabled = true;
            checkBoxHipPermitirNAtipico.Checked = false;
        }

        private void radioButtonHipMejorModelo_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonHipMejorModelo.Checked)
            {
                labelHipNumeroDeIteraciones.Enabled = true;
                comboBoxHipNumeroDeIteraciones.Enabled = true;
                comboBoxHipNumeroDeIteraciones.SelectedIndex = 0;

                checkBoxHipPermitirNAtipico.Checked = false;
                checkBoxHipPermitirNAtipico.Enabled = false;

                labelHipDiEspecificada.Enabled = false;
                labelHipNEspecificado.Enabled = false;
                labelHipDesdeN.Enabled = false;
                labelHipHastaN.Enabled = false;
                textBoxHipDiEspecificada.Enabled = false;
                textBoxHipDiEspecificada.Text = "";
                trackBarHipNEspecificado.Enabled = false;


                trackBarHipNEspecificado.Value = trackBarHipNEspecificado.Minimum;

            }
            else
            {
                labelHipNumeroDeIteraciones.Enabled = false;
                comboBoxHipNumeroDeIteraciones.Enabled = false;
                comboBoxHipNumeroDeIteraciones.SelectedIndex = 0;

                if (radioButtonHipEspecificandoDiYN.Checked)
                {
                    labelHipDiEspecificada.Enabled = true;
                    textBoxHipDiEspecificada.Enabled = true;
                }
                else
                {
                    labelHipDiEspecificada.Enabled = false;
                    textBoxHipDiEspecificada.Enabled = false;
                }

                checkBoxHipPermitirNAtipico.Enabled = true;
                checkBoxHipPermitirNAtipico.Checked = false;

                textBoxHipDiEspecificada.Text = "";
                labelHipNEspecificado.Enabled = true;
                labelHipDesdeN.Enabled = true;
                labelHipHastaN.Enabled = true;
                
                trackBarHipNEspecificado.Enabled = true;


                trackBarHipNEspecificado.Value = trackBarHipNEspecificado.Minimum;
            }
        }

        private void checkBoxHipPermitirNAtipico_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHipPermitirNAtipico.Checked)
            {
                labelHipVariarNDesde.Enabled = true;
                labelHipVariarNHasta.Enabled = true;

                comboBoxHipVariarNDesde.Enabled = true;
                comboBoxHipVariarNHasta.Enabled = true;
                comboBoxHipVariarNDesde.SelectedIndex = 0;
                comboBoxHipVariarNHasta.SelectedIndex = 0;
            }
            else
            {
                labelHipVariarNDesde.Enabled = false;
                labelHipVariarNHasta.Enabled = false;
                comboBoxHipVariarNDesde.Enabled = false;
                comboBoxHipVariarNHasta.Enabled = false;
                comboBoxHipVariarNDesde.SelectedIndex = 0;
                comboBoxHipVariarNHasta.SelectedIndex = 0;
            }
        }
        
        private void DesplegarCoeficientesArima(int p, int q, NumericVector coeficientes, DataGridView dataGridViewCoeficientes)
        {
            dataGridViewCoeficientes.ColumnCount = 2;
            dataGridViewCoeficientes.RowCount = p + q;
            dataGridViewCoeficientes.Columns[0].Name = "Coeficiente";
            dataGridViewCoeficientes.Columns[0].Name = "Valor";
            int numeroDeCoeficiente;

            for(int i = 0; i < (p + q); i++)
            {
                if(i < p)
                {
                    numeroDeCoeficiente = i + 1;
                    dataGridViewCoeficientes[0, i].Value = "AR " + numeroDeCoeficiente;
                }
                else
                {
                    numeroDeCoeficiente = i - p + 1;
                    dataGridViewCoeficientes[0, i].Value = "MA " + numeroDeCoeficiente;
                }

                dataGridViewCoeficientes[1, i].Value = coeficientes[i].ToString();
                
            }
        }



        private void HabilitarCuadrosDeResultados(string modeloUsado)//Habilita los cuadros del panel de resultados tanto de la tabPageModelo como de la tabPagePronostico
        {
            //Se habilitan los paneles de la tabPagePronostico

            panelPronostico.Enabled = true;
            panelPronosticoResultados.Enabled = true;

            string modeloParaCondicion = modeloUsado.Substring(0, 5);
            if(modeloParaCondicion == "Combi")
            {
                //De la tabPageModelo
                labelModeloTablaSoloParaArima.Visible = true;

                labelResultadoDi.Enabled = true;
                textBoxResultadoDi.Enabled = true;
                textBoxResultadoDi.Text = "";

                labelResultadoExponenteN.Enabled = true;
                textBoxResultadoExponenteN.Enabled = true;
                textBoxResultadoExponenteN.Text = "";

                labelResultadoValorP.Enabled = true;
                textBoxResultadoValorP.Enabled = true;
                textBoxResultadoValorP.Text = "";

                labelResultadoAIC.Enabled = true;
                textBoxResultadoAIC.Enabled = true;
                textBoxResultadoAIC.Text = "";

                dataGridViewResultadoCoeficientesArima.Enabled = true;
                dataGridViewResultadoCoeficientesArima.RowCount = 0;

                //De la tabPagePronostico
                labelPronosticoTablaSoloParaArima.Visible = true;

                labelPronosticoDi.Enabled = true;
                textBoxPronosticoDi.Enabled = true;
                textBoxPronosticoDi.Text = "";

                labelPronosticoExponenteN.Enabled = true;
                textBoxPronosticoExponenteN.Enabled = true;
                textBoxPronosticoExponenteN.Text = "";

                labelPronosticoValorP.Enabled = true;
                textBoxPronosticoValorP.Enabled = true;
                textBoxPronosticoValorP.Text = "";

                labelPronosticoAIC.Enabled = true;
                textBoxPronosticoAIC.Enabled = true;
                textBoxPronosticoAIC.Text = "";

                dataGridViewPronosticoCoeficientesArima.Enabled = true;
                dataGridViewPronosticoCoeficientesArima.RowCount = 0;
            }
            else
            {
                if (modeloParaCondicion == "ARIMA")
                {
                    //De la tabPageModelo
                    labelModeloTablaSoloParaArima.Visible = false;

                    labelResultadoDi.Enabled = false;
                    textBoxResultadoDi.Enabled = false;
                    textBoxResultadoDi.Text = "";

                    labelResultadoExponenteN.Enabled = false;
                    textBoxResultadoExponenteN.Enabled = false;
                    textBoxResultadoExponenteN.Text = "";

                    labelResultadoValorP.Enabled = true;
                    textBoxResultadoValorP.Enabled = true;
                    textBoxResultadoValorP.Text = "";

                    labelResultadoAIC.Enabled = true;
                    textBoxResultadoAIC.Enabled = true;
                    textBoxResultadoAIC.Text = "";

                    dataGridViewResultadoCoeficientesArima.Enabled = true;
                    dataGridViewResultadoCoeficientesArima.RowCount = 0;

                    //De la tabPagePronostico
                    labelPronosticoTablaSoloParaArima.Visible = false;

                    labelPronosticoDi.Enabled = false;
                    textBoxPronosticoDi.Enabled = false;
                    textBoxPronosticoDi.Text = "";

                    labelPronosticoExponenteN.Enabled = false;
                    textBoxPronosticoExponenteN.Enabled = false;
                    textBoxPronosticoExponenteN.Text = "";

                    labelPronosticoValorP.Enabled = true;
                    textBoxPronosticoValorP.Enabled = true;
                    textBoxPronosticoValorP.Text = "";

                    labelPronosticoAIC.Enabled = true;
                    textBoxPronosticoAIC.Enabled = true;
                    textBoxPronosticoAIC.Text = "";

                    dataGridViewPronosticoCoeficientesArima.Enabled = true;
                    dataGridViewPronosticoCoeficientesArima.RowCount = 0;
                }
                else
                {
                    //De la tabPageModelo
                    labelModeloTablaSoloParaArima.Visible = true;

                    labelResultadoDi.Enabled = true;
                    textBoxResultadoDi.Enabled = true;
                    textBoxResultadoDi.Text = "";

                    labelResultadoExponenteN.Enabled = true;
                    textBoxResultadoExponenteN.Enabled = true;
                    textBoxResultadoExponenteN.Text = "";

                    labelResultadoValorP.Enabled = false;
                    textBoxResultadoValorP.Enabled = false;
                    textBoxResultadoValorP.Text = "";

                    labelResultadoAIC.Enabled = false;
                    textBoxResultadoAIC.Enabled = false;
                    textBoxResultadoAIC.Text = "";

                    dataGridViewResultadoCoeficientesArima.Enabled = false;
                    dataGridViewResultadoCoeficientesArima.RowCount = 0;

                    //De la tabPagePronostico
                    labelPronosticoTablaSoloParaArima.Visible = true;

                    labelPronosticoDi.Enabled = true;
                    textBoxPronosticoDi.Enabled = true;
                    textBoxPronosticoDi.Text = "";

                    labelPronosticoExponenteN.Enabled = true;
                    textBoxPronosticoExponenteN.Enabled = true;
                    textBoxPronosticoExponenteN.Text = "";

                    labelPronosticoValorP.Enabled = false;
                    textBoxPronosticoValorP.Enabled = false;
                    textBoxPronosticoValorP.Text = "";

                    labelPronosticoAIC.Enabled = false;
                    textBoxPronosticoAIC.Enabled = false;
                    textBoxPronosticoAIC.Text = "";

                    dataGridViewPronosticoCoeficientesArima.Enabled = false;
                    dataGridViewPronosticoCoeficientesArima.RowCount = 0;
                }
            }
            
        }


        
        private void GraficarDatosYModelo(double[] vectorLocalAUsar,double[] vectorValoresDelModelo, double datoMaximo)
        {
            Graphics papelParaModelo = pictureBoxModelo.CreateGraphics();
            Pen lapizParaModelo = new Pen(Color.Black);

            //La variable numeroDeDatos equivale a la cantidad de observaciones que se tienen
            //La variable cantidadDeDatosGraficados hace referencia al numero de datos que se puede graficar (ya que el numero de datos que se puede graficar está limitado al numero de pixeles disponible)
            int cantidadDeDatosGraficados = 0, numeroDeDatos = vectorLocalAUsar.Length;

            graficador.BorrarGrafico(pictureBoxModelo, papelParaModelo); // Limpia el picture box para poder graficar.

            lapizParaModelo.Color = Color.Black;
            graficador.GraficarDatos(pictureBoxModelo, papelParaModelo, lapizParaModelo, vectorLocalAUsar, numeroDeDatos, ref cantidadDeDatosGraficados, datoMaximo);


            lapizParaModelo.Color = Color.Red;
            lapizParaModelo.Width = 2;
            graficador.GraficarDatos(pictureBoxModelo, papelParaModelo, lapizParaModelo, vectorValoresDelModelo, numeroDeDatos, ref cantidadDeDatosGraficados, datoMaximo);

            if (cantidadDeDatosGraficados > 0)//Define la posicion de la etiqueta de fecha final
            {
                if (vectorDatosAjustados.Length > pictureBoxModelo.Width)
                {
                    labelModeloFechaFinal.Location = new System.Drawing.Point(pictureBoxModelo.Location.X + cantidadDeDatosGraficados - 1 - labelFechaFinal.Width / 2, pictureBoxModelo.Location.Y + pictureBoxModelo.Height + 3);

                }
                else
                {
                    int a = (vectorDatosAjustados.Length - 1) * Convert.ToInt32(pictureBoxGraficaDatos.Width / vectorDatosAjustados.Length);
                    labelModeloFechaFinal.Location = new System.Drawing.Point(pictureBoxModelo.Location.X + a - labelFechaFinal.Width, pictureBoxModelo.Location.Y + pictureBoxModelo.Height + 3);
                }
            }

            //Definir texto de las etiquetas de la grafica

            if (datoMaximo > 0)
            {
                labelModeloProduccionMaxima.Text = Convert.ToString(Convert.ToInt32(datoMaximo));
            }
            if (listaFechas.Count > 0)
            {
                labelModeloFechaInicial.Text = listaFechas[usarDatosDesde].Date.ToString();
                labelModeloFechaFinal.Text = listaFechas[usarDatosHasta].Date.ToString();
            }
        }

        private void tabPageModelo_SizeChanged(object sender, EventArgs e)
        {
            pictureBoxModelo.Width = tabPageModelo.Width - pictureBoxModelo.Location.X - margenVisual;
            pictureBoxModelo.Height = tabPageModelo.Height - pictureBoxModelo.Location.Y - margenVisual;

            labelModeloProduccionCero.Location = new System.Drawing.Point(80, pictureBoxModelo.Location.Y + pictureBoxModelo.Height - labelModeloProduccionCero.Height);
            labelModeloFechaInicial.Location = new System.Drawing.Point(103, pictureBoxModelo.Location.Y + pictureBoxModelo.Height + 3);
            labelModeloFechaFinal.Location = new System.Drawing.Point(pictureBoxModelo.Location.X + pictureBoxModelo.Width - labelModeloFechaFinal.Width, pictureBoxModelo.Location.Y + pictureBoxModelo.Height + 3);

            if(tabPageModelo.Width - panelResultados.Location.X - margenVisual < 685)
            {
                panelResultados.Width = tabPageModelo.Width - panelResultados.Location.X - margenVisual;
            }
            else
            {
                panelResultados.Width = 685;
            }
        }

        private void buttonCalcularExponencial_Click(object sender, EventArgs e)
        {
            
            double datoMaximo;
            int posicionMaximo = 0;
            manejadorDeDatos.EncontrarMaximo(vectorDatosAjustados, out datoMaximo, out posicionMaximo);
            produccionInicial = vectorDatosAjustados[usarDatosDesde];
            n = 0;

            double[] vectorLocalAUsar = new double[usarDatosHasta - usarDatosDesde + 1];
            for(int i = 0; i< vectorLocalAUsar.Length;i++)
            {
                vectorLocalAUsar[i] = vectorDatosAjustados[usarDatosDesde + i];
            }
            

            if (radioButtonExpMejorAjuste.Checked) //Elije como hacer el modelo Exponencial.
            {
                constructorDeModelos.ConstruirMejorModeloExponencial(motorDeR, nombreDelVectorDeDatosEnR, datosParaR, vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, out declinacionInicial, ref produccionInicial,ref r2, ref aic);
            }
            else
            {
                if (radioButtonExpEspecificarValorDeDi.Checked)
                {
                    if(textBoxExpDeclinacionInicial.Text.Trim() != "")
                    {
                        try
                        {
                            declinacionInicial = Convert.ToDouble(textBoxExpDeclinacionInicial.Text);
                            textBoxExpDeclinacionInicial.Text = declinacionInicial.ToString();
                            constructorDeModelos.ConstruirModeloExponencialEspecificandoDi(vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, declinacionInicial, ref r2, ref aic);
                        }
                        catch
                        {
                            MessageBox.Show("Dato de declinación inicial invalido. se construirá el modelo que pase por puntos inicial y final.", "Declinación inicial invalida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBoxExpDeclinacionInicial.Text = "";
                            radioButtonExpUltimoPunto.Checked = true;
                            constructorDeModelos.ConstruirModeloExponencialQuePasePorElUltimoPunto(vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, out declinacionInicial, ref r2, ref aic);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No ha proporcionado un valor de declinación inicial. se construirá el modelo que pase por puntos inicial y final.", "Declinación inicial invalida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBoxExpDeclinacionInicial.Text = "";
                        radioButtonExpUltimoPunto.Checked = true;
                        constructorDeModelos.ConstruirModeloExponencialQuePasePorElUltimoPunto(vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, out declinacionInicial, ref r2, ref aic);
                    }
                }
                else
                {
                    constructorDeModelos.ConstruirModeloExponencialQuePasePorElUltimoPunto(vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, out declinacionInicial, ref r2, ref aic);
                }
            }
            
            modeloUsado = "Exponencial de Arps";
            HabilitarCuadrosDeResultados(modeloUsado);

            //Desplegar resultados del modelo exponencial

            textBoxResultadoProduccionInicial.Text = produccionInicial.ToString();
            textBoxResultadoDi.Text = declinacionInicial.ToString();
            textBoxResultadoRCuadrado.Text = r2.ToString();
            textBoxResultadoExponenteN.Text = n.ToString();

            textBoxResultadoModeloUsado.Text = modeloUsado;

            CopiarResultadosDeLaTabPageModeloALaTabPageResultado(); //Despliega los datos en la tabPagePronostico

            //Graficar los datos
            if (vectorLocalAUsar.Length == vectorValoresDelModelo.Length)
            {
                GraficarDatosYModelo(vectorLocalAUsar, vectorValoresDelModelo, datoMaximo);
            }

            //Guardar los datos de los modelos usados
            
            verInformaciónDeModelosUsadosToolStripMenuItem.Enabled = true; //Habilita la opcion del menu que permite ver la informacion de los modelos usados

            numeroDeModelosUsados++;
            ActualizarVectorModelosUsados(ref vectorModelosUsados, numeroDeModelosUsados, modeloUsado);
            ActualizarVectorDeParametros(ref vectorR2DeModelosUsados, numeroDeModelosUsados, r2.ToString());
            ActualizarVectorDeParametros(ref vectorAicDeModelosUsados, numeroDeModelosUsados, "");
            ActualizarVectorDeParametros(ref vectorNDeModelosUsados, numeroDeModelosUsados, n.ToString());
            ActualizarVectorDeParametros(ref vectorDiDeModelosUsados, numeroDeModelosUsados, declinacionInicial.ToString());
            ActualizarVectorDeParametros(ref vectorProduccionInicialDeModelosUsados, numeroDeModelosUsados, produccionInicial.ToString());
            ActualizarVectorDeParametros(ref vectorFechaDatoInicialDeModelosUsados, numeroDeModelosUsados, listaFechas[usarDatosDesde].Date.ToString().Substring(0, longitudCadenaDeFecha));
            ActualizarVectorDeParametros(ref vectorFechaDatoFinalDeModelosUsados, numeroDeModelosUsados, listaFechas[usarDatosHasta].Date.ToString().Substring(0, longitudCadenaDeFecha));
        }

        private void buttonCalcularArmonico_Click(object sender, EventArgs e)
        {

            double datoMaximo;
            int posicionMaximo = 0;
            manejadorDeDatos.EncontrarMaximo(vectorDatosAjustados, out datoMaximo, out posicionMaximo);
            produccionInicial = vectorDatosAjustados[usarDatosDesde];
            n = 1;

            double[] vectorLocalAUsar = new double[usarDatosHasta - usarDatosDesde + 1];
            for (int i = 0; i < vectorLocalAUsar.Length; i++)
            {
                vectorLocalAUsar[i] = vectorDatosAjustados[usarDatosDesde + i];
            }


            if (radioButtonArmMejorAjuste.Checked) //Elije como hacer el modelo Exponencial.
            {
                constructorDeModelos.ConstruirMejorModeloArmonico(ref motorDeR, nombreDelVectorDeDatosEnR, datosParaR, vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, out declinacionInicial, ref produccionInicial, ref r2, ref aic);
            }
            else
            {
                if (radioButtonArmEspecificarDi.Checked)
                {
                    if (textBoxArmDeclinacionInicial.Text.Trim() != "")
                    {
                        try
                        {
                            declinacionInicial = Convert.ToDouble(textBoxArmDeclinacionInicial.Text);
                            textBoxArmDeclinacionInicial.Text = declinacionInicial.ToString();
                            constructorDeModelos.ConstruirModeloArmonicoEspecificandoDi(vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, declinacionInicial, ref r2, ref aic);
                        }
                        catch
                        {
                            MessageBox.Show("Dato de declinación inicial invalido. se construirá el modelo que pase por puntos inicial y final.", "Declinación inicial invalida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBoxArmDeclinacionInicial.Text = "";
                            radioButtonArmPuntoFinal.Checked = true;
                            constructorDeModelos.ConstruirModeloArmonicoQuePasePorElUltimoPunto(vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, out declinacionInicial, ref r2, ref aic);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No ha proporcionado un valor de declinación inicial. se construirá el modelo que pase por puntos inicial y final.", "Declinación inicial invalida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBoxArmDeclinacionInicial.Text = "";
                        radioButtonArmPuntoFinal.Checked = true;
                        constructorDeModelos.ConstruirModeloArmonicoQuePasePorElUltimoPunto(vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, out declinacionInicial, ref r2, ref aic);

                    }
                }
                else
                {
                    constructorDeModelos.ConstruirModeloArmonicoQuePasePorElUltimoPunto(vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, out declinacionInicial, ref r2, ref aic);
                }
            }

            modeloUsado = "Armónico de Arps";
            HabilitarCuadrosDeResultados(modeloUsado);

            //Desplegar resultados del modelo armonico
            
            textBoxResultadoProduccionInicial.Text = produccionInicial.ToString();
            textBoxResultadoDi.Text = declinacionInicial.ToString();
            textBoxResultadoRCuadrado.Text = r2.ToString();
            textBoxResultadoExponenteN.Text = n.ToString();

            textBoxResultadoModeloUsado.Text = modeloUsado;

            CopiarResultadosDeLaTabPageModeloALaTabPageResultado(); //Despliega los datos en la tabPagePronostico

            //Graficar los datos
            if (vectorLocalAUsar.Length == vectorValoresDelModelo.Length)
            {
                GraficarDatosYModelo(vectorLocalAUsar, vectorValoresDelModelo, datoMaximo);
            }

            //Guardar los datos de los modelos usados

            verInformaciónDeModelosUsadosToolStripMenuItem.Enabled = true; //Habilita la opcion del menu que permite ver la informacion de los modelos usados

            numeroDeModelosUsados++;
            ActualizarVectorModelosUsados(ref vectorModelosUsados, numeroDeModelosUsados, modeloUsado);
            ActualizarVectorDeParametros(ref vectorR2DeModelosUsados, numeroDeModelosUsados, r2.ToString());
            ActualizarVectorDeParametros(ref vectorAicDeModelosUsados, numeroDeModelosUsados, "");
            ActualizarVectorDeParametros(ref vectorNDeModelosUsados, numeroDeModelosUsados, n.ToString());
            ActualizarVectorDeParametros(ref vectorDiDeModelosUsados, numeroDeModelosUsados, declinacionInicial.ToString());
            ActualizarVectorDeParametros(ref vectorProduccionInicialDeModelosUsados, numeroDeModelosUsados, produccionInicial.ToString());
            ActualizarVectorDeParametros(ref vectorFechaDatoInicialDeModelosUsados, numeroDeModelosUsados, listaFechas[usarDatosDesde].Date.ToString().Substring(0, longitudCadenaDeFecha));
            ActualizarVectorDeParametros(ref vectorFechaDatoFinalDeModelosUsados, numeroDeModelosUsados, listaFechas[usarDatosHasta].Date.ToString().Substring(0, longitudCadenaDeFecha));
        }

        private void buttonCalcularModeloHiperbolico_Click(object sender, EventArgs e)
        {

            double datoMaximo;
            int posicionMaximo = 0;
            manejadorDeDatos.EncontrarMaximo(vectorDatosAjustados, out datoMaximo, out posicionMaximo);
            produccionInicial = vectorDatosAjustados[usarDatosDesde];
            int numeroDeIteraciones;

            double[] vectorLocalAUsar = new double[usarDatosHasta - usarDatosDesde + 1];

            try
            {

                for (int i = 0; i < vectorLocalAUsar.Length; i++)
                {
                    vectorLocalAUsar[i] = vectorDatosAjustados[usarDatosDesde + i];
                }

                if (radioButtonHipMejorModelo.Checked)
                {
                    try
                    {
                        double variarNDesde = 0, variarNHasta = 1;
                        if (checkBoxHipPermitirNAtipico.Checked)
                        {
                            variarNDesde = Convert.ToDouble(comboBoxHipVariarNDesde.SelectedItem);
                            variarNHasta = Convert.ToDouble(comboBoxHipVariarNHasta.SelectedItem);
                        }
                        numeroDeIteraciones = Convert.ToInt32(comboBoxHipNumeroDeIteraciones.SelectedItem.ToString());
                        constructorDeModelos.ConstruirModeloHiperbolicoAutomatico(vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, numeroDeIteraciones, out declinacionInicial, out n, out r2, out aic);
                    }
                    catch(Exception ex)
                    {
                        throw new Exception("Imposible construir modelo hiperbólico con los datos especificados", ex);
                    }
                }
                if (radioButtonHipUltimoPunto.Checked)
                {
                    constructorDeModelos.ConstruirModeloHiperbolicoQuePasePorElUltimoPunto(vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, n, out declinacionInicial, out r2, out aic);
                }
                if (radioButtonHipEspecificandoDiYN.Checked)
                {
                    textBoxHipDiEspecificada.Text = textBoxHipDiEspecificada.Text.Trim();
                    if (textBoxHipDiEspecificada.Text != "")
                    {
                        try
                        {
                            declinacionInicial = Convert.ToDouble(textBoxHipDiEspecificada.Text);
                            textBoxHipDiEspecificada.Text = declinacionInicial.ToString();
                            if (declinacionInicial >= 0)
                            {
                                constructorDeModelos.ConstruirModeloHiperbolicoDandoDiYN(vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, n, declinacionInicial, out r2, out aic);
                            }
                            else
                            {
                                MessageBox.Show("El valor especificado para declinación inicial debe ser un número positivo entre 0 y 1\nRecuerde que el separador decimal es: \"" + separadorDecimal + "\"\nSe utilizará un valor de Declinacion inicial de 0" + separadorDecimal + "001", "Error al leer la declinacion inicial", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                textBoxHipDiEspecificada.Text = "0" + separadorDecimal + "001";
                                declinacionInicial = 0.001;
                                constructorDeModelos.ConstruirModeloHiperbolicoDandoDiYN(vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, n, declinacionInicial, out r2, out aic);
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Ha ocurrido un error, asegurese de que el valor especificado para declinación inicial esté entre 0 y 1\nRecuerde que el separador decimal es: \"" + separadorDecimal + "\"\nSe utilizará un valor de Declinacion inicial de 0" + separadorDecimal + "001", "Error al realizar el modelo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBoxHipDiEspecificada.Text = "0" + separadorDecimal + "001";
                            declinacionInicial = 0.001;
                            constructorDeModelos.ConstruirModeloHiperbolicoDandoDiYN(vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, n, declinacionInicial, out r2, out aic);

                        }
                    }
                    else
                    {
                        MessageBox.Show("Por favor especifique el valor de la declinacion inicial.\nRecuerde que el separador decimal es: \"" + separadorDecimal + "\"\nSe utilizará un valor de Declinacion inicial de 0" + separadorDecimal + "001", "Declinacion inicial no especificada", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        textBoxHipDiEspecificada.Text = "0" + separadorDecimal + "001";
                        declinacionInicial = 0.001;
                        constructorDeModelos.ConstruirModeloHiperbolicoDandoDiYN(vectorDatosAjustados, out vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, n, declinacionInicial, out r2, out aic);
                    }
                }

                modeloUsado = "Hiperbólico de Arps";
                HabilitarCuadrosDeResultados(modeloUsado);

                //Desplegar resultados del modelo hiperbolico

                textBoxResultadoProduccionInicial.Text = produccionInicial.ToString();
                textBoxResultadoDi.Text = declinacionInicial.ToString();
                textBoxResultadoRCuadrado.Text = r2.ToString();

                textBoxResultadoExponenteN.Text = n.ToString();
                textBoxResultadoDi.Text = declinacionInicial.ToString();

                textBoxResultadoModeloUsado.Text = modeloUsado;

                CopiarResultadosDeLaTabPageModeloALaTabPageResultado(); //Despliega los datos en la tabPagePronostico

                //Graficar los datos
                if (vectorLocalAUsar.Length == vectorValoresDelModelo.Length)
                {
                    GraficarDatosYModelo(vectorLocalAUsar, vectorValoresDelModelo, datoMaximo);
                }

                //Guardar los datos de los modelos usados

                verInformaciónDeModelosUsadosToolStripMenuItem.Enabled = true; //Habilita la opcion del menu que permite ver la informacion de los modelos usados

                numeroDeModelosUsados++;
                ActualizarVectorModelosUsados(ref vectorModelosUsados, numeroDeModelosUsados, modeloUsado);
                ActualizarVectorDeParametros(ref vectorR2DeModelosUsados, numeroDeModelosUsados, r2.ToString());
                ActualizarVectorDeParametros(ref vectorAicDeModelosUsados, numeroDeModelosUsados, "");
                ActualizarVectorDeParametros(ref vectorNDeModelosUsados, numeroDeModelosUsados, n.ToString());
                ActualizarVectorDeParametros(ref vectorDiDeModelosUsados, numeroDeModelosUsados, declinacionInicial.ToString());
                ActualizarVectorDeParametros(ref vectorProduccionInicialDeModelosUsados, numeroDeModelosUsados, produccionInicial.ToString());
                ActualizarVectorDeParametros(ref vectorFechaDatoInicialDeModelosUsados, numeroDeModelosUsados, listaFechas[usarDatosDesde].Date.ToString().Substring(0, longitudCadenaDeFecha));
                ActualizarVectorDeParametros(ref vectorFechaDatoFinalDeModelosUsados, numeroDeModelosUsados, listaFechas[usarDatosHasta].Date.ToString().Substring(0, longitudCadenaDeFecha));
            }
            catch
            {
                MessageBox.Show("Ha ocurrido un error en la construcción del modelo. Por favor intente nuevamente", "Error al construir modelo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCalcularModeloArima_Click(object sender, EventArgs e)
        {
            tabControlModeloAUsar.Enabled = false;

            double datoMaximo;
            int posicionMaximo = 0;
            manejadorDeDatos.EncontrarMaximo(vectorDatosAjustados, out datoMaximo, out posicionMaximo);
            produccionInicial = vectorDatosAjustados[usarDatosDesde];

            double[] vectorLocalAUsar = new double[usarDatosHasta - usarDatosDesde + 1];
            for (int i = 0; i < vectorLocalAUsar.Length; i++)
            {
                vectorLocalAUsar[i] = vectorDatosAjustados[usarDatosDesde + i];
            }

            if(radioButtonArimaMejorAic.Checked)
            {
                int d = 1;
                constructorDeModelos.ConstruirArimaConMejorAic(direccionFuncionParaHallarMejorArimaConAIC, ref motorDeR, nombreDelVectorDeDatosEnR, d, out modeloUsado, datosParaR, vectorDatosAjustados, out vectorValoresDelModelo, out coeficientesArima, usarDatosDesde, usarDatosHasta, out valorP, out aic, out r2);
                arimaP = Convert.ToInt32(modeloUsado.Substring(6, 1));
                arimaD = Convert.ToInt32(modeloUsado.Substring(8, 1));
                arimaQ = Convert.ToInt32(modeloUsado.Substring(10, 1));
            }

            if(radioButtonArimaAuto.Checked)
            {
                constructorDeModelos.ConstruirMejorModeloArima(ref motorDeR, nombreDelVectorDeDatosEnR, out modeloUsado, datosParaR, vectorDatosAjustados, out vectorValoresDelModelo, out coeficientesArima, usarDatosDesde, usarDatosHasta, out valorP, out aic, out r2);
                arimaP = Convert.ToInt32(modeloUsado.Substring(6, 1));
                arimaD = Convert.ToInt32(modeloUsado.Substring(8, 1));
                arimaQ = Convert.ToInt32(modeloUsado.Substring(10, 1));
            }
            if(radioButtonArimaOrdenEspecifico.Checked)
            {
                try
                {
                    arimaD = Convert.ToInt32(textBoxArimaD.Text);
                    textBoxArimaD.Text = arimaD.ToString();
                    arimaP = Convert.ToInt32(textBoxArimaP.Text);
                    textBoxArimaP.Text = arimaP.ToString();
                    arimaQ = Convert.ToInt32(textBoxArimaQ.Text);
                    textBoxArimaQ.Text = arimaQ.ToString();

                    textBoxArimaD.Text = arimaD.ToString();
                    textBoxArimaP.Text = arimaP.ToString();
                    textBoxArimaP.Text = arimaP.ToString();

                    if (arimaD >= 0 && arimaP >= 0 && arimaQ >= 0)
                    {
                        constructorDeModelos.ConstruirModeloArimaEspecificado(ref motorDeR, nombreDelVectorDeDatosEnR, out modeloUsado, datosParaR, vectorDatosAjustados, out vectorValoresDelModelo, out coeficientesArima, usarDatosDesde, usarDatosHasta, out valorP, out aic, out r2, arimaP, arimaD, arimaQ);
                    }
                    else
                    {
                        MessageBox.Show("Cada uno de los valores proporcionados para el orden del modelo debe ser un entero positivo", "Error de orden del modelo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch
                {
                    MessageBox.Show("Ha ocurrido un error al intentar realizar el modelo. Recuerde que los valores p, d y q deben ser enteros positivos", "Error de orden del modelo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

            HabilitarCuadrosDeResultados(modeloUsado);

            //Desplegar resultados del modelo ARIMA

            textBoxResultadoProduccionInicial.Text = produccionInicial.ToString();
            textBoxResultadoRCuadrado.Text = r2.ToString();
            textBoxResultadoAIC.Text = aic.ToString();
            textBoxResultadoValorP.Text = valorP.ToString();
            textBoxResultadoModeloUsado.Text = modeloUsado;

            CopiarResultadosDeLaTabPageModeloALaTabPageResultado(); //Despliega los datos en la tabPagePronostico

            DesplegarCoeficientesArima(arimaP, arimaQ, coeficientesArima, dataGridViewResultadoCoeficientesArima); //Despliega los coeficientes en la tabPageModelo
            DesplegarCoeficientesArima(arimaP, arimaQ, coeficientesArima, dataGridViewPronosticoCoeficientesArima); //Despliega los coeficientes en la tabPageResultado

            //Graficar los datos
            buttonArimaGraficar.Enabled = true;
            if (vectorLocalAUsar.Length == vectorValoresDelModelo.Length)
            {
                GraficarDatosYModelo(vectorLocalAUsar, vectorValoresDelModelo, datoMaximo);
            }

            tabControlModeloAUsar.Enabled = true;

            //Guardar los datos de los modelos usados

            verInformaciónDeModelosUsadosToolStripMenuItem.Enabled = true; //Habilita la opcion del menu que permite ver la informacion de los modelos usados

            numeroDeModelosUsados++;
            ActualizarVectorModelosUsados(ref vectorModelosUsados, numeroDeModelosUsados, modeloUsado);
            ActualizarVectorDeParametros(ref vectorR2DeModelosUsados, numeroDeModelosUsados, r2.ToString());
            ActualizarVectorDeParametros(ref vectorAicDeModelosUsados, numeroDeModelosUsados, aic.ToString());
            ActualizarVectorDeParametros(ref vectorNDeModelosUsados, numeroDeModelosUsados, "");
            ActualizarVectorDeParametros(ref vectorDiDeModelosUsados, numeroDeModelosUsados, "");
            ActualizarVectorDeParametros(ref vectorProduccionInicialDeModelosUsados, numeroDeModelosUsados, produccionInicial.ToString());
            ActualizarVectorDeParametros(ref vectorFechaDatoInicialDeModelosUsados, numeroDeModelosUsados, listaFechas[usarDatosDesde].Date.ToString().Substring(0, longitudCadenaDeFecha));
            ActualizarVectorDeParametros(ref vectorFechaDatoFinalDeModelosUsados, numeroDeModelosUsados, listaFechas[usarDatosHasta].Date.ToString().Substring(0, longitudCadenaDeFecha));
        }

        private void buttonArimaGraficar_Click(object sender, EventArgs e)
        {
            double[] vectorLocalAUsar = new double[usarDatosHasta - usarDatosDesde + 1];
            for (int i = 0; i < vectorLocalAUsar.Length; i++)
            {
                vectorLocalAUsar[i] = vectorDatosAjustados[usarDatosDesde + i];
            }
            if (vectorLocalAUsar.Length == vectorValoresDelModelo.Length)
            {
                GraficarDatosYModelo(vectorLocalAUsar, vectorValoresDelModelo, datoMaximo);
            }
        }

        private void buttonModeloCombinadoGraficar_Click(object sender, EventArgs e)
        {
            double[] vectorLocalAUsar = new double[usarDatosHasta - usarDatosDesde + 1];
            for (int i = 0; i < vectorLocalAUsar.Length; i++)
            {
                vectorLocalAUsar[i] = vectorDatosAjustados[usarDatosDesde + i];
            }
            if (vectorLocalAUsar.Length == vectorValoresDelModelo.Length)
            {
                GraficarDatosYModelo(vectorLocalAUsar, vectorValoresDelModelo, datoMaximo);
            }

        }

        private void buttonCalcularModeloCombinado_Click(object sender, EventArgs e)
        {
            try
            {
                tabControlModeloAUsar.Enabled = false;

                string modeloArima;
                double datoMaximo;
                int posicionMaximo = 0;
                manejadorDeDatos.EncontrarMaximo(vectorDatosAjustados, out datoMaximo, out posicionMaximo);
                produccionInicial = vectorDatosAjustados[usarDatosDesde];

                double[] vectorLocalAUsar = new double[usarDatosHasta - usarDatosDesde + 1];
                for (int i = 0; i < vectorLocalAUsar.Length; i++)
                {
                    vectorLocalAUsar[i] = vectorDatosAjustados[usarDatosDesde + i];
                }
                constructorDeModelos.ConstruirModeloCombinado(ref motorDeR, out modeloArima, vectorDatosAjustados,out vectorValoresDelModelo, out coeficientesArima, usarDatosDesde, usarDatosHasta, out declinacionInicial, out n, out r2, out aic, out valorP);
                arimaP = Convert.ToInt32(modeloArima.Substring(6, 1));
                arimaD = Convert.ToInt32(modeloArima.Substring(8, 1));
                arimaQ = Convert.ToInt32(modeloArima.Substring(10, 1));

                modeloUsado = "Combinado: Hiperbólico + " + modeloArima;


                HabilitarCuadrosDeResultados(modeloUsado);

                //Desplegar resultados del modelo combinado
                
                textBoxResultadoDi.Text = declinacionInicial.ToString();
                textBoxResultadoExponenteN.Text = n.ToString();

                CopiarResultadosDeLaTabPageModeloALaTabPageResultado();
                textBoxResultadoProduccionInicial.Text = produccionInicial.ToString();
                textBoxResultadoRCuadrado.Text = r2.ToString();
                textBoxResultadoAIC.Text = aic.ToString();
                textBoxResultadoValorP.Text = valorP.ToString();
                textBoxResultadoModeloUsado.Text = modeloUsado;

                CopiarResultadosDeLaTabPageModeloALaTabPageResultado(); //Despliega los datos en la tabPagePronostico

                DesplegarCoeficientesArima(arimaP, arimaQ, coeficientesArima, dataGridViewResultadoCoeficientesArima); //Despliega los coeficientes en la tabPageModelo
                DesplegarCoeficientesArima(arimaP, arimaQ, coeficientesArima, dataGridViewPronosticoCoeficientesArima); //Despliega los coeficientes en la tabPageResultado

                //Graficar los datos
                buttonModeloCombinadoGraficar.Enabled = true;
                if (vectorLocalAUsar.Length == vectorValoresDelModelo.Length)
                {
                    GraficarDatosYModelo(vectorLocalAUsar, vectorValoresDelModelo, datoMaximo);
                }

                tabControlModeloAUsar.Enabled = true;

                //Guardar los datos de los modelos usados

                verInformaciónDeModelosUsadosToolStripMenuItem.Enabled = true; //Habilita la opcion del menu que permite ver la informacion de los modelos usados

                numeroDeModelosUsados++;
                ActualizarVectorModelosUsados(ref vectorModelosUsados, numeroDeModelosUsados, modeloUsado);
                ActualizarVectorDeParametros(ref vectorR2DeModelosUsados, numeroDeModelosUsados, r2.ToString());
                ActualizarVectorDeParametros(ref vectorAicDeModelosUsados, numeroDeModelosUsados, aic.ToString());
                ActualizarVectorDeParametros(ref vectorNDeModelosUsados, numeroDeModelosUsados, n.ToString());
                ActualizarVectorDeParametros(ref vectorDiDeModelosUsados, numeroDeModelosUsados, declinacionInicial.ToString());
                ActualizarVectorDeParametros(ref vectorProduccionInicialDeModelosUsados, numeroDeModelosUsados, produccionInicial.ToString());
                ActualizarVectorDeParametros(ref vectorFechaDatoInicialDeModelosUsados, numeroDeModelosUsados, listaFechas[usarDatosDesde].Date.ToString().Substring(0, longitudCadenaDeFecha));
                ActualizarVectorDeParametros(ref vectorFechaDatoFinalDeModelosUsados, numeroDeModelosUsados, listaFechas[usarDatosHasta].Date.ToString().Substring(0, longitudCadenaDeFecha));


            }
            catch
            {
                MessageBox.Show("Ha ocurrido un error al intentar realizar el modelo.", "Error al realizar el modelo", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }


        }


        private void CopiarResultadosDeLaTabPageModeloALaTabPageResultado()
        {
            textBoxPronosticoModeloUsado.Text = textBoxResultadoModeloUsado.Text;
            textBoxPronosticoProduccionInicial.Text = textBoxResultadoProduccionInicial.Text;
            textBoxPronosticoDi.Text = textBoxResultadoDi.Text;
            textBoxPronosticoExponenteN.Text = textBoxResultadoExponenteN.Text;
            textBoxPronosticoRCuadrado.Text = textBoxResultadoRCuadrado.Text;
            textBoxPronosticoAIC.Text = textBoxResultadoAIC.Text;
            textBoxPronosticoValorP.Text = textBoxResultadoValorP.Text;
            textBoxPronosticoFechaUltimaMedicion.Text = listaFechas[usarDatosHasta].Date.ToString().Substring(0, longitudCadenaDeFecha);
        }

        #endregion


        #region tabPagePronostico

        protected DateTime[] vectorFechasPronostico;
        protected double[] vectorDatosPronostico;
        int intervalosDeTiempoAPronosticar = 30;
        int j; //Es la cantidad de datos que el usuario quiere que vayan en su informe
        string indicadorModelo;

        //Con los siguientes metodos se habilita la copia de los datos de los elementos del panel de resultados de la tabPagePronostico al portapapeles:


        private void dataGridViewPronosticoCoeficientesArima_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                DataObject d = dataGridViewPronosticoCoeficientesArima.GetClipboardContent();
                Clipboard.SetDataObject(d);
            }
        }

        private void textBoxPronosticoModeloUsado_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                Clipboard.SetText(textBoxPronosticoModeloUsado.Text);
            }
        }

        private void textBoxPronosticoProduccionInicial_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                Clipboard.SetText(textBoxPronosticoProduccionInicial.Text);
            }
        }

        private void textBoxPronosticoDi_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                Clipboard.SetText(textBoxPronosticoDi.Text);
            }
        }

        private void textBoxPronosticoExponenteN_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                Clipboard.SetText(textBoxPronosticoExponenteN.Text);
            }
        }

        private void textBoxPronosticoRCuadrado_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                Clipboard.SetText(textBoxPronosticoRCuadrado.Text);
            }
        }

        private void textBoxPronosticoAIC_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                Clipboard.SetText(textBoxPronosticoAIC.Text);
            }
        }

        private void textBoxPronosticoValorP_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                Clipboard.SetText(textBoxPronosticoValorP.Text);
            }
        }

        private void dataGridViewPronosticoDatos_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                DataObject d = dataGridViewPronosticoDatos.GetClipboardContent();
                Clipboard.SetDataObject(d);
            }
        }

        private void verToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void radioButtonArmEspecificarDi_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButtonArmEspecificarDi.Checked)
            {
                labelArmDeclinacionInicial.Enabled = true;
                textBoxArmDeclinacionInicial.Enabled = true;
            }
            else
            {
                labelArmDeclinacionInicial.Enabled = false;
                textBoxArmDeclinacionInicial.Enabled = false;
            }
        }

        private void radioButtonExpEspecificarValorDeDi_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonExpEspecificarValorDeDi.Checked)
            {
                labelExpDeclinacionInicial.Enabled = true;
                textBoxExpDeclinacionInicial.Enabled = true;
            }
            else
            {
                labelExpDeclinacionInicial.Enabled = false;
                textBoxExpDeclinacionInicial.Enabled = false;
            }
        }

        private void tabPagePronostico_SizeChanged(object sender, EventArgs e)
        {
            panelPronosticoResultados.Width = tabPagePronostico.Width - panelPronosticoResultados.Location.X - margenVisual;

            dataGridViewPronosticoDatos.Height = tabPagePronostico.Height - dataGridViewPronosticoDatos.Location.Y - margenVisual;
            pictureBoxPronosticoDatos.Width = tabPagePronostico.Width - pictureBoxPronosticoDatos.Location.X - margenVisual;
            pictureBoxPronosticoDatos.Height = tabPagePronostico.Height - pictureBoxPronosticoDatos.Location.Y - margenVisual - labelPronosticoFechaFinal.Height - 3;

            labelPronosticoCeroProduccion.Location = new System.Drawing.Point(labelPronosticoCeroProduccion.Location.X, pictureBoxPronosticoDatos.Location.Y + pictureBoxPronosticoDatos.Height - labelPronosticoCeroProduccion.Height);
            labelPronosticoFechaInicial.Location = new System.Drawing.Point(labelPronosticoFechaInicial.Location.X, tabPagePronostico.Height - margenVisual - labelPronosticoFechaInicial.Height);
            labelPronosticoFechaFinal.Location = new System.Drawing.Point(pictureBoxPronosticoDatos.Location.X + pictureBoxPronosticoDatos.Width - labelPronosticoFechaFinal.Width, tabPagePronostico.Height - margenVisual - labelPronosticoFechaFinal.Height);

        }

        private void buttonUsarDatosDesde_Click(object sender, EventArgs e)
        {
            if (serieComprobada)
            {
                seleccionadorDeFecha seleccionadorDeFechaUsarDatosDesde = new seleccionadorDeFecha();
                seleccionadorDeFechaUsarDatosDesde.fechaSeleccionada += seleccionadorDeFecha_fechaSeleccionada;
                seleccionadorDeFechaUsarDatosDesde.ShowDialog(this);
                if (fechaTemporal.CompareTo(listaFechas[0]) >= 0 && fechaTemporal.CompareTo(listaFechas[listaFechas.Count - 1]) < 0)
                {
                    for(int i = 0; i < listaFechas.Count; i++)
                    {
                        if(fechaTemporal.CompareTo(listaFechas[i]) == 0)
                        {
                            usarDatosDesde = i;
                            break;
                        }
                    }
                    buttonUsarDatosDesde.Text = listaFechas[usarDatosDesde].Date.ToString().Substring(0, longitudCadenaDeFecha);
                    textBoxUsarDatosDesde.Text = usarDatosDesde.ToString();
                    trackBarUsarDatosDesde.Value = usarDatosDesde;
                }
                else
                {
                    MessageBox.Show("Fecha fuera del rango. Se establecerá la fecha inicial de los datos.", "Fecha fuera de rango", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    usarDatosDesde = 0;
                    buttonUsarDatosDesde.Text = listaFechas[usarDatosDesde].Date.ToString().Substring(0, longitudCadenaDeFecha);
                    textBoxUsarDatosDesde.Text = usarDatosDesde.ToString();
                    trackBarUsarDatosDesde.Value = usarDatosDesde;
                }
            }
        }

        private void buttonUsarDatosHasta_Click(object sender, EventArgs e)
        {
            if (serieComprobada)
            {
                seleccionadorDeFecha seleccionadorDeFechaUsarDatosHasta = new seleccionadorDeFecha();
                seleccionadorDeFechaUsarDatosHasta.fechaSeleccionada += seleccionadorDeFecha_fechaSeleccionada;
                seleccionadorDeFechaUsarDatosHasta.ShowDialog(this);
                if (fechaTemporal.CompareTo(listaFechas[usarDatosDesde]) >= 0 && fechaTemporal.CompareTo(listaFechas[listaFechas.Count - 1]) <= 0)
                {
                    int datoTemporal = listaFechas.Count;
                    for (int i = 0; i < listaFechas.Count; i++)
                    {
                        if (fechaTemporal.CompareTo(listaFechas[listaFechas.Count - i - 1]) == 0)
                        {
                            datoTemporal = listaFechas.Count - i - 1;
                            break;
                        }
                    }
                    textBoxUsarDatosHasta.Text = Convert.ToString(listaFechas.Count - 1 - datoTemporal);
                    buttonUsarDatosHasta.Text = listaFechas[usarDatosHasta].Date.ToString().Substring(0, longitudCadenaDeFecha);
                    usarDatosHasta = datoTemporal;
                }
                else
                {
                    MessageBox.Show("Fecha fuera del rango. Se establecerá la fecha inicial de los datos.", "Fecha fuera de rango", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    usarDatosHasta = listaFechas.Count - 1;
                    buttonUsarDatosHasta.Text = listaFechas[usarDatosHasta].Date.ToString().Substring(0, longitudCadenaDeFecha);
                    textBoxUsarDatosHasta.Text = "0";
                }
                GraficarTodo(vectorDatosAjustados, usarDatosDesde, usarDatosHasta);
                PasarDatosAR(vectorDatosAjustados, datosParaR, usarDatosDesde, usarDatosHasta);
            }
        }

        private void textBoxUsarDatosHasta_TextChanged(object sender, EventArgs e)
        {
            if (serieComprobada)
            {
                try
                {
                    if (textBoxUsarDatosHasta.Text.Length > 0 && Convert.ToInt32(textBoxUsarDatosHasta.Text) < vectorDatosAjustados.Length - usarDatosDesde && Convert.ToInt32(textBoxUsarDatosHasta.Text) >= 0)
                    {
                        usarDatosHasta = listaFechas.Count - 1 - Convert.ToInt32(textBoxUsarDatosHasta.Text);
                    }
                    else
                    {
                        usarDatosHasta = listaFechas.Count - 1;
                    }
                    buttonUsarDatosHasta.Text = listaFechas[usarDatosHasta].Date.ToString().Substring(0, longitudCadenaDeFecha);
                    textBoxUsarDatosHasta.Text = Convert.ToString(listaFechas.Count - 1 - usarDatosHasta);
                    GraficarTodo(vectorDatosAjustados, usarDatosDesde, usarDatosHasta);
                    PasarDatosAR(vectorDatosAjustados, datosParaR, usarDatosDesde, usarDatosHasta);
                }
                catch { }
            }
        }

        private void textBoxUsarDatosDesde_TextChanged(object sender, EventArgs e)
        {
            if (serieComprobada)
            {
                int valorTextBox;
                try
                {
                    try
                    {
                        valorTextBox = Convert.ToInt32(textBoxUsarDatosDesde.Text);
                    }
                    catch
                    {
                        textBoxUsarDatosDesde.Text = trackBarUsarDatosDesde.Value.ToString();
                        valorTextBox = trackBarUsarDatosDesde.Value;
                    }

                    if (valorTextBox > 0 && valorTextBox < listaFechas.Count)
                    {
                        trackBarUsarDatosDesde.Value = valorTextBox;
                        usarDatosDesde = trackBarUsarDatosDesde.Value;
                        labelUsarDatosHasta.Enabled = true;
                        buttonUsarDatosDesde.Text = listaFechas[usarDatosDesde].Date.ToString().Substring(0, longitudCadenaDeFecha);
                        comboBoxUsarDatosHasta.Enabled = true;
                        comboBoxUsarDatosHasta.SelectedIndex = 0;

                        comboBoxUsarDatosDesde.Text = listaFechas[usarDatosDesde].ToString().Substring(0, longitudCadenaDeFecha);

                        GraficarTodo(vectorDatosAjustados, usarDatosDesde, usarDatosHasta);
                        PasarDatosAR(vectorDatosAjustados, datosParaR, usarDatosDesde, usarDatosHasta);
                    }
                    else
                    {
                        textBoxUsarDatosDesde.Text = trackBarUsarDatosDesde.Value.ToString();
                    }
                }
                catch
                {
                }
            }
        }

        private void radioButtonPronosticoSolo_CheckedChanged(object sender, EventArgs e)
        {
            buttonPronosticoGraficar.Enabled = false;
            buttonPronosticoExportarAExcel.Enabled = false;
        }

        private void verMuanualDeUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(direcciónManualDeUsuario);
        }

        private void radioButtonPronosticoIncluyendoUltimosJDatos_CheckedChanged(object sender, EventArgs e)
        {
            buttonPronosticoGraficar.Enabled = false;
            buttonPronosticoExportarAExcel.Enabled = false;

            if (radioButtonPronosticoIncluyendoUltimosJDatos.Checked)
            {
                labelPronosticoEspecificarValorDeJ.Enabled = true;
                textBoxPronosticoEspecificarValorDeJ.Enabled = true;
                textBoxPronosticoEspecificarValorDeJ.Text = "1";
            }
            else
            {
                labelPronosticoEspecificarValorDeJ.Enabled = false;
                textBoxPronosticoEspecificarValorDeJ.Enabled = false;
                textBoxPronosticoEspecificarValorDeJ.Text = "";
            }
        }

        private void buttonPronosticoCalcular_Click(object sender, EventArgs e)
        {
            bool datosNegativos = false;
            int ultimoDatoPositivo = 0;
            int intervalosPronosticados = 0;
            panelPronostico.Enabled = false;

            indicadorModelo = modeloUsado.Substring(0, 3);
            DateTime primeraFechaDeDatos = listaFechas[usarDatosDesde], ultimaFechaDeDatos = listaFechas[usarDatosHasta];

            DateTime[] vectorFechasPronosticoLocal;
            double[] vectorDatosPronosticoLocal;
            
            double datoMaximo;

            int tiempoDesdeElCualSePronostica;
            
            try
            {
                intervalosDeTiempoAPronosticar = Convert.ToInt32(textBoxPronosticoIntervaloAPronosticar.Text);
                textBoxPronosticoIntervaloAPronosticar.Text = intervalosDeTiempoAPronosticar.ToString();
            }
            catch
            {
                MessageBox.Show("Debe especificar un numero entero positivo\nSeUsará un valor por defecto de 30", "Error al pronosticar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxPronosticoIntervaloAPronosticar.Text = "30";
            }

            if (intervalosDeTiempoAPronosticar >= 0)
            {
                if (radioButtonPronosticoSolo.Checked)
                {
                    tiempoDesdeElCualSePronostica = usarDatosHasta - usarDatosDesde + 1;

                    vectorFechasPronosticoLocal = new DateTime[intervalosDeTiempoAPronosticar];
                    vectorDatosPronosticoLocal = new double[intervalosDeTiempoAPronosticar];
                    

                    for (int i = 0; i < intervalosDeTiempoAPronosticar; i++)
                    {
                        if (intervaloEnMeses)
                        {
                            vectorFechasPronosticoLocal[i] = listaFechas[usarDatosHasta].AddMonths((i + 1) * intervaloDeTiempo);
                        }
                        else
                        {
                            vectorFechasPronosticoLocal[i] = listaFechas[usarDatosHasta].AddDays((i + 1) * intervaloDeTiempo);
                        }
                    }
                }
                else
                {
                    if (radioButtonPronosticoConTodosLosDatos.Checked)
                    {
                        tiempoDesdeElCualSePronostica = 0;
                        vectorFechasPronosticoLocal = new DateTime[intervalosDeTiempoAPronosticar + usarDatosHasta - usarDatosDesde + 1];
                        vectorDatosPronosticoLocal = new double[intervalosDeTiempoAPronosticar + usarDatosHasta - usarDatosDesde + 1];

                        for (int i = 0; i < (intervalosDeTiempoAPronosticar + usarDatosHasta - usarDatosDesde + 1); i++)
                        {
                            if(intervaloEnMeses)
                            {
                                vectorFechasPronosticoLocal[i] = primeraFechaDeDatos.AddMonths((i + 1) * intervaloDeTiempo);
                            }
                            else
                            {
                                vectorFechasPronosticoLocal[i] = primeraFechaDeDatos.AddDays((i + 1) * intervaloDeTiempo);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            j = Convert.ToInt32(textBoxPronosticoEspecificarValorDeJ.Text);
                            if (j <= 0 || j > (usarDatosHasta - usarDatosDesde))
                                throw new System.ArgumentException("j negativo, cero o muy grande ", "j negativo, cero o muy grande ");
                        }
                        catch
                        {
                            MessageBox.Show("No se pudo leer el valor de \"j\".\nRecuerde que \"j\" debe ser un entero positivo menor al numero de datos.\nSe usará un valor \"j\" igual a uno.", "Error al leer \"j\"", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            j = 1;
                            textBoxPronosticoEspecificarValorDeJ.Text = j.ToString();
                        }

                        tiempoDesdeElCualSePronostica = usarDatosHasta - usarDatosDesde + 1 - j;

                        vectorFechasPronosticoLocal = new DateTime[intervalosDeTiempoAPronosticar + j];
                        vectorDatosPronosticoLocal = new double[intervalosDeTiempoAPronosticar + j];

                        for (int i = 0; i < intervalosDeTiempoAPronosticar + j; i++)
                        {
                            if (intervaloEnMeses)
                            {
                                vectorFechasPronosticoLocal[i] = listaFechas[usarDatosHasta - j].AddMonths((i + 1) * intervaloDeTiempo);
                            }
                            else
                            {
                                vectorFechasPronosticoLocal[i] = listaFechas[usarDatosHasta - j].AddDays((i + 1) * intervaloDeTiempo);
                            }
                        }
                    }
                }
                

                if (indicadorModelo == "Exp")
                {
                    constructorDeModelos.PronosticarConModeloExponencial(ref vectorDatosPronosticoLocal, tiempoDesdeElCualSePronostica, produccionInicial, declinacionInicial);
                }
                if (indicadorModelo == "Com")
                {
                    constructorDeModelos.PronosticarConModeloCombinado(radioButtonPronosticoSolo.Checked, radioButtonPronosticoConTodosLosDatos.Checked, j, motorDeR, nombreDelVectorDeDatosEnR, ref vectorDatosPronosticoLocal, vectorValoresDelModelo, usarDatosDesde, usarDatosHasta, intervalosDeTiempoAPronosticar, tiempoDesdeElCualSePronostica, produccionInicial, declinacionInicial, n);
                }
                if (indicadorModelo == "Arm")
                {
                    constructorDeModelos.PronosticarConModeloArmonico(ref vectorDatosPronosticoLocal, tiempoDesdeElCualSePronostica, produccionInicial, declinacionInicial);
                }
                if (indicadorModelo == "Hip")
                {
                    constructorDeModelos.PronosticarConModeloHiperbolico(ref vectorDatosPronosticoLocal,tiempoDesdeElCualSePronostica, produccionInicial, declinacionInicial, n);
                }
                if (indicadorModelo == "ARI")
                {
                    if (radioButtonPronosticoSolo.Checked)
                    {
                        NumericVector prediccion;
                        if (radioButtonArimaAuto.Checked) //La funcion predict en ocasiones no funciona con auto.arima, y la funcion forecast a veces no funciona con arima especificado, por tanto se hace esto para garantizar que se realice la prediccion.
                        {
                            
                            motorDeR.Evaluate("pronostico <- forecast(modeloArima," + intervalosDeTiempoAPronosticar + ")");
                            prediccion = motorDeR.Evaluate("pronostico$mean").AsNumeric();
                        }
                        
                        else
                        {
                            motorDeR.Evaluate("pronostico <- predict(modeloArima," + intervalosDeTiempoAPronosticar + ")");
                            prediccion = motorDeR.Evaluate("pronostico$pred").AsNumeric();
                        }
                        

                        for (int i = 0; i < vectorDatosPronosticoLocal.Length; i++)
                        {
                            if(prediccion[i] < 0)
                            {
                                vectorDatosPronosticoLocal[i] = 0;
                                datosNegativos = true;
                            }
                            else
                            {
                                vectorDatosPronosticoLocal[i] = prediccion[i];
                                ultimoDatoPositivo = i;
                                intervalosPronosticados = i + 1;
                                datosNegativos = false;
                            }
                            
                        }
                    }
                    else
                    {
                        if(radioButtonPronosticoConTodosLosDatos.Checked)
                        {
                            NumericVector prediccion, valoresDelModelo;
                            if (radioButtonArimaAuto.Checked) //La funcion predict en ocasiones no funciona con auto.arima, y la funcion forecast a veces no funciona con arima especificado, por tanto se hace esto para garantizar que se realice la prediccion.
                            {

                                motorDeR.Evaluate("pronostico <- forecast(modeloArima," + intervalosDeTiempoAPronosticar + ")");
                                prediccion = motorDeR.Evaluate("pronostico$mean").AsNumeric();
                            }
                            
                            else
                            {
                                motorDeR.Evaluate("pronostico <- predict(modeloArima," + intervalosDeTiempoAPronosticar + ")");
                                prediccion = motorDeR.Evaluate("pronostico$pred").AsNumeric();
                            }
                            
                            valoresDelModelo = motorDeR.Evaluate(nombreDelVectorDeDatosEnR + "- residuos").AsNumeric();

                            for (int i = 0; i < usarDatosHasta - usarDatosDesde + 1; i++)
                            {
                                vectorDatosPronosticoLocal[i] = valoresDelModelo[i];
                            }

                            for (int i = 0; i < intervalosDeTiempoAPronosticar; i++)
                            {
                                if (prediccion[i] < 0)
                                {
                                    vectorDatosPronosticoLocal[i + usarDatosHasta - usarDatosDesde + 1] = 0;
                                    datosNegativos = true;
                                }
                                else
                                {
                                    vectorDatosPronosticoLocal[i + usarDatosHasta - usarDatosDesde + 1] = prediccion[i];
                                    ultimoDatoPositivo = i + usarDatosHasta - usarDatosDesde + 1;
                                    intervalosPronosticados = i + 1;
                                    datosNegativos = false;
                                }
                            }
                        }
                        else
                        {
                            NumericVector prediccion, valoresDelModelo;
                            if (radioButtonArimaAuto.Checked) //La funcion predict en ocasiones no funciona con auto.arima, y la funcion forecast a veces no funciona con arima especificado, por tanto se hace esto para garantizar que se realice la prediccion.
                            {

                                motorDeR.Evaluate("pronostico <- forecast(modeloArima," + intervalosDeTiempoAPronosticar + ")");
                                prediccion = motorDeR.Evaluate("pronostico$mean").AsNumeric();
                            }
                            
                            else
                            {
                                motorDeR.Evaluate("pronostico <- predict(modeloArima," + intervalosDeTiempoAPronosticar + ")");
                                prediccion = motorDeR.Evaluate("pronostico$pred").AsNumeric();
                            }
                            
                            valoresDelModelo = motorDeR.Evaluate(nombreDelVectorDeDatosEnR + "- residuos").AsNumeric();

                            for (int i = 0; i < j; i++)
                            {
                                vectorDatosPronosticoLocal[j - 1 - i] = valoresDelModelo[usarDatosHasta - usarDatosDesde - i];
                            }

                            for (int i = 0; i < intervalosDeTiempoAPronosticar; i++)
                            {
                                if (prediccion[i] < 0)
                                {
                                    vectorDatosPronosticoLocal[i + j] = 0;
                                    datosNegativos = true;
                                }
                                else
                                {
                                    vectorDatosPronosticoLocal[i + j] = prediccion[i];
                                    ultimoDatoPositivo = i + j;
                                    intervalosPronosticados = i + 1;
                                    datosNegativos = false;
                                }
                            }
                        }
                    }
                }

                if(datosNegativos)
                {
                    MessageBox.Show("Hubo datos negativos al generar el pronóstico, se hará el pronóstico hasta donde los datos sean positivos", "Datos negativos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DateTime[] vectorFechasPronosticoCorregido = new DateTime[ultimoDatoPositivo];
                    double[] vectorDatosPronosticoCorregido = new double[ultimoDatoPositivo];
                    for (int i = 0; i < ultimoDatoPositivo; i++)
                    {
                        vectorDatosPronosticoCorregido[i] = vectorDatosPronosticoLocal[i];
                        if (intervaloEnMeses)
                        {
                            vectorFechasPronosticoCorregido[i] = listaFechas[usarDatosHasta].AddMonths((i + 1) * intervaloDeTiempo);
                        }
                        else
                        {
                            vectorFechasPronosticoCorregido[i] = listaFechas[usarDatosHasta].AddDays((i + 1) * intervaloDeTiempo);
                        }
                    }
                    vectorDatosPronostico = vectorDatosPronosticoCorregido;
                    vectorFechasPronostico = vectorFechasPronosticoCorregido;
                    textBoxPronosticoIntervaloAPronosticar.Text = intervalosPronosticados.ToString();
                    intervalosDeTiempoAPronosticar = intervalosPronosticados;

                }
                else
                {
                    vectorDatosPronostico = vectorDatosPronosticoLocal;
                    vectorFechasPronostico = vectorFechasPronosticoLocal;
                }

                manejadorDeDatos.DesplegarDatosAlDataGrid(dataGridViewPronosticoDatos, 0, 1, vectorFechasPronostico, vectorDatosPronostico, longitudCadenaDeFecha);

                GraficarPronostico(vectorDatosPronostico);

                buttonPronosticoGraficar.Enabled = true;
                buttonPronosticoExportarAExcel.Enabled = true;
            }
            else
            {
                MessageBox.Show("Debe especificar un numero entero positivo", "Error al pronosticar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonPronosticoGraficar.Enabled = false;
                buttonPronosticoExportarAExcel.Enabled = false;
            }

            panelPronostico.Enabled = true;
        }

        private void GraficarPronostico(double[] vectorDatosPronostico)
        {
            Graphics papelParaPronostico = pictureBoxPronosticoDatos.CreateGraphics();
            Pen lapizParaPronostico = new Pen(Color.Black);
            int cantidadDeDatosGraficados = 0, numeroDeDatos = 0;
            double datoMaximo = 0;

            if (radioButtonPronosticoSolo.Checked)
            {
                manejadorDeDatos.EncontrarMaximo(vectorDatosPronostico, out datoMaximo);

                cantidadDeDatosGraficados = 0;
                numeroDeDatos = vectorDatosPronostico.Length;

                graficador.BorrarGrafico(pictureBoxPronosticoDatos, papelParaPronostico); // Limpia el picture box para poder graficar.

                lapizParaPronostico.Color = Color.Red;
                graficador.GraficarDatos(pictureBoxPronosticoDatos, papelParaPronostico, lapizParaPronostico, vectorDatosPronostico, numeroDeDatos, ref cantidadDeDatosGraficados, datoMaximo);
            }
            else
            {
                if (radioButtonPronosticoConTodosLosDatos.Checked)
                {
                    manejadorDeDatos.EncontrarMaximo(vectorDatosAjustados, out datoMaximo);

                    cantidadDeDatosGraficados = 0;
                    numeroDeDatos = vectorDatosPronostico.Length;

                    graficador.BorrarGrafico(pictureBoxPronosticoDatos, papelParaPronostico); // Limpia el picture box para poder graficar.

                    double[] vectorLocalAUsar = new double[usarDatosHasta - usarDatosDesde + 1];
                    for (int i = 0; i < vectorLocalAUsar.Length; i++)
                    {
                        vectorLocalAUsar[i] = vectorDatosAjustados[usarDatosDesde + i];
                    }

                    lapizParaPronostico.Color = Color.Black;
                    graficador.GraficarDatos(pictureBoxPronosticoDatos, papelParaPronostico, lapizParaPronostico, vectorLocalAUsar, numeroDeDatos, ref cantidadDeDatosGraficados, datoMaximo);

                    lapizParaPronostico.Color = Color.Red;
                    graficador.GraficarDatos(pictureBoxPronosticoDatos, papelParaPronostico, lapizParaPronostico, vectorDatosPronostico, numeroDeDatos, ref cantidadDeDatosGraficados, datoMaximo);

                    graficador.GraficarLimite(pictureBoxPronosticoDatos, numeroDeDatos, papelParaPronostico, lapizParaPronostico, (usarDatosHasta - usarDatosDesde));
                }
                else
                {
                    numeroDeDatos = vectorDatosPronostico.Length;
                    
                    double[] vectorLocalAUsar = new double[j];
                    for (int i = 0; i < j; i++)
                    {
                        vectorLocalAUsar[j - 1 - i] = vectorDatosAjustados[usarDatosHasta - i];
                    }

                    double maximoDatos, maximoPronostico;
                    manejadorDeDatos.EncontrarMaximo(vectorLocalAUsar, out datoMaximo);
                    maximoDatos = datoMaximo;
                    manejadorDeDatos.EncontrarMaximo(vectorDatosPronostico, out datoMaximo);
                    maximoPronostico = datoMaximo;

                    if (maximoDatos > maximoPronostico)
                        datoMaximo = maximoDatos;
                    else
                        datoMaximo = maximoPronostico;

                    cantidadDeDatosGraficados = 0;
                    numeroDeDatos = vectorDatosPronostico.Length;

                    graficador.BorrarGrafico(pictureBoxPronosticoDatos, papelParaPronostico); // Limpia el picture box para poder graficar.

                    lapizParaPronostico.Color = Color.Black;
                    graficador.GraficarDatos(pictureBoxPronosticoDatos, papelParaPronostico, lapizParaPronostico, vectorLocalAUsar, numeroDeDatos, ref cantidadDeDatosGraficados, datoMaximo);

                    lapizParaPronostico.Color = Color.Red;
                    graficador.GraficarDatos(pictureBoxPronosticoDatos, papelParaPronostico, lapizParaPronostico, vectorDatosPronostico, numeroDeDatos, ref cantidadDeDatosGraficados, datoMaximo);

                    graficador.GraficarLimite(pictureBoxPronosticoDatos, numeroDeDatos, papelParaPronostico, lapizParaPronostico, j - 1);

                }
            }

            if(numeroDeDatos != 0)
            {
                if (numeroDeDatos > pictureBoxPronosticoDatos.Width)
                {
                    labelPronosticoFechaFinal.Location = new System.Drawing.Point(pictureBoxPronosticoDatos.Location.X + cantidadDeDatosGraficados - 1 - labelPronosticoFechaFinal.Width / 2, tabPagePronostico.Height - margenVisual - labelPronosticoFechaFinal.Height);

                }
                else
                {
                    int a = (numeroDeDatos - 1) * Convert.ToInt32(pictureBoxPronosticoDatos.Width / numeroDeDatos);
                    labelPronosticoFechaFinal.Location = new System.Drawing.Point(pictureBoxPronosticoDatos.Location.X + a - labelPronosticoFechaFinal.Width, tabPagePronostico.Height - margenVisual - labelPronosticoFechaFinal.Height);
                }

                labelPronosticoProduccionMaxima.Text = datoMaximo.ToString();
                labelPronosticoFechaInicial.Text = vectorFechasPronostico[0].Date.ToString();
                labelPronosticoFechaFinal.Text = vectorFechasPronostico[vectorFechasPronostico.Length - 1].Date.ToString();
            }

            
        }

        private void buttonPronosticoGraficar_Click(object sender, EventArgs e)
        {
            GraficarPronostico(vectorDatosPronostico);
        }

        #endregion

        private void ReiniciarPanelesDeResultados()
        {
            textBoxResultadoExponenteN.Clear();
            textBoxResultadoModeloUsado.Clear();
            textBoxResultadoProduccionInicial.Clear();
            textBoxResultadoRCuadrado.Clear();
            textBoxResultadoValorP.Clear();
            textBoxResultadoAIC.Clear();
            textBoxResultadoDi.Clear();

            textBoxPronosticoExponenteN.Clear();
            textBoxPronosticoModeloUsado.Clear();
            textBoxPronosticoProduccionInicial.Clear();
            textBoxPronosticoRCuadrado.Clear();
            textBoxPronosticoValorP.Clear();
            textBoxPronosticoAIC.Clear();
            textBoxPronosticoDi.Clear();
        }

        private void buttonPronosticoExportarAExcel_Click(object sender, EventArgs e)
        {
            panelPronostico.Enabled = false;

            if (radioButtonPronosticoSolo.Checked)
                GenerarYGuardarArchivoDeExcelConSoloElPronostico();
            if (radioButtonPronosticoConTodosLosDatos.Checked)
                GenerarYGuardarArchivoDeExcelConTodosLosDatos();
            if (radioButtonPronosticoIncluyendoUltimosJDatos.Checked)
                GenerarYGuardarArchivoDeExcelConAlgunosDatos();

            panelPronostico.Enabled = true;
        }

        private void GenerarYGuardarArchivoDeExcelConSoloElPronostico()
        {
            Excel.Application excel = new Excel.Application();
            Excel._Workbook libro = null;
            Excel._Worksheet hoja = null;
            Excel.Range rango = null;

            try
            {
                string titulo = textBoxPronosticoTitulo.Text;
                if (titulo.Trim() == "")
                {
                    titulo = "Pronóstico de producción";
                    MessageBox.Show("No ha especificado el título que desea que lleve su informe.\nSe ha puesto el nombre por defecto: \"Pronóstico de producción\"", "Título no especificado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBoxPronosticoTitulo.Text = titulo;
                }
                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                libro = (Excel._Workbook)excel.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
                hoja = (Excel._Worksheet)libro.Worksheets.Add();

                if (titulo.Length < 31)
                {
                    hoja.Name = titulo;
                }
                else
                {
                    MessageBox.Show("El título es muy largo para nombrar la hoja del mismo modo. \nSe nombrará la hoja como: \"Pronóstico (ARIMA Forecasting)\"", "Nombre muy largo para la hoja de cálculo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    hoja.Name = "Pronóstico (ARIMA Forecasting)";
                }

                ((Excel.Worksheet)excel.ActiveWorkbook.Sheets["Hoja1"]).Delete();   //Borramos la hoja que crea en el libro por defecto

                rango = hoja.Columns[1];
                rango.ColumnWidth = 10;

                hoja.Cells[2, 2] = titulo;
                rango = hoja.Range["B2", "J2"];
                rango.Font.Size = 24;
                rango.Font.Bold = true;
                rango.Merge(true);
                rango = hoja.Rows[2];
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                hoja.Cells[4, 2] = "Fecha";
                hoja.Cells[4, 3] = "Producción pronosticada";
                rango = hoja.Range["B4", "C4"];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkBlue);
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                rango.Font.Bold = true;



                for (int i = 0; i < intervalosDeTiempoAPronosticar; i++)
                {
                    hoja.Cells[i + 5, 2] = vectorFechasPronostico[i].Date.ToString().Substring(0, longitudCadenaDeFecha);
                    hoja.Cells[i + 5, 3] = vectorDatosPronostico[i];

                    rango = hoja.Range["B" + Convert.ToString(i + 5), "C" + Convert.ToString(i + 5)];
                    if (i % 2 == 0)
                    {
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.AliceBlue);
                    }
                    else
                    {
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);
                    }
                }

                rango = excel.Columns[3];
                rango.ColumnWidth = 22.14;
                rango = excel.Columns[4];
                rango.ColumnWidth = 22.14;

                rango = hoja.Range["B4", "C" + Convert.ToString(vectorDatosPronostico.Length + 4)];

                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

                CrearGraficoEnExcel(ref hoja, rango);
                DesplegarResultadosEnExcel(ref hoja);

                saveFileDialogGuardarArchivoExportado.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Inicializa el buscador de archivos en la carpeta mis documentos.
                saveFileDialogGuardarArchivoExportado.Filter = "Archivos de Excel (*.xlsx)|*.xlsx"; //Muestra solo los archivos .xlsx
                saveFileDialogGuardarArchivoExportado.FileName = titulo;

                bool archivoGuardado = false;

                if (saveFileDialogGuardarArchivoExportado.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        libro.SaveAs(saveFileDialogGuardarArchivoExportado.FileName);
                        archivoGuardado = true;
                    }
                    catch
                    {
                        MessageBox.Show("El archivo no pudo ser guardado", "No se guardó el archivo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        archivoGuardado = false;
                    }
                }

                // Cerrar Excel

                libro.Close();
                ReleaseObject(libro);

                excel.UserControl = false;
                excel.Quit();
                ReleaseObject(excel);

                // Abrir el archivo generado

                if (archivoGuardado)
                {
                    Process.Start(saveFileDialogGuardarArchivoExportado.FileName);
                }

            }
            catch
            {
                MessageBox.Show("Ha ocurrido un error al intentar crear el archivo de Excel.", "Error al crear archivo de Excel", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Cerrar Excel

                libro.Close();
                ReleaseObject(libro);

                excel.UserControl = false;
                excel.Quit();
                ReleaseObject(excel);
            }
        }

        private void GenerarYGuardarArchivoDeExcelConTodosLosDatos()
        {
            Excel.Application excel = new Excel.Application();
            Excel._Workbook libro = null;
            Excel._Worksheet hoja = null;
            Excel.Range rango = null;

            try
            {
                string titulo = textBoxPronosticoTitulo.Text;
                if (titulo.Trim() == "")
                {
                    titulo = "Pronóstico de producción";
                    MessageBox.Show("No ha especificado el título que desea que lleve su informe.\nSe ha puesto el nombre por defecto: \"Pronóstico de producción\"", "Título no especificado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBoxPronosticoTitulo.Text = titulo;
                }
                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                libro = (Excel._Workbook)excel.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
                hoja = (Excel._Worksheet)libro.Worksheets.Add();

                if (titulo.Length < 31)
                {
                    hoja.Name = titulo;
                }
                else
                {
                    MessageBox.Show("El título es muy largo para nombrar la hoja del mismo modo. \nSe nombrará la hoja como: \"Pronóstico (ARIMA Forecasting)\"", "Nombre muy largo para la hoja de cálculo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    hoja.Name = "Pronóstico (ARIMA Forecasting)";
                }

                ((Excel.Worksheet)excel.ActiveWorkbook.Sheets["Hoja1"]).Delete();   //Borramos la hoja que crea en el libro por defecto

                rango = hoja.Columns[1];
                rango.ColumnWidth = 10;

                hoja.Cells[2, 2] = titulo;
                rango = hoja.Range["B2", "J2"];
                rango.Font.Size = 24;
                rango.Font.Bold = true;
                rango.Merge(true);
                rango = hoja.Rows[2];
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                hoja.Cells[4, 2] = "Fecha";
                hoja.Cells[4, 3] = "Producción";
                hoja.Cells[4, 4] = "Producción pronosticada";
                rango = hoja.Range["B4", "D4"];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkBlue);
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                rango.Font.Bold = true;

                for(int i = 0; i < usarDatosHasta - usarDatosDesde + 1; i++)
                {
                    hoja.Cells[i + 5, 3] = vectorDatosAjustados[usarDatosDesde + i];
                }

                for (int i = 0; i < vectorDatosPronostico.Length; i++)
                {
                    hoja.Cells[i + 5, 2] = vectorFechasPronostico[i].Date.ToString().Substring(0, longitudCadenaDeFecha) + " ";
                    hoja.Cells[i + 5, 4] = vectorDatosPronostico[i];

                    rango = hoja.Range["B" + Convert.ToString(i + 5), "D" + Convert.ToString(i + 5)];
                    if (i % 2 == 0)
                    {
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.AliceBlue);
                    }
                    else
                    {
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);
                    }
                }

                //Si el usuario no desea tener los datos del modelo antes de la fecha a partir de la cual se pronostica para ver el ajuste, se borran dichos datos.

                if (MessageBox.Show("¿Desea que además del pronóstico se incluyan en el archivo de Excel los datos dados por el modelo (previos a la fecha a partir de la cual se pronostica) para compararlos con los datos reales y ver el ajuste?", "¿Incluir datos del modelo para ver el ajuste?", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                {
                    for (int i = 0; i < j; i++)
                    {
                        hoja.Cells[i + 5, 4] = "";
                    }
                }

                rango = excel.Columns[3];
                rango.ColumnWidth = 22.14;
                rango = excel.Columns[4];
                rango.ColumnWidth = 22.14;

                rango = hoja.Range["B4", "D" + Convert.ToString(vectorDatosPronostico.Length + 4)];

                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

                CrearGraficoEnExcel(ref hoja, rango);
                DesplegarResultadosEnExcel(ref hoja);

                saveFileDialogGuardarArchivoExportado.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Inicializa el buscador de archivos en la carpeta mis documentos.
                saveFileDialogGuardarArchivoExportado.Filter = "Archivos de Excel (*.xlsx)|*.xlsx"; //Muestra solo los archivos .xlsx
                saveFileDialogGuardarArchivoExportado.FileName = titulo;

                bool archivoGuardado = false;

                if (saveFileDialogGuardarArchivoExportado.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        libro.SaveAs(saveFileDialogGuardarArchivoExportado.FileName);
                        archivoGuardado = true;
                    }
                    catch
                    {
                        MessageBox.Show("El archivo no pudo ser guardado", "No se guardó el archivo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        archivoGuardado = false;
                    }
                }

                // Cerrar Excel

                libro.Close();
                ReleaseObject(libro);

                excel.UserControl = false;
                excel.Quit();
                ReleaseObject(excel);

                // Abrir el archivo generado

                if (archivoGuardado)
                {
                    Process.Start(saveFileDialogGuardarArchivoExportado.FileName);
                }

            }
            catch
            {
                MessageBox.Show("Ha ocurrido un error al intentar crear el archivo de Excel.", "Error al crear archivo de Excel", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Cerrar Excel

                libro.Close();
                ReleaseObject(libro);

                excel.UserControl = false;
                excel.Quit();
                ReleaseObject(excel);
            }
        }

        private void GenerarYGuardarArchivoDeExcelConAlgunosDatos()
        {
            Excel.Application excel = new Excel.Application();
            Excel._Workbook libro = null;
            Excel._Worksheet hoja = null;
            Excel.Range rango = null;

            try
            {
                string titulo = textBoxPronosticoTitulo.Text.Trim();
                if (titulo == "")
                {
                    titulo = "Pronóstico de producción";
                    MessageBox.Show("No ha especificado el título que desea que lleve su informe.\nSe ha puesto el nombre por defecto: \"Pronóstico de producción\"", "Título no especificado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBoxPronosticoTitulo.Text = titulo;
                }
                if(titulo.IndexOf('\n') >= 0)
                {
                    titulo = titulo.Substring(0, titulo.IndexOf('\n'));

                }
                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                libro = (Excel._Workbook)excel.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
                hoja = (Excel._Worksheet)libro.Worksheets.Add();

                if(titulo.Length < 31)
                {
                    hoja.Name = titulo;
                }
                else
                {
                    MessageBox.Show("El título es muy largo para nombrar la hoja del mismo modo. \nSe nombrará la hoja como: \"Pronóstico (ARIMA Forecasting)\"", "Nombre muy largo para la hoja de cálculo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    hoja.Name = "Pronóstico (ARIMA Forecasting)";
                }
                
                ((Excel.Worksheet)excel.ActiveWorkbook.Sheets["Hoja1"]).Delete();   //Borramos la hoja que crea en el libro por defecto

                rango = hoja.Columns[1];
                rango.ColumnWidth = 10;

                hoja.Cells[2, 2] = titulo;
                rango = hoja.Range["B2", "J2"];
                rango.Font.Size = 24;
                rango.Font.Bold = true;
                rango.Merge(true);
                rango = hoja.Rows[2];
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                hoja.Cells[4, 2] = "Fecha";
                hoja.Cells[4, 3] = "Producción";
                hoja.Cells[4, 4] = "Producción pronosticada";
                rango = hoja.Range["B4", "D4"];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkBlue);
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                rango.Font.Bold = true;

                for (int i = 0; i < j; i++)
                {
                    hoja.Cells[i + 5, 3] = vectorDatosAjustados[usarDatosHasta - j + 1 + i];
                }

                for (int i = 0; i < vectorDatosPronostico.Length; i++)
                {
                    hoja.Cells[i + 5, 2] = vectorFechasPronostico[i].Date.ToString().Substring(0, longitudCadenaDeFecha);
                    hoja.Cells[i + 5, 4] = vectorDatosPronostico[i];

                    rango = hoja.Range["B" + Convert.ToString(i + 5), "D" + Convert.ToString(i + 5)];
                    if (i % 2 == 0)
                    {
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.AliceBlue);
                    }
                    else
                    {
                        rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);
                    }
                }

                //Si el usuario no desea tener los datos del modelo antes de la fecha a partir de la cual se pronostica para ver el ajuste, se borran dichos datos.
                if (MessageBox.Show("¿Desea que además del pronóstico se incluyan en el archivo de Excel los datos dados por el modelo (previos a la fecha a partir de la cual se pronostica) para compararlos con los datos reales y ver el ajuste?", "¿Incluir datos del modelo para ver el ajuste?", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                {
                    for (int i = 0; i < j; i++)
                    {
                        hoja.Cells[i + 5, 4] = "";
                    }
                }

                rango = excel.Columns[3];
                rango.ColumnWidth = 22.14;
                rango = excel.Columns[4];
                rango.ColumnWidth = 22.14;

                rango = hoja.Range["B4", "D" + Convert.ToString(vectorDatosPronostico.Length + 4)];
                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                rango.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

                CrearGraficoEnExcel(ref hoja, rango);
                DesplegarResultadosEnExcel(ref hoja);

                saveFileDialogGuardarArchivoExportado.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Inicializa el buscador de archivos en la carpeta mis documentos.
                saveFileDialogGuardarArchivoExportado.Filter = "Archivos de Excel (*.xlsx)|*.xlsx"; //Muestra solo los archivos .xlsx
                saveFileDialogGuardarArchivoExportado.FileName = titulo;

                bool archivoGuardado = false;

                if (saveFileDialogGuardarArchivoExportado.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        libro.SaveAs(saveFileDialogGuardarArchivoExportado.FileName);
                        archivoGuardado = true;
                    }
                    catch
                    {
                        MessageBox.Show("El archivo no pudo ser guardado", "No se guardó el archivo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        archivoGuardado = false;
                    }
                }

                // Cerrar Excel

                libro.Close();
                ReleaseObject(libro);

                excel.UserControl = false;
                excel.Quit();
                ReleaseObject(excel);

                // Abrir el archivo generado

                if (archivoGuardado)
                {
                    Process.Start(saveFileDialogGuardarArchivoExportado.FileName);
                }
            }
        
            catch
            {
                MessageBox.Show("Ha ocurrido un error al intentar crear el archivo de Excel.", "Error al crear archivo de Excel", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Cerrar Excel

                libro.Close();
                ReleaseObject(libro);

                excel.UserControl = false;
                excel.Quit();
                ReleaseObject(excel);
            }
        }



        private void CrearGraficoEnExcel(ref Excel._Worksheet hoja, Excel.Range rango)
        {
            try
            {
                Excel.ChartObjects coleccionDeGraficos = null; //Colección de gráficos de la hoja
                Excel.ChartObject contenedorDeGrafico = null;   //Contenedor del objeto chart
                Excel.Chart grafico = null;           //Objeto gráfico con el que vamos a trabajar
                Excel.Range rangoGrafico = null;
                Excel.Range rangoParaCalculos = null;
                object misValue = System.Reflection.Missing.Value;
                
                

                coleccionDeGraficos = (Excel.ChartObjects)hoja.ChartObjects(Type.Missing);
                contenedorDeGrafico = (Excel.ChartObject)coleccionDeGraficos.Add(415, 61.5, 400, 250);  //izq, arriba, ancho, alto
                grafico = contenedorDeGrafico.Chart;

                rangoGrafico = rango;
                grafico.ChartType = Excel.XlChartType.xlLine;
                grafico.HasTitle = true;
                grafico.ChartTitle.Caption = "Pronóstico de producción";
                grafico.ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowValue);
                grafico.SetSourceData(rangoGrafico);
                contenedorDeGrafico.Chart.HasLegend = true;
                contenedorDeGrafico.Chart.ShowDataLabelsOverMaximum = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al intentar crear el gráfico en el archivo de Excel.", "Error al crear archivo de Excel", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DesplegarResultadosEnExcel(ref Excel._Worksheet hoja)
        {
            Excel.Range rangoParaCalculos = null;

            int renglonResultados = 22;

            rangoParaCalculos = hoja.Range["F"+ Convert.ToString(renglonResultados), "G" + Convert.ToString(renglonResultados)];
            rangoParaCalculos.Merge(true);
            rangoParaCalculos.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkBlue);
            rangoParaCalculos.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
            rangoParaCalculos.Font.Bold = true;
            hoja.Cells[renglonResultados, 6] = "Resultados";

            hoja.Cells[renglonResultados + 1, 6] = "Modelo usado:";
            hoja.Cells[renglonResultados + 1, 7] = modeloUsado;
            hoja.Cells[renglonResultados + 2, 6] = "Fecha de primer dato usado:";
            hoja.Cells[renglonResultados + 2, 7] = listaFechas[usarDatosDesde].Date.ToString().Substring(0, longitudCadenaDeFecha);
            hoja.Cells[renglonResultados + 3, 6] = "Fecha de último dato usado:";
            hoja.Cells[renglonResultados + 3, 7] = listaFechas[usarDatosHasta].Date.ToString().Substring(0, longitudCadenaDeFecha);

            hoja.Cells[renglonResultados + 4, 6] = "Pronosticado hasta:";
            hoja.Cells[renglonResultados + 4, 7] = vectorFechasPronostico[vectorFechasPronostico.Length - 1].Date.ToString().Substring(0, longitudCadenaDeFecha);
            hoja.Cells[renglonResultados + 5, 6] = "R cuadrado:";
            hoja.Cells[renglonResultados + 5, 7] = r2;
            

            rangoParaCalculos = hoja.Range["G" + Convert.ToString(renglonResultados + 1), "G" + Convert.ToString(renglonResultados + 7)];
            rangoParaCalculos.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

            for (int i = 0; i < 7; i++)
            {
                rangoParaCalculos = hoja.Range["F" + Convert.ToString(renglonResultados + 1 + i), "G" + Convert.ToString(renglonResultados + 1 + i)];
                if (i % 2 == 0)
                {
                    rangoParaCalculos.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.AliceBlue);
                }
                else
                {
                    rangoParaCalculos.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);
                }
            }

            rangoParaCalculos = hoja.Range["F" + Convert.ToString(renglonResultados), "G" + Convert.ToString(renglonResultados + 7)];
            rangoParaCalculos.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            rangoParaCalculos.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

            rangoParaCalculos = hoja.Range["I" + Convert.ToString(renglonResultados), "J" + Convert.ToString(renglonResultados)];
            rangoParaCalculos.Merge(true);
            rangoParaCalculos.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkBlue);
            rangoParaCalculos.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
            rangoParaCalculos.Font.Bold = true;
            hoja.Cells[renglonResultados, 9] = "Coeficientes y parámetros";

            if (indicadorModelo == "ARI")
            {
                hoja.Cells[renglonResultados + 6, 6] = "AIC:";
                hoja.Cells[renglonResultados + 6, 7] = aic;
                hoja.Cells[renglonResultados + 7, 6] = "Valor P:";
                hoja.Cells[renglonResultados + 7, 7] = valorP;

                for (int i = 0; i < coeficientesArima.Length; i++)
                {
                    if(i < arimaP)
                    {
                        hoja.Cells[renglonResultados + 1 + i, 9] = "AR "+ Convert.ToString(i + 1) + ":";
                        hoja.Cells[renglonResultados + 1 + i, 10] = coeficientesArima[i];
                    }
                    else
                    {
                        hoja.Cells[renglonResultados + 1 + i, 9] = "MA " + Convert.ToString(i + 1 - arimaP) + ":";
                        hoja.Cells[renglonResultados + 1 + i, 10] = coeficientesArima[i];
                    }


                    rangoParaCalculos = hoja.Range["I" + Convert.ToString(renglonResultados + 1 + i), "J" + Convert.ToString(renglonResultados + 1 + i)];
                    if (i % 2 == 0)
                    {
                        rangoParaCalculos.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.AliceBlue);
                    }
                    else
                    {
                        rangoParaCalculos.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);
                    }
                }
                rangoParaCalculos = hoja.Range["I" + Convert.ToString(renglonResultados), "J" + Convert.ToString(coeficientesArima.Length + renglonResultados)];
                rangoParaCalculos.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                rangoParaCalculos.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                
            }
            else
            {
                hoja.Cells[renglonResultados + 1, 9] = "Producción inicial:";
                hoja.Cells[renglonResultados + 1, 10] = produccionInicial;
                hoja.Cells[renglonResultados + 2, 9] = "Declinación inicial:";
                hoja.Cells[renglonResultados + 2, 10] = declinacionInicial;
                hoja.Cells[renglonResultados + 3, 9] = "Exponente n:";
                hoja.Cells[renglonResultados + 3, 10] = n;

                for (int i = 0; i < 3; i++)
                {
                    rangoParaCalculos = hoja.Range["I" + Convert.ToString(renglonResultados + 1 + i), "J" + Convert.ToString(renglonResultados + 1 + i)];
                    if (i % 2 == 0)
                    {
                        rangoParaCalculos.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.AliceBlue);
                    }
                    else
                    {
                        rangoParaCalculos.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);
                    }
                }
                rangoParaCalculos = hoja.Range["I" + Convert.ToString(renglonResultados), "J" + Convert.ToString(renglonResultados)];
                rangoParaCalculos.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                rangoParaCalculos.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
            }

            rangoParaCalculos = hoja.Columns[9];
            rangoParaCalculos.ColumnWidth = 17;

            rangoParaCalculos = hoja.Columns[10];
            rangoParaCalculos.ColumnWidth = 15;
            hoja.Columns[6].autofit();
            hoja.Columns[7].autofit();
        }

        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Error mientras se liberaba el objeto " + ex.ToString(), "Error al liberar de la memoria", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
