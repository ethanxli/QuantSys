using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Misc
{
    public class ROC : AbstractIndicator
    {
        private readonly MovingQueue<double> tickData;
        private AbstractIndicator indicator;
        public ROC(int n, AbstractIndicator indicator = null) : base(n)
        {
            tickData = new MovingQueue<double>(n);
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
            tickData.Enqueue(d);
            if (tickData.Count >= Period)
                value = (d - tickData.ToArray()[1]) / tickData.ToArray()[1];

            indicatorData.Enqueue(value);
            return value;            
        }

        public override string ToString()
        {
            return "ROC" + Period;
        }
    }
}