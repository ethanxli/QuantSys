using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Misc
{
    public class MFM : AbstractIndicator //Money Flow Multiplier
    {
        public MFM(int n) : base(n)
        {
            
        }

        public override double HandleNextTick(Tick t)
        {
            return ((t.BidClose - t.BidLow) - (t.BidHigh - t.BidClose))/(t.BidHigh - t.BidLow);
        }


        public override string ToString()
        {
            return "MFM";
        }
    }
}