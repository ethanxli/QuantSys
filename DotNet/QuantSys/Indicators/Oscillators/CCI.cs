using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.Indicators.Averages;
using QuantSys.Analytics;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Oscillators
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
