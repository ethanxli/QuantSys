using QuantSys.Indicators.Abstraction;
using QuantSys.Indicators.Averages;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Channels
{
    public class GannHiLo: AbstractIndicator
    {
        public SMA SMAHi;
        public SMA SMALow;
        public GannHiLo(int n) : base(n)
        {
            SMAHi = new SMA(n);
            SMALow = new SMA(n);
        }

        public override double HandleNextTick(Tick t)
        {
            SMAHi.HandleNextTick(t.BidHigh);
            SMALow.HandleNextTick(t.BidLow);

            return SMALow[0];
        }

        public override string ToString()
        {
            return "GannHiLo" + Period;
        }
    }
}
