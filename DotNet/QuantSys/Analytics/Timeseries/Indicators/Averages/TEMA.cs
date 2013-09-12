using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Averages
{
    public class TEMA : AbstractIndicator
    {
        private EMA EMA1;
        private EMA EMA2;
        private EMA EMA3;

        private AbstractIndicator indicator;

        public TEMA(int n, AbstractIndicator indicator = null)
            : base(n)
        {
            EMA1 = new EMA(n);
            EMA2 = new EMA(n);
            EMA3 = new EMA(n);
            this.indicator = indicator;
        }

        public override double HandleNextTick(Tick t)
        {
            if (indicator != null) return HandleNextTick(indicator.HandleNextTick(t));
            return HandleNextTick(t.BidClose);
        }

        public double HandleNextTick(double d)
        {
            double value = double.NaN;

            double v1 = EMA1.HandleNextTick(d);
            double v2 = EMA2.HandleNextTick(v1);
            double v3 = EMA3.HandleNextTick(v2);

            value = 3 * v1 - 3 * v2 + v3;
            indicatorData.Enqueue(value);
            return value;
        }

        public override string ToString()
        {
            return "TEMA" + Period;
        }
    }
}
