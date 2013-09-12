using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;

namespace QuantSys.Analytics
{

    public class VarianceRatio
    {
        public double ZScore { get; set; }
        public double VRatio { get; set; }
        public double PValue { get; set; }

        public enum TimeSeriesType
        {
            HOM, //homoskedastic
            HET  //heteroskedastic
        }
        private static double NormCDF(double x)
        {
            return 0;
        }

        public void VarianceRatioTest(double[] data, int lag = 1, TimeSeriesType cor = TimeSeriesType.HOM)
        {
            DenseVector x = new DenseVector(data);

            //Mean
            double mu = x.Mean();

            //Variance for 1st Order Difference
            double s1 = (x.SubVector(lag, x.Count - lag) - x.SubVector(0, x.Count - 2)).Variance();


            double dLag = lag;
            double varvrt = double.NaN;
            switch (cor)
            {
                case TimeSeriesType.HOM:
                {
                    varvrt = 2*(2*dLag - 1)*(dLag - 1)/(3*dLag*x.Count);
                    break;
                }
                case TimeSeriesType.HET:
                {
                    varvrt = 0;
                    double sum2 = 0;
                    for (int j = 0; j < lag; j++)
                    {
                        double sum1a = 0; //(x(j+2:n)-x(j+1:n-1)-mu).^2
                        double sum1b = 0; //(x(2:n-j)-x(1:n-j-1)-mu).^2;
                        double sum1 = sum1a*sum1b;
                        double delta = sum1/(Math.Pow(sum2, 2));
                        varvrt += 0;  //(2*(q(i)-j)/q(i))^2)*delta;
                    }
                    break;
                }
            }

            ZScore = (VRatio - 1)/Math.Sqrt(varvrt);
            PValue = NormCDF(ZScore);

        }
    }
}
