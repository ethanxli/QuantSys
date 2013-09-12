using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Averages
{
    public class AdaptiveSmoothing : AbstractIndicator
    {
        private GDEMA[] GDS;
        private AbstractIndicator indicator;

        public AdaptiveSmoothing(int n, double v = 0.7, int t = 3, AbstractIndicator indicator = null)
            : base(n)
        {
            this.indicator = indicator;
            GDS = new GDEMA[t];
            for(int i = 0 ; i < t; i++) GDS[i] = new GDEMA(n, v, indicator);
        }

        public override double HandleNextTick(Tick t)
        {
            if (indicator != null) return HandleNextTick(indicator.HandleNextTick(t));
            return HandleNextTick(t.BidClose);
        }

        public double HandleNextTick(double d)
        {
            double value = double.NaN;

            for (int i = 1; i < GDS.Length; i++)
            {
                value = GDS[i].HandleNextTick(GDS[i - 1].HandleNextTick(d));
            }

            indicatorData.Enqueue(value);
            return value;
        }

        public override string ToString()
        {
            return "T" + GDS.Length;
        }
    }
}
