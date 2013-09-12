using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Misc
{
    class Covariance: AbstractMultiSymbolIndicator
    {
        
        public Covariance(int n) : base(n)
        {
            
        }

        public override double HandleNextTicks(Tick[] t)
        {
            foreach (Tick tick in t)
            {
                if (!subIndicators.ContainsKey(tick.Symbol.SymbolString))
                {
                    subIndicators.Add(tick.Symbol.SymbolString, new GenericContainer(Period));
                }


            }
            return 0;

        }

        public override string ToString()
        {
            return "Covariance";
        }
    }
}
