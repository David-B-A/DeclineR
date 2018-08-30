using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.IO;

namespace DeclineR.Pantalla_Principal
{
    class DesplegadorDeArchivoEnDataGridView
    {
        public void Proceso(DataGridView dataGridViewCargadorDeDatos, string direccionArchivo, bool titulosExistentes, char separadorColumnas)
        {
            dataGridViewCargadorDeDatos.RowCount = 1;
            dataGridViewCargadorDeDatos.ColumnCount = 1;
            try
            {
                StreamReader textoArchivo = File.OpenText(direccionArchivo);
                string temporal, linea = textoArchivo.ReadLine();
                string[] celdas, titulos;
                int renglon = 0, columna = 0;

                if (titulosExistentes) //Si hay titulos en el archivo se los pone a las columnas.
                {
                    titulos = linea.Split(separadorColumnas);
                    dataGridViewCargadorDeDatos.ColumnCount = titulos.Length;
                    foreach (string titulo in titulos)
                    {
                        temporal = titulo.Trim();
                        if (temporal != "")
                        {
                            dataGridViewCargadorDeDatos.Columns[columna].Name = temporal;
                        }
                        else
                        {
                            dataGridViewCargadorDeDatos.Columns[columna].Name = Convert.ToString(columna + 1);
                        }

                        columna++;
                    }
                    linea = textoArchivo.ReadLine();
                }

                while (linea != null)
                {
                    celdas = linea.Split(separadorColumnas);
                    columna = 0;
                    if (dataGridViewCargadorDeDatos.ColumnCount < celdas.Length) //Asegura que existan las columnas suficientes para albergar los datos.
                    {
                        dataGridViewCargadorDeDatos.ColumnCount = celdas.Length;
                    }

                    foreach (string celda in celdas)
                    {
                        dataGridViewCargadorDeDatos[columna, renglon].Value = celda;
                        columna++;
                    }

                    linea = textoArchivo.ReadLine();

                    dataGridViewCargadorDeDatos.Rows.Add();
                    renglon++;
                }


                if (titulosExistentes == false) //Si no hay titulos en el archivo, enumera las columnas.
                {
                    for (int i = 0; i < dataGridViewCargadorDeDatos.ColumnCount; i++)
                    {
                        dataGridViewCargadorDeDatos.Columns[i].Name = Convert.ToString(i + 1);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Parece que no se ha especificado ningun archivo.\nPor favor seleccione el archivo e intente nuevamente.", "Error al abrir archivo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
