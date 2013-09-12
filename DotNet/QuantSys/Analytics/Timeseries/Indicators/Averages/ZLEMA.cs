using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Averages
{
    public class ZLEMA: AbstractIndicator
    {
        private EMA EMA;
        private MovingQueue<double> priceData;
 
        private AbstractIndicator indicator;
        public ZLEMA(int n, AbstractIndicator indicator = null) : base(n)
        {
            EMA = new EMA(n);
            this.indicator = indicator;
            priceData = new MovingQueue<double>((n-1)/2);
        }

        public override double HandleNextTick(Tick t)
        {
            if(indicator!=null)
                return HandleNextTick(indicator.HandleNextTick(t));
            return HandleNextTick(t.BidClose);
        }

        public double HandleNextTick(double d)
        {
            priceData.Enqueue(d);

            double value = double.NaN;
            if (priceData.Count.Equals(priceData.Capacity))
            {
                value = d + (d - priceData.ToArray()[0]);
            }
            double returnValue = EMA.HandleNextTick(value);

            indicatorData.Enqueue(returnValue);
            return returnValue;
        }
        public override string ToString()
        {
            return "ZLEMA" + Period;
        }
    }
}
