using QuantSys.Indicators.Abstraction;
using QuantSys.Indicators.Averages;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Oscillators
{

    /// <summary>
    /// MEDIAN PRICE = (HIGH+LOW)/2 
    /// AO = SMA(MEDIAN PRICE, 5)-SMA(MEDIAN PRICE, 34)
    /// </summary>
    public class AO : AbstractIndicator
    {
        private SMA SMA1;
        private SMA SMA2;

        public AO() : this(5, 34){}

        public AO(int p1 = 5, int p2 = 34) : base(p2)
        {
            SMA1 = new SMA(p1);
            SMA2 = new SMA(p2);
        }

        public override double HandleNextTick(Tick t)
        {
            double value = double.NaN;
            value = SMA1[0] - SMA2[0];
            indicatorData.Enqueue(value);

            double MEDIANPRICE = (t.BidHigh + t.BidLow)/2;
            SMA1.HandleNextTick(MEDIANPRICE);
            SMA2.HandleNextTick(MEDIANPRICE);

            return value;
        }

        public override string ToString()
        {
            return "AO" + Period;
        }
    }
}
