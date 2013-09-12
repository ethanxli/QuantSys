using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.Analytics.Timeseries.Indicators.Averages;
using QuantSys.Analytics.Timeseries.Indicators.Channels;
using QuantSys.Analytics.Timeseries.Indicators.Oscillators;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Misc
{
    public class ReversalGenesis : AbstractIndicator
    {
        private AdaptiveSmoothing T3;
        private AdaptiveSmoothing T32;
        private CCI CCI;
        private WilliamsR WILLR;
        private AC AC;
        private GannHiLo Gann;
        private KST KST;
        private Chaikin CH;
        private BollingerBands BB;
        private QSPolyChannel QChannel;
        private ForceIndex FI;
        private PFE PFE;
        private RWI RWI;
        private UltimateOscillator UO;

        public const double indNumber = 2;
        public ReversalGenesis(int n)
            : base(n)
        {
            T3 = new AdaptiveSmoothing(n);
            T32 = new AdaptiveSmoothing(5 * n);
            CCI = new CCI(n);
            WILLR = new WilliamsR(n);

            Gann = new GannHiLo(n);
            KST = new KST();
            CH = new Chaikin();
            BB = new BollingerBands(n);
            UO = new UltimateOscillator(n,2*n,3*n);

        }

        public override double HandleNextTick(Tick t)
        {
            BB.HandleNextTick(t);
            WILLR.HandleNextTick(t);
            UO.HandleNextTick(t);

            double value = 0;


            if (t.BidClose > BB.HI(0))
                value--;
            else if (t.BidClose < BB.LOW(0))
                value++;

            if (WILLR[0] > -20)
                value--;
            else if (WILLR[0] < -80)
                value++;

            if (UO[0] > 70)
                value--;
            else if (UO[0] < 30)
                value++;



            indicatorData.Enqueue(value);
            return value;
        }

        public override string ToString()
        {
            return "RGenesis" + Period;
        }
    }
}
