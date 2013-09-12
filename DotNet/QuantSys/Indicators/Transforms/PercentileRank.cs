using System.Linq;
using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Transforms
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
                    Rankvalue = 100 * (data.Data.Count(x => x < tick));
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
