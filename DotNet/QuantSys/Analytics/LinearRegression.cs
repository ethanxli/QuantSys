using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.Analytics;

namespace QuantSys.Analytics
{
    public class LinearRegression
    {
        private double A;
        private double B;
        private double _stderr;

        public double X1{ get { return A; }}
        public double X2 { get { return B; } }

        public double STDERR{get { return _stderr; }}

        public LinearRegression()
        {
            A = double.NaN;
            B = double.NaN;
        }

        public void Model(double[] x, double[] y, bool calculateStdErr = true)
        {
            if(x.Length != y.Length)
                throw new InvalidDataException("Arrays X and Y must be same length.");

            A = y.Average();

            double div = 0;
            for (int i = 0; i < y.Length; i++) div += x[i] * y[i];
            div /= x.Sum(xi => Math.Pow(xi, 2));
            B = div;

            //calcualte stderr
            _stderr = 0;
            for (int i = 0; i < y.Length; i++)
            {
                _stderr +=  Math.Pow(y[i] -(A + B*x[i]), 2);
            }
            _stderr /= y.Length;
            _stderr = Math.Sqrt(_stderr);
        }

        public void Model(double[] y, bool calculateStdErr = true)
        {
            double[] x = new double[y.Length];
            int counter = 0;
            for (double i = (double)-y.Length/2 +0.5; i < (double)y.Length/2; i++) x[counter++] = i;
            Model(x, y, calculateStdErr);
        }

        public double Fit(double x)
        {
            return A + B*x;
        }





    }
}
