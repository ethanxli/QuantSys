using System;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Oscillators
{
    public class PFE : AbstractIndicator
    {
        private MovingQueue<double> priceData; 
        public PFE(int n) : base(n)
        {
            priceData = new MovingQueue<double>(n);
        }

        public override double HandleNextTick(Tick t)
        {
            double value = double.NaN;
            priceData.Enqueue(t.BidClose);

            if (priceData.Count.Equals(priceData.Capacity))
            {
                double[] pArray = priceData.ToArray();
                double pN = pArray[0];
                double sign = (t.BidClose > pN) ? 1 : -1;
                double hypot = Hypot(Period - 1, pChange(t.BidClose, pN));

                double sumHypot = 0;
                for (int i = 0; i < priceData.Count - 1; i++)
                {
                    sumHypot += Hypot((double) i + 1, pChange(pArray[i], pArray[i + 1]));
                }
                value = 100* (sign*hypot)/sumHypot;
            }

            indicatorData.Enqueue(value);
            return value;
        }

        public override string ToString()
        {
            return "PFE" + Period;
        }

        private double Hypot(double x, double y)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        private double pChange(double xnew, double xold)
        {
            return 100*(xnew - xold)/xold;
        }
    }
}
