using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.Analytics.Timeseries.Indicators.Averages;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Oscillators
{

    /// <summary>
    /// AC = AO-SMA(AO, 5)
    /// </summary>
    
    public class AC : AbstractIndicator
    {

        private SMA SMA;
        private AO AO;

        public AC():this(5){}

        public AC(int n = 5, int y = 5, int z = 34) : base(n)
        {
            SMA = new SMA(n);
            AO = new AO(y, z);
        }

        public override double HandleNextTick(Tick t)
        {
            double value = double.NaN;
            double aonext = AO.HandleNextTick(t);
            double smanext = SMA.HandleNextTick(aonext);

            value = aonext - smanext;
            indicatorData.Enqueue(value);
            return value;
        }
        
        public override string ToString()
        {
            return "AC" + Period;
        }
    }
}
