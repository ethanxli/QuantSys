using System;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Misc
{
    /// <summary>
    ///  The Heikin-Ashi Close is simply an average of the open, 
    ///   high, low and close for the current period. 
    ///
    ///   HA-Close = (Open(0) + High(0) + Low(0) + Close(0)) / 4
    ///
    ///   2. The Heikin-Ashi Open is the average of the prior Heikin-Ashi 
    ///   candlestick open plus the close of the prior Heikin-Ashi candlestick. 
    ///
    ///   HA-Open = (HA-Open(-1) + HA-Close(-1)) / 2 
    ///
    ///   3. The Heikin-Ashi High is the maximum of three data points: 
    ///   the current period's high, the current Heikin-Ashi 
    ///   candlestick open or the current Heikin-Ashi candlestick close. 
    ///
    ///   HA-High = Maximum of the High(0), HA-Open(0) or HA-Close(0) 
    ///
    ///   4. The Heikin-Ashi low is the minimum of three data points: 
    ///   the current period's low, the current Heikin-Ashi 
    ///   candlestick open or the current Heikin-Ashi candlestick close.
    ///
    ///   HA-Low = Minimum of the Low(0), HA-Open(0) or HA-Close(0) 
    /// </summary>

    public class HeikenAshi : AbstractIndicator
    {
        public MovingQueue<Tick> HACandle;
        private Tick prevTick;
        public Tick GetCandle(int index)
        {
           return (HACandle.Count > index) ? HACandle.ToArray()[HACandle.Count - index - 1] : null;
        }

        public HeikenAshi(int n)
            : base(n)
        {
            HACandle = new MovingQueue<Tick>(n);
            prevTick = null;
        }

        public override double HandleNextTick(Tick t)
        {
            Tick hTick = null;

            if (HACandle.Count.Equals(0))
            {
                hTick = t;
            }

            if (prevTick != null)
            {
                double HABidClose = (t.BidOpen + t.BidHigh + t.BidLow + t.BidClose) / 4;
                double HAAskClose = (t.AskOpen + t.AskHigh + t.AskLow + t.AskClose) / 4;

                double HABidOpen = (prevTick.BidOpen + prevTick.BidClose) / 2;
                double HAAskOpen = (prevTick.AskOpen + prevTick.AskClose) / 2;

                double HABidHigh = Math.Max(t.BidHigh, Math.Max(HABidOpen, HABidClose));
                double HAAskHigh = Math.Max(t.AskHigh, Math.Max(HAAskOpen, HAAskClose));

                double HABidLow = Math.Min(t.BidLow, Math.Min(HABidOpen, HABidClose));
                double HAAskLow = Math.Min(t.AskLow, Math.Min(HAAskOpen, HAAskClose));

                hTick = new Tick(
                    t.CurrentBid, HABidOpen, HABidHigh, HABidLow, HABidClose,
                    t.CurrentAsk, HAAskOpen, HAAskHigh, HAAskLow, HAAskClose,
                    t.Volume, t.Time
                    );
            }

            HACandle.Enqueue(hTick);
            prevTick = hTick;

            return hTick.BidClose;
        }

        public override string ToString()
        {
            return "HeikenAshi";
        }
    }
}
