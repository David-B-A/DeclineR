using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeclineR.Pantalla_Principal
{
    public partial class InformacionDeModelosUsados : Form
    {
        string[] vectorModelosUsados;
        string[] vectorAicDeModelosUsados;
        string[] vectorR2DeModelosUsados;
        string[] vectorNDeModelosUsados;
        string[] vectorDiDeModelosUsados;
        string[] vectorProduccionInicialDeModelosUsados;
        string[] vectorFechaDatoInicialDeModelosUsados;
        string[] vectorFechaDatoFinalDeModelosUsados;

        public InformacionDeModelosUsados(string[] vectorModelos, string[] vectorFechaDatoInicial, string[] vectorFechaDatoFinal, string[] vectorAic, string[] vectorR2, string[] vectorN, string[] vectorDi, string[] vectorQi)
        {
            InitializeComponent();
            vectorModelosUsados = vectorModelos;
            vectorAicDeModelosUsados = vectorAic;
            vectorR2DeModelosUsados = vectorR2;
            vectorNDeModelosUsados = vectorN;
            vectorDiDeModelosUsados = vectorDi;
            vectorProduccionInicialDeModelosUsados = vectorQi;
            vectorFechaDatoInicialDeModelosUsados = vectorFechaDatoInicial;
            vectorFechaDatoFinalDeModelosUsados = vectorFechaDatoFinal;
        }

        private void InformacionDeModelosUsados_Load(object sender, EventArgs e)
        {
            dataGridViewInformacionDeModelosUsados.RowCount = vectorAicDeModelosUsados.Length;

            for(int i = 0; i < vectorAicDeModelosUsados.Length; i++)
            {
                dataGridViewInformacionDeModelosUsados[0, i].Value = vectorModelosUsados[i];
                dataGridViewInformacionDeModelosUsados[1, i].Value = vectorFechaDatoInicialDeModelosUsados[i];
                dataGridViewInformacionDeModelosUsados[2, i].Value = vectorFechaDatoFinalDeModelosUsados[i];
                dataGridViewInformacionDeModelosUsados[3, i].Value = vectorNDeModelosUsados[i];
                dataGridViewInformacionDeModelosUsados[4, i].Value = vectorDiDeModelosUsados[i];
                dataGridViewInformacionDeModelosUsados[5, i].Value = vectorProduccionInicialDeModelosUsados[i];
                dataGridViewInformacionDeModelosUsados[6, i].Value = vectorAicDeModelosUsados[i];
                dataGridViewInformacionDeModelosUsados[7, i].Value = vectorR2DeModelosUsados[i];
            }
        }

        private void dataGridViewInformacionDeModelosUsados_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.C))
            {
                DataObject d = dataGridViewInformacionDeModelosUsados.GetClipboardContent();
                Clipboard.SetDataObject(d);
            }
        }

        private void InformacionDeModelosUsados_SizeChanged(object sender, EventArgs e)
        {
            dataGridViewInformacionDeModelosUsados.Width = this.Width - 16;
            dataGridViewInformacionDeModelosUsados.Height = this.Height - 38;
        }
    }
}
