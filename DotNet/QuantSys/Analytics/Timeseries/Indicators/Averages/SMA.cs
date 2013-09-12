using QuantSys.Analytics.StatisticalModeling;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Averages
{
    public class SMA : AbstractIndicator
    {
        private readonly MovingQueue<double> tickdata;
        private AbstractIndicator indicator;

        public SMA(int N, AbstractIndicator indicator = null) : base(N)
        {
            tickdata = new MovingQueue<double>(N);
            this.indicator = indicator;
        }

        public override double HandleNextTick(Tick t)
        {
            if (indicator != null) return HandleNextTick(indicator.HandleNextTick(t));
            return HandleNextTick(t.BidClose);
        }

        public double HandleNextTick(double currentTick)
        {
            if (!currentTick.Equals(double.NaN))
            {
                double value = double.NaN;

                tickdata.Enqueue(currentTick);
                value = tickdata.ToArray().Average();
                indicatorData.Enqueue(value);

                return value;
            }

            return double.NaN;
        }

        public override string ToString()
        {
            return "SMA" + Period;
        }
    }
}