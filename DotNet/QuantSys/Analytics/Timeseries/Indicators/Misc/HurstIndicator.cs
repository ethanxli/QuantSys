using QuantSys.Analytics.StatisticalModeling;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Misc
{
    public class HurstIndicator : AbstractIndicator
    {
        private MovingQueue<double> priceData; 
        public HurstIndicator(int n) : base(n)
        {
            priceData = new MovingQueue<double>(n);
        }

        public override double HandleNextTick(Tick t)
        {
            priceData.Enqueue(t.BidClose);

            double value = double.NaN;
            if (priceData.Count.Equals(priceData.Capacity))
            {
                value = HurstEstimation.CalculateHurstEstimate(priceData.ToArray());
            }
            indicatorData.Enqueue(value);
            return value;
        }

        public override string ToString()
        {
            return "Hurst";
        }
    }
}
