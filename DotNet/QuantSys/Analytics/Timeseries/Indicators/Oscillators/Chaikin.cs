using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.Analytics.Timeseries.Indicators.Averages;
using QuantSys.Analytics.Timeseries.Indicators.Misc;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Oscillators
{
    /// <summary>
    ///     1. Money Flow Multiplier = [(Close  -  Low) - (High - Close)] /(High - Low)
    ///     2. Money Flow Volume = Money Flow Multiplier x Volume for the Period
    ///     3. ADL = Previous ADL + Current Period's Money Flow Volume
    ///     4. Chaikin Oscillator = (3-day EMA of ADL)  -  (10-day EMA of ADL)
    /// </summary>
    public class Chaikin : AbstractIndicator
    {
        private readonly EMA ADL1;
        private readonly EMA ADL2;
        private readonly MFM MoneyFlowMultiplier;

        private double ADL;

        public Chaikin(int n1 = 3, int n2 = 10) : base(n2)
        {
            ADL1 = new EMA(n1);
            ADL2 = new EMA(n2);
            MoneyFlowMultiplier = new MFM(1);
            
        }
    
        public override double HandleNextTick(Tick t)
        {
            double mfm = MoneyFlowMultiplier.HandleNextTick(t);
            double mfv = mfm*t.Volume;
            ADL += mfv;

            double a1 = ADL1.HandleNextTick(ADL);
            double a2 = ADL2.HandleNextTick(ADL);

            indicatorData.Enqueue(a1 - a2);
            return a1 - a2;
        }

        public override string ToString()
        {
            return "Chaikin Oscillator" + Period;
        }
    }
}