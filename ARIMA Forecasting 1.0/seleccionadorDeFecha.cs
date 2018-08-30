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
    public partial class seleccionadorDeFecha : Form
    {
        public EventHandler<FechaSeleccionadaEvenArgs> fechaSeleccionada;
        DateTime fecha;

        public seleccionadorDeFecha()
        {
            InitializeComponent();
        }

        private void buttonSeleccionarFecha_Click(object sender, EventArgs e)
        {
            fecha = monthCalendar.SelectionRange.Start;
            EventHandler<FechaSeleccionadaEvenArgs> manejadorDeEvento = fechaSeleccionada;
            manejadorDeEvento(this, new FechaSeleccionadaEvenArgs() { fecha = fecha});
            this.Close();
        }

        private void seleccionadorDeFecha_Load(object sender, EventArgs e)
        {
        }
    }

    public class FechaSeleccionadaEvenArgs : EventArgs
    {
        public DateTime fecha;
    }
}
