using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Misc
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
