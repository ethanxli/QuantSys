using System;
using QuantSys.Analytics.StatisticalModeling;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Oscillators
{
    //Average True Range
    public class ATR : AbstractIndicator
    {
        private Tick prevTick;
        private MovingQueue<double> TrueRange;
        private int counter;
        public ATR(int n = 14) : base(n)
        {
            TrueRange = new MovingQueue<double>(n);
            counter = 0;
        }

        public override double HandleNextTick(Tick t)
        {
            double ATR = double.NaN;

            if (prevTick != null)
            {
                double m1 = t.BidHigh - t.BidLow;
                double m2 = Math.Abs(t.BidHigh - prevTick.BidClose);
                double m3 = Math.Abs(t.BidLow - prevTick.BidClose);
                double TR = Math.Max(m1, Math.Max(m2, m3));
                TrueRange.Enqueue(TR);
            }
            else
            {
                TrueRange.Enqueue(t.BidHigh - t.BidLow);
            }

            counter++;
            if (counter.Equals(Period))
            {
                ATR = TrueRange.ToArray().Average();
            }
            else if(counter > Period)
            {
                ATR = (this[0]*(Period - 1) + TrueRange.ToArray()[Period-1])/Period;
            }
            
            indicatorData.Enqueue(ATR);
            prevTick = t;
            return ATR;
        }

        public override string ToString()
        {
            return "ATR" + Period;
        }
    }
}