using System;
using QuantSys.Indicators.Abstraction;
using QuantSys.Indicators.Averages;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Oscillators
{

    /// <summary>
    /// AC = AO-SMA(AO, 5)
    /// </summary>
    
    public class AC : AbstractIndicator
    {

        private SMA SMA1;
        private AO AO;

        public AC():this(5){}

        public AC(int n = 5) : base(n)
        {
            SMA1 = new SMA(n);
            AO = new AO();
        }

        public override double HandleNextTick(Tick t)
        {
            double value = double.NaN;
            double aonext = AO.HandleNextTick(t);
            double smanext = SMA1.HandleNextTick(aonext);

            value = aonext - smanext;
            indicatorData.Enqueue(value);
            return value;
        }

        public double HandleNextTick(double d)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "AC" + Period;
        }
    }
}
