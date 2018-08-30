using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;

namespace DeclineR.Pantalla_Principal
{
    class Graficador
    {
        ManejadorDeDatos manejadorDeDatos = new ManejadorDeDatos();
        public void GraficarDatos(PictureBox pictureBoxParaGraficar, Graphics papel, Pen lapiz, double [] vectorDatos, int numeroDeDatos, ref int cantidadDeDatosGraficados, double produccionMaxima)
        {
            if(vectorDatos != null && vectorDatos.Length > 2) //Se asegura de que haya datos suficientes para graficar
            {
                double datoMaximo = Convert.ToInt32(produccionMaxima);
                //Se crean los objetos necesarios para graficar

                lapiz.Width = 1;

                int ancho = pictureBoxParaGraficar.Width, alto = pictureBoxParaGraficar.Height, pixelesEntreDatos = 1, posicionMaximo;
                int datosIntervalo = 1, xInicial, yInicial, xFinal, yFinal, cantidadDeDatos = 0;

                /*Es necesario saber si los datos caben en los pixeles correspondientes al alto del cuadro de imagen, y si no entonces ajustar una escala.
                 *Es posible que haya mas datos que pixeles en el ancho del cuadro de imagen, asi que se debe ajustar la grafica para que alcance a mostrar todos los datos. Lo hace el metodo definirDatos.
                 */

                
                DefinirDatos(pictureBoxParaGraficar, vectorDatos, numeroDeDatos , ref datosIntervalo,ref cantidadDeDatos, ref pixelesEntreDatos); //Se asegura de que todos los datos quepan en el cuadro de imagen.

                int[] datosParaGraficar = new int[cantidadDeDatos];

                int a = 0, b = 0;

                while (a < vectorDatos.Length && b < datosParaGraficar.Length)
                {
                    datosParaGraficar[b] = Convert.ToInt32((Math.Round(vectorDatos[a] * (alto / datoMaximo)))); //Convierte los datos a la escala necesaria
                    
                    if (datosParaGraficar[b] < 0) //Se asegura de que no haya datos negativos para evitar error en la gráfica (ya que está hecha para presentar solo datos positivos)
                    {
                        datosParaGraficar[b] = 0;
                    }
                    a = a + datosIntervalo;
                    b++;
                }

                datosParaGraficar[datosParaGraficar.Length - 1] = Convert.ToInt32((Math.Round(vectorDatos[vectorDatos.Length - 1] * (alto / datoMaximo)))); //Convierte los datos a la escala necesaria
                
                for (int i = 0; i < datosParaGraficar.Length - 1; i++)
                {
                    xInicial = i * pixelesEntreDatos;
                    xFinal = (i + 1) * pixelesEntreDatos;
                    yInicial = alto - datosParaGraficar[i];
                    yFinal = alto - datosParaGraficar[i + 1];

                    papel.DrawLine(lapiz, xInicial, yInicial, xFinal, yFinal);
                }

                cantidadDeDatosGraficados = cantidadDeDatos;
            }
            else cantidadDeDatosGraficados = 0;

        }

        public void BorrarGrafico(PictureBox pictureBoxParaGraficar, Graphics papel)
        {
            papel.Clear(Color.White);
        }

        public void GraficarLimite(PictureBox pictureBoxParaGraficar, List<double> listaDatos, int numeroDeDatos, Graphics papel, Pen lapiz, int limite) //Grafica una linea en el punto correspondiente al dato indicado
        {
            int datosIntervalo = 1, cantidadDeDatos = 0, pixelesEntreDatos = 1;
            double[] vectorDatos = new double[listaDatos.Count];
            DefinirDatos(pictureBoxParaGraficar, listaDatos, numeroDeDatos, ref datosIntervalo, ref cantidadDeDatos, ref pixelesEntreDatos);

            lapiz.Color = Color.BlueViolet;
            lapiz.Width = 1;
           
            papel.DrawLine(lapiz, Convert.ToInt32(limite * pixelesEntreDatos / (datosIntervalo)), 0, Convert.ToInt32(limite * pixelesEntreDatos / (datosIntervalo)), pictureBoxParaGraficar.Height);
        }

        public void GraficarLimite(PictureBox pictureBoxParaGraficar, int numeroDeDatos, Graphics papel, Pen lapiz, int limite) //Grafica una linea en el punto correspondiente al dato indicado
        {
            int datosIntervalo = 1, cantidadDeDatos = 0, pixelesEntreDatos = 1;
            DefinirDatos(pictureBoxParaGraficar, numeroDeDatos, ref datosIntervalo, ref cantidadDeDatos, ref pixelesEntreDatos);

            lapiz.Color = Color.BlueViolet;
            lapiz.Width = 2;

            papel.DrawLine(lapiz, Convert.ToInt32(limite * pixelesEntreDatos / (datosIntervalo)), 0, Convert.ToInt32(limite * pixelesEntreDatos / (datosIntervalo)), pictureBoxParaGraficar.Height);
        }

        protected void DefinirDatos(PictureBox pictureBoxParaGraficar,double[] vectorDatosAGraficar, int numeroDeDatos,ref int datosIntervalo, ref int cantidadDeDatos, ref int pixelesEntreDatos)
        {
            //Es posible que haya mas datos que pixeles en el ancho del cuadro de imagen, asi que se debe ajustar la grafica para que alcance a mostrar todos los datos.

            if (pictureBoxParaGraficar.Width < numeroDeDatos)
            {
                datosIntervalo = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(numeroDeDatos) / pictureBoxParaGraficar.Width));
                cantidadDeDatos = Convert.ToInt32(Math.Floor(Convert.ToDouble(vectorDatosAGraficar.Length) / datosIntervalo) +1);
                pixelesEntreDatos = 1;
            }
            else
            {
                pixelesEntreDatos = pictureBoxParaGraficar.Width / numeroDeDatos;
                cantidadDeDatos = vectorDatosAGraficar.Length;
                datosIntervalo = 1;
            }
        }

        protected void DefinirDatos(PictureBox pictureBoxParaGraficar, List<double> listaDatosAGraficar, int numeroDeDatos, ref int datosIntervalo, ref int cantidadDeDatos, ref int pixelesEntreDatos)
        {
            //Es posible que haya mas datos que pixeles en el ancho del cuadro de imagen, asi que se debe ajustar la grafica para que alcance a mostrar todos los datos.

            if (pictureBoxParaGraficar.Width < numeroDeDatos)
            {
                datosIntervalo = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(numeroDeDatos) / pictureBoxParaGraficar.Width));
                cantidadDeDatos = Convert.ToInt32(Math.Floor(Convert.ToDouble(listaDatosAGraficar.Count) / datosIntervalo) + 1);
                pixelesEntreDatos = 1;
            }
            else
            {
                pixelesEntreDatos = pictureBoxParaGraficar.Width / numeroDeDatos;
                cantidadDeDatos = listaDatosAGraficar.Count;
                datosIntervalo = 1;
            }
        }

        protected void DefinirDatos(PictureBox pictureBoxParaGraficar, int numeroDeDatos, ref int datosIntervalo, ref int cantidadDeDatos, ref int pixelesEntreDatos)
        {
            //Es posible que haya mas datos que pixeles en el ancho del cuadro de imagen, asi que se debe ajustar la grafica para que alcance a mostrar todos los datos.

            if (pictureBoxParaGraficar.Width < numeroDeDatos)
            {
                datosIntervalo = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(numeroDeDatos) / pictureBoxParaGraficar.Width));
                cantidadDeDatos = Convert.ToInt32(Math.Floor(Convert.ToDouble(numeroDeDatos) / datosIntervalo) + 1);
                pixelesEntreDatos = 1;
            }
            else
            {
                pixelesEntreDatos = pictureBoxParaGraficar.Width / numeroDeDatos;
                cantidadDeDatos = numeroDeDatos;
                datosIntervalo = 1;
            }
        }

    }
}
