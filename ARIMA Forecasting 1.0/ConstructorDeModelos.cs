using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RDotNet;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Globalization;





namespace DeclineR.Pantalla_Principal
{
    class ConstructorDeModelos
    {
        protected double nMinimoParaNoSerCero = 0.005;

        public void ConstruirModeloHiperbolicoDandoDiYN(double[] vectorDeDatos, out double[] vectorValoresDelModelo, int usarDatosDesde, int usarDatosHasta, double n,double declinacionInicial, out double r2, out double aic)
        {
            int numeroDeParametros = 2;
            double produccionInicial = vectorDeDatos[usarDatosDesde], produccionFinal = vectorDeDatos[usarDatosHasta], ti = 0, tf = usarDatosHasta;
            double[] vectorLocalValoresDelModelo = new double[usarDatosHasta - usarDatosDesde + 1];
            if (Math.Abs(n) > nMinimoParaNoSerCero)
            {

                DarValoresAlModeloHiperbolico(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial, n);
                HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
                HallarAIC(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out aic, numeroDeParametros);
            }
            else
            {
                DarValoresAlModeloExponencial(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial);
                HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
                HallarAIC(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out aic, numeroDeParametros);
            }

            vectorValoresDelModelo = vectorLocalValoresDelModelo;
        }

        public void ConstruirModeloHiperbolicoQuePasePorElUltimoPunto(double[] vectorDeDatos, out double[] vectorValoresDelModelo, int usarDatosDesde, int usarDatosHasta, double n,out double declinacionInicial,out double r2,out double aic)
        {
            int numeroDeParametros = 2;
            double produccionInicial = vectorDeDatos[usarDatosDesde], produccionFinal = vectorDeDatos[usarDatosHasta], ti = usarDatosDesde, tf = usarDatosHasta - usarDatosDesde;
            double[] vectorLocalValoresDelModelo = new double[usarDatosHasta - usarDatosDesde + 1];

            if(Math.Abs(n) > nMinimoParaNoSerCero)
            {
                declinacionInicial = (-1 + Math.Pow(produccionInicial / produccionFinal, n)) / (n * tf);

                DarValoresAlModeloHiperbolico(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial, n);
                HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
                HallarAIC(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out aic, numeroDeParametros);
            }
            else
            {

                declinacionInicial = -Math.Log(produccionFinal / produccionInicial) / tf;

                DarValoresAlModeloExponencial(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial);
                HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
                HallarAIC(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out aic, numeroDeParametros);
            }

            vectorValoresDelModelo = vectorLocalValoresDelModelo;

        }

        public void ConstruirModeloHiperbolicoAutomatico(double[] vectorDeDatos, out double[] vectorValoresDelModelo, int usarDatosDesde, int usarDatosHasta, int numeroDeIteraciones,out double declinacionInicial, out double n, out double r2, out double aic)
        {
            /*Se plantea que el modelo esté obligado a pasar por el punto inicial y final de los datos
             */

            double a = 0.1, b = 1, produccionInicial = vectorDeDatos[usarDatosDesde], produccionFinal = vectorDeDatos[usarDatosHasta], ti=usarDatosDesde, tf=usarDatosHasta - usarDatosDesde;
            double[] vectorLocalValoresDelModelo = new double[usarDatosHasta - usarDatosDesde + 1];
            double r2Anterior, paso = 0.1, na, nb, r2na, r2nb;
            int numeroDeParametros = 2;

            n = -1;
            if (Math.Abs(n) < nMinimoParaNoSerCero)
            {
                declinacionInicial = -Math.Log(produccionFinal / produccionInicial) / (tf);
                n = 0;
            }
            else
            {
                declinacionInicial = (-1 + Math.Pow(produccionInicial / produccionFinal, n)) / (n * tf);
            }
            
            DarValoresAlModeloHiperbolico(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial, n);
            HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
            r2Anterior = r2;

            while (Math.Abs(r2) >= r2Anterior)
            {
                if(r2 >= 0)
                {
                    r2Anterior = r2;
                }
                else
                {
                    r2Anterior = 0;
                }
                
                n = n + paso;
                if (Math.Abs(n) < nMinimoParaNoSerCero)
                {
                    declinacionInicial = -Math.Log(produccionFinal / produccionInicial) / (tf);
                    n = 0;
                }
                else
                {
                    declinacionInicial = (-1 + Math.Pow(produccionInicial / produccionFinal, n)) / (n * tf);
                }
                DarValoresAlModeloHiperbolico(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial, n);
                HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
            }
            
                a = n - 2 * paso;
                b = n + paso;

                for (int iteracion = 0; iteracion <= numeroDeIteraciones; iteracion++)
                {
                    na = a + (b - a) / 4;
                    nb = a + 3 * (b - a) / 4;
                    if (na == 0)
                    {
                        declinacionInicial = -Math.Log(produccionFinal / produccionInicial) / (tf);
                    }
                    else
                    {
                        declinacionInicial = (-1 + Math.Pow(produccionInicial / produccionFinal, na)) / (na * tf);
                    }

                    DarValoresAlModeloHiperbolico(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial, na);
                    HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2na);

                    if (nb == 0)
                    {
                        declinacionInicial = -Math.Log(produccionFinal / produccionInicial) / (tf);
                    }
                    else
                    {
                        declinacionInicial = (-1 + Math.Pow(produccionInicial / produccionFinal, nb)) / (nb * tf);
                    }

                    DarValoresAlModeloHiperbolico(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial, nb);
                    HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2nb);

                    if (r2na > r2nb)
                    {
                        b = nb;
                    }
                    else
                    {
                        a = na;
                    }
                }
                n = (a + b) / 2;

            if (Math.Abs(n) < nMinimoParaNoSerCero)
            {
                declinacionInicial = -Math.Log(produccionFinal / produccionInicial) / (tf);
                n = 0;
            }
            else
            {
                declinacionInicial = (-1 + Math.Pow(produccionInicial / produccionFinal, n)) / (n * tf);
            }
            DarValoresAlModeloHiperbolico(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial, n);
            HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
            HallarAIC(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out aic, numeroDeParametros);
            vectorValoresDelModelo = vectorLocalValoresDelModelo;
        }

        public void ConstruirModeloHiperbolicoAutomaticoConNewtonRaphson(double[] vectorDeDatos, out double[] vectorValoresDelModelo, int usarDatosDesde, int usarDatosHasta, int numeroDeIteraciones, out double declinacionInicial, out double n, out double r2, out double aic)
        {
            /*Se plantea que el modelo esté obligado a pasar por el punto inicial y final de los datos
             */

            double produccionInicial = vectorDeDatos[usarDatosDesde], produccionFinal = vectorDeDatos[usarDatosHasta], ti = usarDatosHasta, tf = usarDatosHasta - usarDatosDesde;
            double[] vectorLocalValoresDelModelo = new double[usarDatosHasta - usarDatosDesde + 1];
            double r2Derivada, paso = 0.1, a, b;
            int numeroDeParametros = 2;
            n = 0.33;
            declinacionInicial = (-1 + Math.Pow(produccionInicial / produccionFinal, n)) / (n * tf);
            DarValoresAlModeloHiperbolico(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial, n);
            HallarR2Derivada(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2Derivada, n, declinacionInicial);

            if (r2Derivada != 0)
            {
                if (r2Derivada < 0)
                {
                    while (r2Derivada < 0)
                    {
                        n = n - paso;
                        declinacionInicial = (-1 + Math.Pow(produccionInicial / produccionFinal, n)) / (n * tf);
                        DarValoresAlModeloHiperbolico(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial, n);
                        HallarR2Derivada(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2Derivada, n, declinacionInicial);
                    }

                    n = n + paso;

                }
                else
                {
                    while (r2Derivada > 0)
                    {
                        n = n + paso;
                        declinacionInicial = (-1 + Math.Pow(produccionInicial / produccionFinal, n)) / (n * tf);
                        DarValoresAlModeloHiperbolico(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial, n);
                        HallarR2Derivada(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2Derivada, n, declinacionInicial);
                    }
                }

                a = n - paso;
                b = n;
                n = (a + b) / 2;
                for (int i = 0; i <= numeroDeIteraciones; i++)
                {
                    
                    declinacionInicial = (-1 + Math.Pow(produccionInicial / produccionFinal, n)) / (n * tf);
                    DarValoresAlModeloHiperbolico(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial, n);
                    HallarR2Derivada(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2Derivada, n, declinacionInicial);

                    if(r2Derivada > 0 )
                    {
                        a = n;
                    }
                    else
                    {
                        b = n;
                    }
                    n = (a + b) / 2;
                    declinacionInicial = (-1 + Math.Pow(produccionInicial / produccionFinal, n)) / (n * tf);
                }
            }
            
            DarValoresAlModeloHiperbolico(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial, n);
            HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
            HallarAIC(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out aic, numeroDeParametros);
            vectorValoresDelModelo = vectorLocalValoresDelModelo;
        }

        protected void DarValoresAlModeloHiperbolico(ref double[] vectorLocalValoresDelModelo, double produccionInicial, double declinacionInicial, double n)
        {
            if(Math.Abs(n) < nMinimoParaNoSerCero)
            {
                DarValoresAlModeloExponencial(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicial);
            }
            else
            {
                for (int tiempo = 0; tiempo < vectorLocalValoresDelModelo.Length; tiempo++)
                {
                    vectorLocalValoresDelModelo[tiempo] = produccionInicial * Math.Pow(1 + n * declinacionInicial * tiempo, (-1 / n));
                }
            }
        }

        public void PronosticarConModeloHiperbolico(ref double[] vectorValoresDelModelo, int tiempoAPartirDelCualSePronostica, double produccionInicial, double declinacionInicial, double n)
        {
            int tiempo;
            for (int i = 0; i < vectorValoresDelModelo.Length; i++)
            {
                tiempo = i + tiempoAPartirDelCualSePronostica;
                vectorValoresDelModelo[i] = produccionInicial * Math.Pow(1 + n * declinacionInicial * tiempo, (-1 / n));
            }
        }

        protected void HallarR2(double[] vectorDeDatos, double[] vectorLocalValoresDelModelo, int usarDatosDesde, int usarDatosHasta, out double r2)
        {
            double suma = 0, varianza = 1;
            for (int i = 0;i < vectorLocalValoresDelModelo.Length; i++)
            {
                suma = suma + Math.Pow((vectorLocalValoresDelModelo[i] - vectorDeDatos[i + usarDatosDesde]), 2);
            }
            HallarVarianzaDeDatos(vectorDeDatos, usarDatosDesde, usarDatosHasta, out varianza);
            r2 = 1 - (suma /* / vectorLocalValoresDelModelo.Length */) / varianza;
        }

        protected void HallarR2Derivada(double[] vectorDeDatos, double[] vectorLocalValoresDelModelo, int usarDatosDesde, int usarDatosHasta, out double r2derivada, double n, double di)
        {
            int tiempo;
            double suma = 0, varianza = 1, media, produccionInicial = vectorDeDatos[usarDatosDesde];
            HallarMedia(vectorDeDatos, out media, usarDatosDesde, usarDatosHasta);
            for (int i = 0; i < vectorLocalValoresDelModelo.Length; i++)
            {
                tiempo = i;
                suma = suma + (produccionInicial * Math.Pow((1 + n*di*tiempo), (-1/n)) - vectorDeDatos[usarDatosDesde+ i]) * Math.Pow((1 + n * di * tiempo), (-1 / n)) * ((1 / Math.Pow(n,2)) *Math.Log(1 + n * di * tiempo) - (1 / n)*((di * tiempo)/(1 + n * di * tiempo)));
            }
            HallarVarianzaDeDatos(vectorDeDatos, usarDatosDesde, usarDatosHasta, out varianza);
            r2derivada = 2*produccionInicial*(suma /* / vectorLocalValoresDelModelo.Length */) / (varianza);
        }


        protected void HallarVarianzaDeDatos(double[] vectorDeDatos, int usarDatosDesde, int usarDatosHasta,out double varianza)
        {
            double media, suma=0;
            HallarMedia(vectorDeDatos, out media, usarDatosDesde, usarDatosHasta);
            for (int i = usarDatosDesde; i <= usarDatosHasta; i++)
            {
                suma = suma + Math.Pow(vectorDeDatos[i] - media, 2);
            }
            varianza = suma /* / (usarDatosHasta - usarDatosDesde + 1)*/;
        }

        protected void HallarMedia(double[] vectorDatos, out double media, int usarDatosDesde, int usarDatosHasta)
        {
            double suma = 0;
            for (int i = usarDatosDesde; i <= usarDatosHasta; i++)
            {
                suma = suma + vectorDatos[i];
            }
            media = suma / (usarDatosHasta - usarDatosDesde + 1);
        }

        protected void HallarAIC(double[] vectorDeDatos, double[] vectorLocalValoresDelModelo, int usarDatosDesde, int usarDatosHasta, out double aic, int numeroDeParametros)
        {
            double suma = 0;
            for (int i = 0; i < vectorLocalValoresDelModelo.Length; i++)
            {
                suma = suma + Math.Pow((vectorLocalValoresDelModelo[i] - vectorDeDatos[i + usarDatosDesde]), 2);
            }
            aic = Math.Log(Math.Exp(2 * numeroDeParametros / vectorLocalValoresDelModelo.Length) * suma / vectorLocalValoresDelModelo.Length);
        }


        
        public void ConstruirModeloExponencialQuePasePorElUltimoPunto(double[] vectorDeDatos, out double[] vectorValoresDelModelo, int usarDatosDesde, int usarDatosHasta, out double declinacionInicial, ref double r2, ref double aic)
        {
            int numeroDeParametros = 1;
            int tf = usarDatosHasta - usarDatosDesde, ti = usarDatosDesde;
            double produccionInicial = vectorDeDatos[usarDatosDesde], produccionFinal = vectorDeDatos[usarDatosHasta], declinacionInicialLocal;
            double[] vectorLocalValoresDelModelo = new double[usarDatosHasta - usarDatosDesde + 1];

            //Es necesario asegurarse de que el ultimo dato no sea cero, para que no se genere error al realizar el modelo

            if(vectorDeDatos[tf] != 0)
            {
                declinacionInicialLocal = -Math.Log(produccionFinal / produccionInicial) / (tf);

                DarValoresAlModeloExponencial(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicialLocal);
                HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
                HallarAIC(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out aic, numeroDeParametros);
                vectorValoresDelModelo = vectorLocalValoresDelModelo;
                declinacionInicial = declinacionInicialLocal;
                
            }
            else
            {
                MessageBox.Show("El ultimo dato de produccion es cero (0), por tanto no puede ser usado para realizar el modelo\nPor favor regrese a la pestaña de datos e ignore los ultimos datos de manera que el ultimo a usar sea diferente de cero (0)", "Error de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                vectorValoresDelModelo = vectorLocalValoresDelModelo;
                declinacionInicial = 0;
            }
            


        }

        public void ConstruirModeloExponencialEspecificandoDi(double[] vectorDeDatos, out double[] vectorValoresDelModelo, int usarDatosDesde, int usarDatosHasta, double declinacionInicial, ref double r2, ref double aic)
        {
            int numeroDeParametros = 1;
            int tf = usarDatosHasta - usarDatosDesde, ti = usarDatosDesde;
            double produccionInicial = vectorDeDatos[usarDatosDesde], produccionFinal = vectorDeDatos[usarDatosHasta], declinacionInicialLocal;
            double[] vectorLocalValoresDelModelo = new double[usarDatosHasta - usarDatosDesde + 1];

            declinacionInicialLocal = declinacionInicial;

            DarValoresAlModeloExponencial(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicialLocal);
            HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
            HallarAIC(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out aic, numeroDeParametros);
            vectorValoresDelModelo = vectorLocalValoresDelModelo;
        }

        public void ConstruirMejorModeloExponencial(REngine motorDeR, string nombreDelVectorDeDatosR,NumericVector datosParaR,double[] vectorDeDatos, out double[] vectorValoresDelModelo, int usarDatosDesde, int usarDatosHasta,out double declinacionInicial, ref double produccionInicial, ref double r2, ref double aic)
        {
            int numeroDeParametros = 2;
            double declinacionInicialLocal;
            double[] vectorLocalValoresDelModelo = new double[usarDatosHasta -  usarDatosDesde + 1];
            
            NumericVector coeficientesDesdeR = motorDeR.CreateNumericVector(2);
             
            int numeroDeDatos = usarDatosHasta - usarDatosDesde + 1;

            motorDeR.Evaluate("matriz <- matrix(nrow = length(" + nombreDelVectorDeDatosR + "), ncol = 2)");
            motorDeR.Evaluate("tiempo <- c(1:(length(" + nombreDelVectorDeDatosR + ")))");
            motorDeR.Evaluate("matriz[,1] <- 1");
            motorDeR.Evaluate("matriz[,2] <- tiempo");
            motorDeR.Evaluate("vectorLogaritmico <- log(" + nombreDelVectorDeDatosR + ")");
            motorDeR.Evaluate("i=1\nwhile (i < length(vectorLogaritmico))\n{\nif (" + nombreDelVectorDeDatosR + "[i] == 0)\n{\nvectorLogaritmico[i] = 0\n}\ni = i + 1\n}");
            motorDeR.Evaluate("coeficientesLogaritmico <- solve(t(matriz) %*% matriz) %*% (t(matriz) %*% vectorLogaritmico)");

            coeficientesDesdeR = motorDeR.Evaluate("coeficientesLogaritmico").AsNumeric();
            produccionInicial = Math.Exp(coeficientesDesdeR[0]);
            declinacionInicialLocal = -coeficientesDesdeR[1];
            DarValoresAlModeloExponencial(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicialLocal);
            HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
            HallarAIC(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out aic, numeroDeParametros);

            vectorValoresDelModelo = vectorLocalValoresDelModelo;
            declinacionInicial = declinacionInicialLocal;
        
        }

        protected void DarValoresAlModeloExponencial(ref double[] vectorLocalValoresDelModelo, double produccionInicial, double declinacionInicial)
        {
            for (int tiempo = 0; tiempo < vectorLocalValoresDelModelo.Length; tiempo++)
            {
                vectorLocalValoresDelModelo[tiempo] = produccionInicial * Math.Exp(-declinacionInicial*tiempo);
            }

        }

        public void PronosticarConModeloExponencial(ref double[] vectorValoresDelModelo, int tiempoAPartirDelCualSePronostica, double produccionInicial, double declinacionInicial)
        {
            int tiempo;
            for (int i = 0; i < vectorValoresDelModelo.Length; i++)
            {
                tiempo = i + tiempoAPartirDelCualSePronostica;
                vectorValoresDelModelo[i] = produccionInicial * Math.Exp(-declinacionInicial * tiempo);
            }

        }



        public void ConstruirModeloArmonicoQuePasePorElUltimoPunto(double[] vectorDeDatos, out double[] vectorValoresDelModelo, int usarDatosDesde, int usarDatosHasta, out double declinacionInicial,ref double r2,ref double aic)
        {
            int numeroDeParametros = 1;
            int tf = usarDatosHasta - usarDatosDesde, ti = usarDatosDesde;
            double produccionInicial = vectorDeDatos[usarDatosDesde], produccionFinal = vectorDeDatos[usarDatosHasta], declinacionInicialLocal;
            double[] vectorLocalValoresDelModelo = new double[usarDatosHasta - usarDatosDesde + 1];

            //Es necesario asegurarse de que el ultimo dato no sea cero, para que no se genere error al realizar el modelo

            if (vectorDeDatos[tf] != 0)
            {
                declinacionInicialLocal = ((produccionInicial / produccionFinal) - 1) / tf;

                DarValoresAlModeloArmonico(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicialLocal);
                HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
                HallarAIC(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out aic, numeroDeParametros);
                vectorValoresDelModelo = vectorLocalValoresDelModelo;

                declinacionInicial = declinacionInicialLocal;
            }
            else
            {
                MessageBox.Show("El ultimo dato de produccion es cero (0), por tanto no puede ser usado para realizar el modelo\nPor favor regrese a la pestaña de datos e ignore los ultimos datos de manera que el ultimo a usar sea diferente de cero (0)", "Error de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                vectorValoresDelModelo = vectorLocalValoresDelModelo;
                declinacionInicial = 0;
            }

        }

        public void ConstruirModeloArmonicoEspecificandoDi(double[] vectorDeDatos, out double[] vectorValoresDelModelo, int usarDatosDesde, int usarDatosHasta, double declinacionInicial, ref double r2, ref double aic)
        {
            int numeroDeParametros = 1;
            int tf = usarDatosHasta - usarDatosDesde, ti = usarDatosDesde;
            double produccionInicial = vectorDeDatos[usarDatosDesde], produccionFinal = vectorDeDatos[usarDatosHasta], declinacionInicialLocal;
            double[] vectorLocalValoresDelModelo = new double[usarDatosHasta - usarDatosDesde + 1];

            //Es necesario asegurarse de que el ultimo dato no sea cero, para que no se genere error al realizar el modelo
            
            declinacionInicialLocal = declinacionInicial;
            DarValoresAlModeloArmonico(ref vectorLocalValoresDelModelo, produccionInicial, declinacionInicialLocal);
            HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
            HallarAIC(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out aic, numeroDeParametros);
            vectorValoresDelModelo = vectorLocalValoresDelModelo;
            
        }

        public void ConstruirMejorModeloArmonico(ref REngine motorDeR, string nombreDelVectorDeDatosR, NumericVector datosParaR, double[] vectorDeDatos, out double[] vectorValoresDelModelo, int usarDatosDesde, int usarDatosHasta, out double declinacionInicial, ref double produccionInicial, ref double r2,ref double aic)
        {
            int numeroDeParametros = 2;
            double declinacionInicialLocal;
            double produccionInicialLocal;
            double[] vectorLocalValoresDelModelo = new double[usarDatosHasta - usarDatosDesde + 1];

            NumericVector coeficientesDesdeR = motorDeR.CreateNumericVector(2);

            int numeroDeDatos = usarDatosHasta - usarDatosDesde + 1;
            motorDeR.Evaluate("matriz <- matrix(nrow = length(" + nombreDelVectorDeDatosR + "), ncol = 2)");
            motorDeR.Evaluate("tiempo <- c(1:(length(" + nombreDelVectorDeDatosR + ")))");
            motorDeR.Evaluate("matriz[,1] <- 1");
            motorDeR.Evaluate("matriz[,2] <- tiempo");
            motorDeR.Evaluate("vectorInverso <- 1/(" + nombreDelVectorDeDatosR + ")");
            motorDeR.Evaluate("i=1\nwhile (i < length(vectorInverso))\n{\nif (" + nombreDelVectorDeDatosR + "[i] == 0)\n{\nvectorInverso[i] = 0\n}\ni = i + 1\n}");
            motorDeR.Evaluate("coeficientesArmonico <- solve(t(matriz) %*% matriz) %*% (t(matriz) %*% vectorInverso)");

            coeficientesDesdeR = motorDeR.Evaluate("coeficientesArmonico").AsNumeric();
            produccionInicialLocal = 1/(coeficientesDesdeR[0]);
            declinacionInicialLocal = coeficientesDesdeR[1] / coeficientesDesdeR[0];
            DarValoresAlModeloArmonico(ref vectorLocalValoresDelModelo, produccionInicialLocal, declinacionInicialLocal);
            HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
            HallarAIC(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out aic, numeroDeParametros);

            vectorValoresDelModelo = vectorLocalValoresDelModelo;
            produccionInicial = produccionInicialLocal;
            declinacionInicial = declinacionInicialLocal;
        }

        protected void DarValoresAlModeloArmonico(ref double[] vectorLocalValoresDelModelo, double produccionInicial, double declinacionInicial)
        {
            vectorLocalValoresDelModelo[0] = produccionInicial;
            for (int tiempo = 1; tiempo < vectorLocalValoresDelModelo.Length; tiempo++)
            {
                vectorLocalValoresDelModelo[tiempo] = produccionInicial / (1 + declinacionInicial * tiempo);
            }

        }

        public void PronosticarConModeloArmonico(ref double[] vectorValoresDelModelo, int tiempoAPartirDelCualSePronostica, double produccionInicial, double declinacionInicial)
        {
            int tiempo;
            if (tiempoAPartirDelCualSePronostica == 0)
            {
                vectorValoresDelModelo[0] = produccionInicial;
                for (int i = 1; i < vectorValoresDelModelo.Length; i++)
                {
                    tiempo = i;
                    vectorValoresDelModelo[i] = produccionInicial / (1 + declinacionInicial * tiempo);
                }
            }
            else
            {
                
                for (int i = 0; i < vectorValoresDelModelo.Length; i++)
                {
                    tiempo = tiempoAPartirDelCualSePronostica + i;
                    vectorValoresDelModelo[i] = produccionInicial / (1 + declinacionInicial * tiempo);
                }
            }
        }



        public void ConstruirMejorModeloArima(ref REngine motorDeR, string nombreDelVectorDeDatosR,out string modeloArima, NumericVector datosParaR, double[] vectorDeDatos, out double[] vectorValoresDelModelo, out NumericVector coeficientesArima, int usarDatosDesde, int usarDatosHasta, out double valorP, out double aic, out double r2)
        {
            int numeroDeParametros;
            NumericVector valoresDelModelo;
            double[] vectorLocalValoresDelModelo = new double[usarDatosHasta - usarDatosDesde + 1];
            motorDeR.Evaluate("modeloArima <- auto.arima(" + nombreDelVectorDeDatosR + ")");
            motorDeR.Evaluate("arimaParaPronostico <- forecast(modeloArima)");
            numeroDeParametros = Convert.ToInt32(motorDeR.Evaluate("length(modeloArima$coef)").AsNumeric().First());
            modeloArima = motorDeR.Evaluate("arimaParaPronostico$method").AsCharacter().ToArray().First().Substring(0,12);
            motorDeR.Evaluate("residuos <- arimaParaPronostico$residuals");
            motorDeR.Evaluate("testDeBox <- Box.test(residuos, lag = 12, type = c(\"Ljung-Box\"), fitdf = 0)");
            valorP = Convert.ToDouble(motorDeR.Evaluate("testDeBox$p.value").AsNumeric().First());
            valoresDelModelo = motorDeR.Evaluate(nombreDelVectorDeDatosR + "- residuos").AsNumeric();
            coeficientesArima = motorDeR.Evaluate("arimaParaPronostico$model$coef").AsNumeric();

            aic = Convert.ToInt32(motorDeR.Evaluate("arimaParaPronostico$model$aic").AsNumeric().First());

            for (int i=0;i< vectorLocalValoresDelModelo.Length; i++)
            {
                vectorLocalValoresDelModelo[i] = valoresDelModelo[i];
            }
            vectorValoresDelModelo = vectorLocalValoresDelModelo;
            HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
            vectorValoresDelModelo = vectorLocalValoresDelModelo;
        }

        public void ConstruirModeloArimaEspecificado(ref REngine motorDeR, string nombreDelVectorDeDatosR,out string modeloArima, NumericVector datosParaR, double[] vectorDeDatos, out double[] vectorValoresDelModelo, out NumericVector coeficientesArima, int usarDatosDesde, int usarDatosHasta, out double valorP, out double aic, out double r2, int p, int d, int q)
        {
            int numeroDeParametros;
            NumericVector valoresDelModelo;
            double[] vectorLocalValoresDelModelo = new double[usarDatosHasta - usarDatosDesde + 1];
            motorDeR.Evaluate("modeloArima <- arima(" + nombreDelVectorDeDatosR + ", order = c("+p+","+d+","+q+"))");
            
            numeroDeParametros = Convert.ToInt32(motorDeR.Evaluate("length(modeloArima$coef)").AsNumeric().First());
            modeloArima = "ARIMA(" + p + "," + d + "," + q + ")";
            
            aic = Convert.ToDouble(motorDeR.Evaluate("modeloArima$aic").AsNumeric().First());
            motorDeR.Evaluate("residuos <- modeloArima$residuals");
            motorDeR.Evaluate("testDeBox <- Box.test(residuos, lag = 12, type = c(\"Ljung-Box\"), fitdf = 0)");
            valorP = Convert.ToDouble(motorDeR.Evaluate("testDeBox$p.value").AsNumeric().First());
            valoresDelModelo = motorDeR.Evaluate(nombreDelVectorDeDatosR + "- residuos").AsNumeric();
            coeficientesArima = motorDeR.Evaluate("modeloArima$coef").AsNumeric();

            for (int i = 0; i < vectorLocalValoresDelModelo.Length; i++)
            {
                vectorLocalValoresDelModelo[i] = valoresDelModelo[i];
            }
            vectorValoresDelModelo = vectorLocalValoresDelModelo;
            HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
            vectorValoresDelModelo = vectorLocalValoresDelModelo;
        }

        public void ConstruirArimaConMejorAic(string direccionFuncion, ref REngine motorDeR, string nombreDelVectorDeDatosR, int d, out string modeloArima, NumericVector datosParaR, double[] vectorDeDatos, out double[] vectorValoresDelModelo, out NumericVector coeficientesArima, int usarDatosDesde, int usarDatosHasta, out double valorP, out double aic, out double r2)
        {
            
            int numeroDeParametros, p, q;
            NumericVector valoresDelModelo;
            double[] vectorLocalValoresDelModelo = new double[usarDatosHasta - usarDatosDesde + 1];

            string direccionFuncionParaR = CambiarCaracter(direccionFuncion, @"\", "/");

            motorDeR.Evaluate("source(\""+direccionFuncionParaR+"\")");
            motorDeR.Evaluate("resultadoMejorARIMA <- MejorARIMA(" + nombreDelVectorDeDatosR + ", "+ d +")");
            motorDeR.Evaluate("modeloArima <- resultadoMejorARIMA$modelo");

            numeroDeParametros = Convert.ToInt32(motorDeR.Evaluate("length(modeloArima$coef)").AsNumeric().First());
            p = Convert.ToInt32(motorDeR.Evaluate("resultadoMejorARIMA$p").AsNumeric().First());
            q = Convert.ToInt32(motorDeR.Evaluate("resultadoMejorARIMA$q").AsNumeric().First());
            modeloArima = "ARIMA(" + p + "," + d + "," + q + ")";

            aic = Convert.ToDouble(motorDeR.Evaluate("modeloArima$aic").AsNumeric().First());
            motorDeR.Evaluate("residuos <- modeloArima$residuals");
            motorDeR.Evaluate("testDeBox <- Box.test(residuos, lag = 12, type = c(\"Ljung-Box\"), fitdf = 0)");
            valorP = Convert.ToDouble(motorDeR.Evaluate("testDeBox$p.value").AsNumeric().First());
            valoresDelModelo = motorDeR.Evaluate(nombreDelVectorDeDatosR + "- residuos").AsNumeric();
            coeficientesArima = motorDeR.Evaluate("modeloArima$coef").AsNumeric();

            for (int i = 0; i < vectorLocalValoresDelModelo.Length; i++)
            {
                vectorLocalValoresDelModelo[i] = valoresDelModelo[i];
            }
            vectorValoresDelModelo = vectorLocalValoresDelModelo;
            HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
            vectorValoresDelModelo = vectorLocalValoresDelModelo;
        }

        protected string CambiarCaracter(string original, string aSustituir, string sustituto)
        {
            string izq, der;
            int contador = 0;
            int pos = original.IndexOf(aSustituir);

            if (aSustituir.Length != 0)
            {
                while (pos >= contador)
                {
                    izq = original.Substring(0, pos);
                    der = original.Substring(pos + aSustituir.Length, original.Length - pos - aSustituir.Length);
                    original = izq + sustituto + der;
                    contador = izq.Length + sustituto.Length;
                    pos = original.IndexOf(aSustituir);
                }
            }
            return original;
        }



        public void ConstruirModeloCombinado(ref REngine motorDeR, out string modeloArima, double[] vectorDeDatos, out double[] vectorValoresDelModelo, out NumericVector coeficientesArima, int usarDatosDesde, int usarDatosHasta, out double declinacionInicial, out double n, out double r2, out double aic, out double valorP)
        {
            string nombreDelVectorDeDatosR = "datosParaArima";
            int numeroDeIteraciones = 80;
            double[] vectorModeloHiperbolico;
            ConstruirModeloHiperbolicoAutomatico(vectorDeDatos, out vectorModeloHiperbolico, usarDatosDesde, usarDatosHasta, numeroDeIteraciones, out declinacionInicial, out n, out r2, out aic);
            NumericVector datosParaArima = motorDeR.CreateNumericVector(usarDatosHasta - usarDatosDesde + 1);
            for(int i=0; i<datosParaArima.Length; i++)
            {
                datosParaArima[i] = vectorDeDatos[i + usarDatosDesde] - vectorModeloHiperbolico[i];
            }
            motorDeR.SetSymbol(nombreDelVectorDeDatosR, datosParaArima);

            
            NumericVector valoresDelModeloParteArima;
            double[] vectorLocalValoresDelModelo = new double[usarDatosHasta - usarDatosDesde + 1];
            motorDeR.Evaluate("modeloParteArima <- auto.arima(" + nombreDelVectorDeDatosR + ")");
            motorDeR.Evaluate("arimaDatos <- forecast(modeloParteArima)");
            modeloArima = motorDeR.Evaluate("arimaDatos$method").AsCharacter().ToArray().First().Substring(0, 12);
            motorDeR.Evaluate("residuosParteArima <- arimaDatos$residuals");
            motorDeR.Evaluate("testDeBoxParteArima <- Box.test(residuosParteArima, lag = 12, type = c(\"Ljung-Box\"), fitdf = 0)");
            valorP = Convert.ToDouble(motorDeR.Evaluate("testDeBoxParteArima$p.value").AsNumeric().First());
            valoresDelModeloParteArima = motorDeR.Evaluate(nombreDelVectorDeDatosR + "- residuosParteArima").AsNumeric();
            coeficientesArima = motorDeR.Evaluate("arimaDatos$model$coef").AsNumeric();

            aic = Convert.ToInt32(motorDeR.Evaluate("arimaDatos$model$aic").AsNumeric().First());

            for (int i = 0; i < vectorLocalValoresDelModelo.Length; i++)
            {
                vectorLocalValoresDelModelo[i] = vectorModeloHiperbolico[i] + valoresDelModeloParteArima[i];
            }
            vectorValoresDelModelo = vectorLocalValoresDelModelo;
            HallarR2(vectorDeDatos, vectorLocalValoresDelModelo, usarDatosDesde, usarDatosHasta, out r2);
            vectorValoresDelModelo = vectorLocalValoresDelModelo;
        }

        public void PronosticarConModeloCombinado(bool pronosticoSolo, bool pronosticoTodosLosDatos, int j, REngine motorDeR, string nombreDelVectorDeDatosEnR, ref double[] vectorPronostico, double[] vectorValoresDelModelo,int usarDatosDesde, int usarDatosHasta, int intervalosDeTiempoAPronosticar, int tiempoAPartirDelCualSePronostica, double produccionInicial, double declinacionInicial, double n)
        {
            double[] vectorLocalHiperbólico = new double[vectorPronostico.Length];
            int tiempo;
            for (int i = 0; i < vectorLocalHiperbólico.Length; i++)
            {
                tiempo = i + tiempoAPartirDelCualSePronostica;
                vectorLocalHiperbólico[i] = produccionInicial * Math.Pow(1 + n * declinacionInicial * tiempo, (-1 / n));
            }

            NumericVector prediccionParteArima;
            motorDeR.Evaluate("pronostico <- forecast(modeloParteArima," + intervalosDeTiempoAPronosticar + ")");
            prediccionParteArima = motorDeR.Evaluate("pronostico$mean").AsNumeric();

            if (pronosticoSolo)
            {
                for (int i = 0; i < vectorLocalHiperbólico.Length; i++)
                {
                    vectorPronostico[i] = vectorLocalHiperbólico[i] + prediccionParteArima[i];
                }
            }
            else
            {
                if (pronosticoTodosLosDatos)
                {
                    for (int i = 0; i < usarDatosHasta - usarDatosDesde + 1; i++)
                    {
                        vectorPronostico[i] = vectorValoresDelModelo[i];
                    }
                    for (int i = 0; i < intervalosDeTiempoAPronosticar; i++)
                    {
                        vectorPronostico[i + usarDatosHasta - usarDatosDesde + 1] = vectorLocalHiperbólico[i + usarDatosHasta - usarDatosDesde + 1] + prediccionParteArima[i]; ;
                    }
                }
                else
                {
                    for (int i = 0; i < j; i++)
                    {
                        vectorPronostico[j - 1 - i] = vectorValoresDelModelo[vectorValoresDelModelo.Length -1 - i];
                    }

                    for (int i = 0; i < intervalosDeTiempoAPronosticar; i++)
                    {
                        vectorPronostico[i + j] = vectorLocalHiperbólico[i + j] + prediccionParteArima[i];
                    }

                }
            }

            for (int i = 0; i < vectorLocalHiperbólico.Length; i++)
            {
                if(vectorPronostico[i] < 0)
                {
                    vectorPronostico[i] = 0;
                }
                
            }
        }
        
        public void ConstruirMejorArimaConAicMetodoUtilParaHilos(ref REngine motorDeR, string nombreDelVectorDeDatosR, out string modeloArima, NumericVector datosParaR, double[] vectorDeDatos, out double[] vectorValoresDelModelo, out NumericVector coeficientesArima, int usarDatosDesde, int usarDatosHasta, out double valorP, out double aic, out double r2)
        {
            double[,] matrizAic = new double[10, 10];
            string nombreDelModeloEnR = "modelo";
            MessageBox.Show("Este proceso puede tardar varios minutos", "Continuar...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            int p = 1, q = 1, d = 1;

            List<Thread> threads = new List<Thread>();  
            
            while (p <= 10)
            {
                while (q <= 10)
                {
                    motorDeR.Evaluate("" + nombreDelModeloEnR + " <- auto.arima(" + nombreDelVectorDeDatosR + ", max.p = " + p + ", max.q = " + q + ", start.p = " + p + ", start.q = " + q + ", d=" + d + ")");
                    matrizAic[p-1, q-1] = Convert.ToDouble(motorDeR.Evaluate(nombreDelModeloEnR + "$aic").AsNumeric().First());
                    q++;
                }
                p++;
            }
            p = 1;
            q = 2;
            int pMejorAic = p;
            int qMejorAic = q;
            double mejorAic = matrizAic[0, 0];
            while (p <= 10)
            {
                while (q <= 10)
                {
                    if(matrizAic[p-1, q-1] < mejorAic)
                    {
                        pMejorAic = p;
                        qMejorAic = q;
                        mejorAic = matrizAic[p-1, q-1];
                    }
                }
            }
            ConstruirModeloArimaEspecificado(ref motorDeR, nombreDelVectorDeDatosR, out modeloArima, datosParaR, vectorDeDatos, out vectorValoresDelModelo, out coeficientesArima, usarDatosDesde, usarDatosHasta, out valorP, out aic, out r2, p, d, q);
            
        }
        
    }
}
