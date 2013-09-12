using QuantSys.Analytics.StatisticalModeling;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.Analytics.Timeseries.Indicators.Averages;
using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Oscillators
{
    public class CCI : AbstractIndicator
    {
        private SMA SMA;
        private double constant;
        private MovingQueue<double> priceData;

        public CCI(int n = 20, double constant = 0.015) : base(n)
        {
            SMA = new SMA(n);
            priceData = new MovingQueue<double>(n);
            this.constant = constant;
        }

        public override double HandleNextTick(Tick t)
        {
            double value = double.NaN;

            double typicalPrice = (t.BidClose + t.BidHigh + t.BidLow)/3;
            priceData.Enqueue(typicalPrice);
            SMA.HandleNextTick(typicalPrice);

            if(priceData.Capacity.Equals(priceData.Count))
                value = (1/constant)*(typicalPrice - SMA[0])/priceData.ToArray().StandardDeviation();

            indicatorData.Enqueue(value);
            return value;
        }
        public override string ToString()
        {
            return "CCI" + Period;
        }
    }
}
