using System.Linq;
using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Oscillators
{
    /// <summary>
    ///     %R = (Highest High - Close)/(Highest High - Lowest Low) * -100
    ///     Lowest Low = lowest low for the look-back period
    ///     Highest High = highest high for the look-back period
    ///     %R is multiplied by -100 correct the inversion and move the decimal.
    /// </summary>
    public class WilliamsR : AbstractIndicator
    {
        private readonly MovingQueue<double> _highData;
        private readonly MovingQueue<double> _lowData;

        public WilliamsR() : this(14)
        {
            
        }

        public WilliamsR(int n = 14):base(n)
        {
            _highData = new MovingQueue<double>(n);
            _lowData = new MovingQueue<double>(n);
        }


        public override double HandleNextTick(Tick t)
        {
            double value = double.NaN;

            _highData.Enqueue(t.BidHigh);
            _lowData.Enqueue(t.BidLow);
            value = (_highData.ToArray().Max() - t.BidClose)/
                    (_highData.ToArray().Max() - _lowData.ToArray().Min())*-100;

            indicatorData.Enqueue(value);

            return value;
        }
        public override string ToString()
        {
            return "Williams%R" + Period;
        }
    }
}