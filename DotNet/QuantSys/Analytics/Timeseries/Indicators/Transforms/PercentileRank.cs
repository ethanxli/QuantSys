using System.Linq;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Transforms
{
    public class PercentileRank: AbstractIndicator
    {
        private MovingQueue<double> data;
        private AbstractIndicator indicator;

        public PercentileRank(int n, AbstractIndicator indicator = null) : base(n)
        {
            data = new MovingQueue<double>(n);
            if(indicator!=null) this.indicator = indicator;
        }

        public override double HandleNextTick(Tick t)
        {
            if (indicator != null) return HandleNextTick(indicator.HandleNextTick(t));
            
            return HandleNextTick(t.BidClose);
        }


        public double HandleNextTick(double tick)
        {
            double Rankvalue = double.NaN;

            if (tick != double.NaN)
            {
                data.Enqueue(tick);


                if (indicatorData.Capacity == indicatorData.Count)
                {
                    Rankvalue = 100 * (data.Data.Count(x => x < tick) + 0.5* data.Data.Count(x => x.Equals(tick)));
                    Rankvalue /= Period;
                }
            }

            indicatorData.Enqueue(Rankvalue);
            return Rankvalue;
        }


        public override string ToString()
        {
            return "PercentRank" + Period;
        }
    }
}
