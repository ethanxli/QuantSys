using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using QuantSys.Analytics.StatisticalModeling;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.Analytics.Timeseries.Indicators.Averages;
using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Channels
{
    /// <summary>
    /// Similar to Kirshenbaum Bands, but uses a polynomial
    /// regression to find least squares instead.
    /// </summary>

    class QSPolyChannel : AbstractChannel
    {
        private EMA EMA;
        private MovingQueue<double> LRValues;
        private double[] X;
        private double STDEV;

        public QSPolyChannel(int n = 20, int l = 30, double dev = 1.5)
            : base(n)
        {
            EMA = new EMA(n);
            LRValues= new MovingQueue<double>(l);
            X = new double[l];
            for (int i = 0; i < X.Count(); i++) X[i] = i;
            STDEV = dev;
        }

        public override void HandleNextTick(Tick t)
        {
            double emaVal = EMA.HandleNextTick(t);
            LRValues.Enqueue(emaVal);

            double[] Y = LRValues.ToArray();

            double stdErr = 0;

            if (Y.Count() == X.Length)
            {
                Vector x_data = new Vector(X);
                Vector y_data = new Vector(LRValues.ToArray());

                var poly = new PolynomialRegression(x_data, y_data, 2);
                for (int i = 0; i < Period; i++)
                {
                    double x = (i);
                    double y = poly.Fit(x);
                    stdErr += Math.Pow(LRValues.ToArray()[i] - y, 2);
                }

                stdErr = Math.Sqrt(stdErr);
            }

            HighData.Enqueue(EMA[0]+STDEV * stdErr);
            MiddleData.Enqueue(EMA[0]);
            LowData.Enqueue(EMA[0] - STDEV * stdErr);
             
        }

        public override string ToString()
        {
            return "QSPoly Bands" + Period;
        }
    }
}
