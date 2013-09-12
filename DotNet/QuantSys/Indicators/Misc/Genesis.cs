using QuantSys.Indicators.Abstraction;
using QuantSys.Indicators.Averages;
using QuantSys.Indicators.Channels;
using QuantSys.Indicators.Oscillators;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Misc
{
    public class Genesis : AbstractIndicator
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

        public const double indNumber = 4;
        public Genesis(int n) : base(n)
        {
            T3 = new AdaptiveSmoothing(n);
            T32 = new AdaptiveSmoothing(5 * n);
            CCI = new CCI(n);
            WILLR = new WilliamsR(n);
            AC = new AC(n);
            Gann = new GannHiLo(n);
            KST = new KST();
            CH = new Chaikin();
            BB = new BollingerBands();
            QChannel = new QSPolyChannel();
            FI = new ForceIndex(30);
            PFE = new PFE(150);
            RWI = new RWI(30);

        }

        public override double HandleNextTick(Tick t)
        {
            PFE.HandleNextTick(t);
            FI.HandleNextTick(t);
            RWI.HandleNextTick(t);
            double value = 0;



            
            if (PFE[0] > 0)
                value++;
            else if (PFE[0] < 0)
                value--;

            if (FI[0] > 0)
                value++;
            else if (FI[0] < 0)
                value--;

            if (RWI.SubIndicators["RWHI"][0] > 1 && RWI.SubIndicators["RWHI"][0] > RWI.SubIndicators["RWLO"][0])
                value++;
            if (RWI.SubIndicators["RWLO"][0] > 1 && RWI.SubIndicators["RWLO"][0] > RWI.SubIndicators["RWHI"][0])
                value--;

            indicatorData.Enqueue(value);
            return value;
        }

        public override string ToString()
        {
            return "Genesis" + Period;
        }
    }
}
