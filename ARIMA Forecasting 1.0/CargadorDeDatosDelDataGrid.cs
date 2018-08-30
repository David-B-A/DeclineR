using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace DeclineR.Pantalla_Principal
{
    class CargadorDeDatosDelDataGrid
    {
        public void Proceso(DataGridView dataGridViewCargadorDeDatos, int columnaFechas, int columnaDatos, string separadorDecimal, string formatoFecha, string separadorDeMiles, List<DateTime> listaFechas, List<double> listaDatos)
        {
            string fecha, dato;
            int posicionSeparadorDecimal, posicionSeparadorDeMiles;
            DateTime fechaConvertida;

            for (int i = 0; i < dataGridViewCargadorDeDatos.RowCount; i++)
            {
                if (Convert.ToString(dataGridViewCargadorDeDatos[columnaFechas, i].Value) != "") //Recorre el DataGridView convirtiendo y guardando las fechas y los datos de produccion.
                {
                    if (Convert.ToString(dataGridViewCargadorDeDatos[columnaFechas, i].Value).Length > formatoFecha.Length)//Si el dato que sale del DataGridView tiene un "\n" al final de la linea (lo que es usual) o datos de hora, se eliminan para poder trabajar con el dato.
                    {
                        fecha = Convert.ToString(dataGridViewCargadorDeDatos[columnaFechas, i].Value).Substring(0, formatoFecha.Length);
                    }
                    else
                    {
                        fecha = Convert.ToString(dataGridViewCargadorDeDatos[columnaFechas, i].Value);
                    }

                    dato = Convert.ToString(dataGridViewCargadorDeDatos[columnaDatos, i].Value);


                    while (dato.Length > 0 && dato[dato.Length - 1] != '1' && dato[dato.Length - 1] != '2' && dato[dato.Length - 1] != '3' && dato[dato.Length - 1] != '4' && dato[dato.Length - 1] != '5' && dato[dato.Length - 1] != '6' && dato[dato.Length - 1] != '7' && dato[dato.Length - 1] != '8' && dato[dato.Length - 1] != '9' && dato[dato.Length - 1] != '0')
                    {
                        dato = dato.Substring(0, dato.Length - 1);//El dato que sale del DataGridView tiene un "\n" al final de la linea, el "\n" se elimina para poder trabajar con el dato. Ademas si el dato no existe y hay alguna etiqueta como "Na" o "-" se elimina, y se deja vacia la cadena, para mas adelante manejarla como dato faltante
                    }

                    if (DateTime.TryParseExact(fecha, formatoFecha, null, DateTimeStyles.None, out fechaConvertida)) // Convierte el texto en la celda del DataGridView a formato DateTime
                    {
                        listaFechas.Add(fechaConvertida);
                    }
                    else
                    {
                        MessageBox.Show("Error al cargar los datos.\nAsegurese de que los datos esten en el formato correcto e intente nuevamente ", "Error de carga de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    //Para el siguiente metodo se usaba NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator pero al final, se usó punto ".", ya que c# trabaja internamente con .
                    if (separadorDecimal != ".") //Si el separador decimal especificado no es el establecido en la configuracion regional del usuario, se reemplaza por este para poder cargar el dato y convertirlo a dato numerico
                    {
                        posicionSeparadorDecimal = dato.IndexOf(separadorDecimal);
                        if (posicionSeparadorDecimal >= 0)
                        {
                            dato = dato.Remove(posicionSeparadorDecimal, 1);
                            dato = dato.Insert(posicionSeparadorDecimal, ".");
                            //MessageBox.Show(NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator, NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator);
                        }
                    }


                    if (separadorDeMiles != "") //Si el separador de miles existe se elimina para poder cargar el dato y convertirlo a dato numerico
                    {
                        posicionSeparadorDeMiles = dato.IndexOf(separadorDeMiles);
                        while (posicionSeparadorDeMiles >= 0)
                        {
                            dato = dato.Remove(posicionSeparadorDeMiles, 1);
                        }
                    }
                    
                    //para validar
                    /*
                    MessageBox.Show(NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator, "");
                    MessageBox.Show(Convert.ToString(Convert.ToDouble(dato)), "");
                    MessageBox.Show(Convert.ToString(1999.2), "");
                    MessageBox.Show(Convert.ToString(Convert.ToDouble("1999,2")), "");
                    MessageBox.Show(Convert.ToString(Convert.ToDouble("1999.2")), "");
                    */
                    try
                    {
                        dato = dato.Trim();
                        if (dato != "")
                        {
                            listaDatos.Add(Convert.ToDouble(dato));
                        }
                        else
                        {
                            listaDatos.Add(0);
                        }

                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error al cargar los datos.\nAsegurese de que los datos esten en el formato correcto e intente nuevamente", "Error de carga de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                }
            }



            //Es necesario advertir al usuario sobre la posible perdida de datos:
             
            if (listaFechas.Count < dataGridViewCargadorDeDatos.RowCount)
            {
                MessageBox.Show("El numero de datos es mayor que el numero de fechas disponibles, por tanto se perderan algunos datos.", "Inconsistencia datos y fechas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (listaFechas.Count < listaDatos.Count)
                {
                    listaDatos.RemoveRange(listaFechas.Count, listaDatos.Count - listaFechas.Count);
                }
            }
        }
    }
}
