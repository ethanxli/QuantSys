using QuantSys.Indicators.Abstraction;
using QuantSys.Indicators.Averages;
using QuantSys.Indicators.Misc;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Oscillators
{
    /// <summary>
    ///     RCMA1 = 10-Period SMA of 10-Period Rate-of-Change
    ///     RCMA2 = 10-Period SMA of 15-Period Rate-of-Change
    ///     RCMA3 = 10-Period SMA of 20-Period Rate-of-Change
    ///     RCMA4 = 15-Period SMA of 30-Period Rate-of-Change
    ///     KST = (RCMA1 x 1) + (RCMA2 x 2) + (RCMA3 x 3) + (RCMA4 x 4)
    ///     Signal Line = 9-period SMA of KST
    /// </summary>
    public class KST : AbstractIndicator
    {
        private ROC ROC1;
        private ROC ROC2;
        private ROC ROC3;
        private ROC ROC4;

        private SMA SMA1;
        private SMA SMA2;
        private SMA SMA3;
        private SMA SMA4;

        public KST(int p1 = 10, int p2 = 15, int p3 = 20, int p4 = 30, int n = 10) : base(n)
        {
            ROC1 = new ROC(p1);
            ROC2 = new ROC(p2);
            ROC3 = new ROC(p3);
            ROC4 = new ROC(p4);

            SMA1 = new SMA(p1);
            SMA2 = new SMA(p2);
            SMA3 = new SMA(p3);
            SMA4 = new SMA(p4);

        }


        public override double HandleNextTick(Tick t)
        {
            double r1 = ROC1.HandleNextTick(t);
            double r2 = ROC1.HandleNextTick(t);
            double r3 = ROC1.HandleNextTick(t);
            double r4 = ROC1.HandleNextTick(t);

            double s1 = SMA1.HandleNextTick(r1);
            double s2 = SMA1.HandleNextTick(r2);
            double s3 = SMA1.HandleNextTick(r3);
            double s4 = SMA1.HandleNextTick(r4);

            double KST = (s1*1) + (s2*2) + (s3*3) + (s4*4);

            indicatorData.Enqueue(KST);
            return KST;
        }

        public override string ToString()
        {
            return "KST" + Period;
        }
    }
}