using QuantSys.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Averages
{
    public class GDEMA : AbstractIndicator
    {
        private EMA EMA1;
        private EMA EMA2;

        private AbstractIndicator indicator;
        private double v;

        public GDEMA(int n, double v = 1, AbstractIndicator indicator = null) : base(n)
        {
            EMA1 = new EMA(n);
            EMA2 = new EMA(n);
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

            value = (1 + v) * v1 - (v * v2);
            indicatorData.Enqueue(value);
            return value;
        }

        public override string ToString()
        {
            return "GDEMA" + Period;
        }
    }
}
