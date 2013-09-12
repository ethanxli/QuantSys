using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Misc
{
    public class GenericContainer : AbstractIndicator
    {
        public GenericContainer(int n) : base(n)
        {
            
        }
        public override double HandleNextTick(Tick t)
        {
            return HandleNextTick(t.BidClose);
        }

        public double HandleNextTick(double d)
        {
            indicatorData.Enqueue(d);
            return d;
        }
        public override string ToString()
        {
            return "Container";
        }
    }
}
