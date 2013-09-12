using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.Indicators.Averages;
using QuantSys.Indicators.Misc;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Oscillators
{
    public class MACD : AbstractIndicator
    {
        private int N1;
        private int N2;
        private int N3;

        private EMA EMA1;
        private EMA EMA2;

        public MACD() : this(12, 26, 9){}

        public MACD(int N1 = 12, int N2 = 26, int N3 = 9) : base(N2)
        {
            this.N1 = N1;
            this.N2 = N2;
            this.N3 = N3;
            EMA1 = new EMA(N1);
            EMA2 = new EMA(N2);

            subIndicators.Add("Signal", new EMA(N3));
            subIndicators.Add("Histogram", new GenericContainer(N2));
        }

        public override double HandleNextTick(Tick t)
        {
            EMA1.HandleNextTick(t.BidClose);
            EMA2.HandleNextTick(t.BidClose);

            double MACD = EMA1[0] - EMA2[0];
            indicatorData.Enqueue(MACD);
            
            ((EMA)subIndicators["Signal"]).HandleNextTick(MACD);

            double MACDHist = MACD - subIndicators["Signal"][0];
            ((GenericContainer)subIndicators["Histogram"]).HandleNextTick(MACDHist);

            return MACD;
        }

        public override string ToString()
        {
            return "MACD" + N1 + "-" + N2;
        }
    }
}