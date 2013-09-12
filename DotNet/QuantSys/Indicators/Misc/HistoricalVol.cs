using System;
using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.Analytics;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Misc
{
    public class HistoricalVol : AbstractIndicator
    {
        private double prevTick;
        private MovingQueue<double> returnsData;
        private MovingQueue<double> volData;
        private int n;

        public HistoricalVol(int n) : base(n)
        {
            returnsData = new MovingQueue<double>(n);
            prevTick = double.NaN;
        }

        public string Indicator { get { return "Historical Vol" + n; }}

        public override double HandleNextTick(Tick t)
        {
            returnsData.Enqueue((t.BidClose-prevTick)/prevTick);
            double value = returnsData.ToArray().StandardDeviation();
            indicatorData.Enqueue(value);

            prevTick = t.BidClose;
            return value;
        }


        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
