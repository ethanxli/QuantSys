using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.Indicators.Abstraction;
using QuantSys.Indicators.Averages;
using QuantSys.Indicators.Misc;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Oscillators
{
    /// <summary>
    /// The directional movement index by J. Welles Wilder 
    /// expresses so-called directional movement, from high to 
    /// high, or low to low, as a percentage of true range
    /// </summary>
    public class DI : AbstractIndicator
    {
        private ATR ATR;
        private Tick prevTick;

        private EMA EMA_DMP;
        private EMA EMA_DMN;
        public DI(int n) :base(n)
        {
            subIndicators.Add("DIP", new GenericContainer(n));
            subIndicators.Add("DIM", new GenericContainer(n));
            EMA_DMN = new EMA(n);
            EMA_DMP = new EMA(n);
            ATR = new ATR(n);
        }

        public override double HandleNextTick(Tick t)
        {
            double DMP = 0;
            double DMN = 0;

            if (prevTick != null)
            {
                double upMove = t.BidHigh - prevTick.BidHigh;
                double downMove = prevTick.BidLow - t.BidLow;
                if (upMove > downMove && upMove > 0) DMP = upMove;
                if (downMove > upMove && downMove > 0) DMN = downMove;
            }

            double atr = ATR.HandleNextTick(t);
            double DIP = 100*EMA_DMP.HandleNextTick(DMP)/atr;
            double DIM = 100*EMA_DMN.HandleNextTick(DMN)/atr;

            ((GenericContainer)subIndicators["DIP"]).HandleNextTick(DIP);
            ((GenericContainer)subIndicators["DIM"]).HandleNextTick(DIM);

            prevTick = t;
            indicatorData.Enqueue(double.NaN);
            return double.NaN;
        }

        public override string ToString()
        {
            return "DI" + Period;
        }
    }
}
