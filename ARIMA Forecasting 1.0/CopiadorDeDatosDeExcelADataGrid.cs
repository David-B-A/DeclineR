using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace DeclineR.Pantalla_Principal
{
    class CopiadorDeDatosDeExcelADataGrid
    {
        public void Copiar(object sender, DataGridView dataGridViewCargadorDeDatos, CheckBox checkBoxDefinirRangoDeFechas)
        {
            DataObject d = dataGridViewCargadorDeDatos.GetClipboardContent();
            Clipboard.SetDataObject(d);
        }
        public void Pegar(object sender, DataGridView dataGridViewCargadorDeDatos, CheckBox checkBoxDefinirRangoDeFechas)
        {
            try
            {
                string textoDelPortapapeles = Clipboard.GetText();
                string[] lineasCopiadas = textoDelPortapapeles.Split('\n');
                int renglon = dataGridViewCargadorDeDatos.CurrentCell.RowIndex;
                int columna = dataGridViewCargadorDeDatos.CurrentCell.ColumnIndex;


                if (!dataGridViewCargadorDeDatos.Columns[columna].ReadOnly) //Se asegura de que no se peguen fechas sobre el rango especificado si lo hay.
                {
                    if (dataGridViewCargadorDeDatos.RowCount < lineasCopiadas.Length + renglon - 1 && !checkBoxDefinirRangoDeFechas.Checked) //Crea los renglones necesarios para albergar los datos si no hay un rango de fechas definido.
                    {
                        dataGridViewCargadorDeDatos.RowCount = lineasCopiadas.Length + renglon - 1;
                    }

                    if (dataGridViewCargadorDeDatos.RowCount < lineasCopiadas.Length + renglon - 1)
                    {
                        string lineaCopiada;
                        MessageBox.Show("No se han definido fechas para todos los datos que se desean copiar.\nSe copiaran solo algunos datos", "Numero de datos mayor que las fechas especificadas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        for (int lineaCopiadaPosicion = 0; lineaCopiadaPosicion < dataGridViewCargadorDeDatos.RowCount - renglon; lineaCopiadaPosicion++)
                        {
                            lineaCopiada = lineasCopiadas[lineaCopiadaPosicion];
                            CopiarLineaAGrid(dataGridViewCargadorDeDatos, lineaCopiada, columna, renglon + lineaCopiadaPosicion);
                        }
                    }
                    else
                    {
                        foreach (string lineaCopiada in lineasCopiadas)
                        {
                            CopiarLineaAGrid(dataGridViewCargadorDeDatos, lineaCopiada, columna, renglon);
                            renglon++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("No se ha podido copiar todos los datos", "Error al intentar copiar datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        

        protected void CopiarLineaAGrid(DataGridView dataGridViewCargadorDeDatos, string lineaCopiada, int columna, int renglon)
        {
            if (lineaCopiada.Length > 0)
            {
                string[] celdasCopiadas = lineaCopiada.Split('\t');
                for (int i = 0; i < celdasCopiadas.GetLength(0); ++i)
                {
                    if (columna + i < dataGridViewCargadorDeDatos.ColumnCount)
                    {
                        if (!dataGridViewCargadorDeDatos.Columns[columna + i].ReadOnly)
                        {
                            dataGridViewCargadorDeDatos[columna + i, renglon].Value = Convert.ChangeType(celdasCopiadas[i], dataGridViewCargadorDeDatos[columna + i, renglon].ValueType);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

    }
}
