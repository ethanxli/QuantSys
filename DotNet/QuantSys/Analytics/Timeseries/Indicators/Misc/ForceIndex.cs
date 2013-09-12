using System;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.Analytics.Timeseries.Indicators.Averages;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Misc
{
    public class ForceIndex: AbstractIndicator
    {
        private EMA EMA;
        private Tick prevTick;
        public ForceIndex(int n) : base(n)
        {
            EMA = new EMA(n);
        }
        public override double HandleNextTick(Tick t)
        {
            double value = Double.NaN;

            if (prevTick != null)
            {
                value = EMA.HandleNextTick(t.Volume*(t.BidClose - prevTick.BidClose));
            }

            prevTick = t;
            indicatorData.Enqueue(value);
            return value;
        }

        public override string ToString()
        {
            return "Force Index" + Period;
        }
    }
}
