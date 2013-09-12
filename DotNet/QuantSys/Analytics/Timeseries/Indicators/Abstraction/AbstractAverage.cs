using System.Collections.Generic;
using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Abstraction
{
    public abstract class AbstractAverage
    {

        public double this[int index]
        {
            get { return (averageData.Count > index) ? averageData.ToArray()[averageData.Count - index - 1] : double.NaN; }
        }


        protected MovingQueue<double> averageData;
        protected AbstractIndicator indicator;

        public int Period;

        protected AbstractAverage(int n)
        {
            averageData = new MovingQueue<double>(n);
            Period = n;
        }

        public double[] ToArray()
        {
            return averageData.ToArray();
        }

        public abstract double HandleNextTick(Tick t);

        //public abstract double Peek(Tick t);
        public abstract override string ToString();

    }
}
