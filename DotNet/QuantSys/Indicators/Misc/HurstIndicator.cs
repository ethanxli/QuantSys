using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.Analytics;
using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Misc
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
