using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace DeclineR.Pantalla_Principal
{
    class ManejadorDeDatos
    {
        public void EvaluarContinuidadDeDatosYDatosNegativos(List<DateTime> listaFechas, List<double> listaDatos, int intervaloDeTiempo, bool intervaloEnMeses)
        {
            int j = 0, numeroDeDatosNegativos = 0;
            while (j < listaDatos.Count)
            {

                if (listaDatos[j] < 0)
                {
                    numeroDeDatosNegativos++;
                    listaDatos[j] = 0;
                }
                j++;
            }

            if (numeroDeDatosNegativos > 0)
            {
                MessageBox.Show("Se eliminaron " + numeroDeDatosNegativos + " datos que eran negativos", "Datos negativos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            double sumaDatosDeUnMismoDia = 0;
            DateTime evaluador = new DateTime();
            int a = 0, i = 0;
            while (i < listaFechas.Count - 1)
            {
                evaluador = listaFechas[i];
                if (intervaloEnMeses)
                {
                    evaluador = listaFechas[i].AddMonths(intervaloDeTiempo);
                }
                else
                {
                    evaluador = listaFechas[i].AddDays(intervaloDeTiempo);
                }

                sumaDatosDeUnMismoDia = listaDatos[i];
                a = i;
                while ((listaFechas.Count - 1) > i && listaFechas[i] == listaFechas[i + 1]) /*Se toma (i+1) que para este entorno es una constante ya que al ir eliminando el elemento (i+1) la lista se va corriendo y actualizando.
                                                                                              Se usa el operador && porque si el primer termino es falso, no se evalua el segundo. Asi se asegura que en el ultimo dato no se presente error en el segundo termino por la no existencia de listaFechas[i + 1]  */
                {
                    sumaDatosDeUnMismoDia = sumaDatosDeUnMismoDia + listaDatos[i + 1];
                    listaDatos.RemoveAt(i + 1);
                    listaFechas.RemoveAt(i + 1); ; //Asi se va actualizando la lista y corriendose a la derecha
                    a++;
                }

                if (a != i)
                {
                    listaDatos[i] = sumaDatosDeUnMismoDia / (a - i + 1);
                }

                if ((listaFechas.Count - 1) > i && evaluador.CompareTo(listaFechas[i + 1]) < 0) /*Primero se comprueba la existencia de listaFechas[i + 1], para evitar errores (como se explico anteriormente)
                                                                                                  La funcion dateTime1.CompareTo(dateTime2) compara las fechas, y devuelve un entero menor, igual o mayor que cero dependiendo si es anterior, igual o posterior la 1 a la 2.*/
                {
                    listaFechas.Insert(i + 1, evaluador);
                    listaDatos.Insert(i + 1, 0);
                }
                else
                {
                    while ((listaFechas.Count - 1) > i && evaluador.CompareTo(listaFechas[i + 1]) > 0) //Se usa el operador && porque si el primer termino es falso, no se evalua el segundo. Asi se asegura que en el ultimo dato no se presente error en el segundo termino por la no existencia de listaFechas[i + 1]
                    {
                        listaFechas.RemoveAt(i + 1);
                        listaDatos.RemoveAt(i + 1);
                    }
                }

                //Es necesario advertir al usuario sobre una posible perdida de datos, y asegurar que los datos correspondan a las fechas dadas.
                if (listaFechas.Count < listaDatos.Count)
                {
                    MessageBox.Show("El numero de datos es mayor que el numero de fechas disponibles, por tanto, se perderan algunos datos", "Inconsistencia datos y fechas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    listaDatos.RemoveRange(listaFechas.Count, listaDatos.Count - listaFechas.Count);
                }

                i++;
            }


        }

        public void DesplegarDatosAlDataGrid(DataGridView dataGridViewDatos, int columnaFechas, int columnaDatos, List<DateTime> listaFechas, List<double> listaDatos, int longitudCadenaDeFecha) //Despliega listas
        {
            try
            {
                dataGridViewDatos.RowCount = listaFechas.Count;

                int renglon = 0;
                foreach (DateTime fecha in listaFechas) //Recorre la lista de fechas mostrando en el datagrid el valor correspondiente a fecha y dato.
                {
                    dataGridViewDatos[columnaFechas, renglon].Value = fecha.Date.ToString().Substring(0, longitudCadenaDeFecha);
                    dataGridViewDatos[columnaDatos, renglon].Value = listaDatos[renglon].ToString();

                    renglon++;
                }
            }
            catch
            {
                MessageBox.Show("No hay datos para cargar", "No se obtuvieron datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void DesplegarDatosAlDataGrid(DataGridView dataGridViewDatos, int columnaFechas, int columnaDatos, DateTime[] vectorFechas, double[] vectorDatos, int longitudCadenaDeFecha) //Despliega vectores
        {
            try
            {
                dataGridViewDatos.RowCount = vectorFechas.Length;

                int renglon = 0;
                foreach (DateTime fecha in vectorFechas) //Recorre el vector de fechas mostrando en el datagrid el valor correspondiente a fecha y dato.
                {
                    dataGridViewDatos[columnaFechas, renglon].Value = fecha.Date.ToString().Substring(0, longitudCadenaDeFecha);
                    dataGridViewDatos[columnaDatos, renglon].Value = vectorDatos[renglon].ToString();

                    renglon++;
                }
            }
            catch
            {
                MessageBox.Show("No hay datos para cargar", "No se obtuvieron datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void DesplegarDatosAlDataGrid(DataGridView dataGridViewDatos, int columnaFechas, int columnaDatos, List<DateTime> listaFechas, double[] vectorDatos, int longitudCadenaDeFecha)
        {
            try
            {
                dataGridViewDatos.RowCount = listaFechas.Count;

                int renglon = 0;
                foreach (DateTime fecha in listaFechas) //Recorre la lista de fechas mostrando en el datagrid el valor correspondiente a fecha y dato.
                {
                    dataGridViewDatos[columnaFechas, renglon].Value = fecha.Date.ToString().Substring(0, longitudCadenaDeFecha);
                    dataGridViewDatos[columnaDatos, renglon].Value = vectorDatos[renglon].ToString();

                    renglon++;
                }
            }
            catch
            {
                MessageBox.Show("No hay datos para cargar", "No se obtuvieron datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void AjustarDatosPorMediaMovil(List<double> listaDatos, ref double[] vectorDeDatosAjustados)
        {
            int i = 7;
            double suma = 0;
            double[] vectorLocalDatosAjustados = new double[listaDatos.Count];

            for(int j=0; j<listaDatos.Count; j++)
            {
                vectorLocalDatosAjustados[j] = listaDatos[j];
            }

            if(vectorLocalDatosAjustados.Length > i)
            {
                while (i < vectorLocalDatosAjustados.Length)
                {
                    
                    if (vectorLocalDatosAjustados[i] <= 0)
                    {
                        suma = 0;
                        for (int a = 1; a <= 7; a++)
                        {
                            suma = suma + vectorLocalDatosAjustados[i - a];
                        }
                        vectorLocalDatosAjustados[i] = suma / 7;
                    }
                    i++;
                }
                vectorDeDatosAjustados = vectorLocalDatosAjustados;
            }
            else
            {
                MessageBox.Show("No hay suficientes datos", "Escasez de datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                for (int j = 0; j < listaDatos.Count; j++)
                {
                    vectorLocalDatosAjustados[j] = listaDatos[j];
                }
            }
        }

        public void EncontrarMaximo(double[] vectorDatos, out double datoMaximo, out int posicionMaximo)
        {
            datoMaximo = 0;
            posicionMaximo = 0;
            
            if (vectorDatos != null)
            {
                for (int i = 0; i < vectorDatos.Length; i++)
                {
                    if (vectorDatos[i] > datoMaximo)
                    {
                        datoMaximo = vectorDatos[i];
                        posicionMaximo = i;
                    }
                }
            }
        }

        public void EncontrarMaximo(double[] vectorDatos, out double datoMaximo)
        {
            datoMaximo = 0;

            if (vectorDatos != null)
            {
                for (int i = 0; i < vectorDatos.Length; i++)
                {
                    if (vectorDatos[i] > datoMaximo)
                    {
                        datoMaximo = vectorDatos[i];
                    }
                }
            }
        }
    }
}
