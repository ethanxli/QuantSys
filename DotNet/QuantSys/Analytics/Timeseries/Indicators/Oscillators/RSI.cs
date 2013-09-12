using QuantSys.Analytics.StatisticalModeling;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Oscillators
{
    public class RSI : AbstractIndicator
    {
        private Tick previousTick;

        private MovingQueue<double> UP;
        private MovingQueue<double> DOWN;
        private double AVGUP;
        private double AVGDOWN;

        public RSI() : this(14)
        {

        }

        public RSI(int n = 14) : base(n)
        {
            UP = new MovingQueue<double>(n);
            DOWN = new MovingQueue<double>(n);
            AVGUP = double.NaN;
            AVGDOWN = double.NaN;
        }

        public override double HandleNextTick(Tick currentTick)
        {
            double value = double.NaN;

            if (previousTick != null)
            {
                if (currentTick.BidClose < previousTick.BidClose)
                {
                    DOWN.Enqueue(previousTick.BidClose - currentTick.BidClose);
                    UP.Enqueue(0);
                }
                else if (currentTick.BidClose > previousTick.BidClose)
                {
                    DOWN.Enqueue(0);
                    UP.Enqueue(currentTick.BidClose - previousTick.BidClose);
                }
            }

            if (DOWN.Count.Equals(DOWN.Capacity))
            {
                AVGUP = UP.ToArray().Average();
                AVGDOWN = DOWN.ToArray().Average();
            }

            value = 100 - 100 / (1 + AVGUP / AVGDOWN);
            previousTick = currentTick;
            
            indicatorData.Enqueue(value);
            return value;
        }


        public override string ToString()
        {
            return "RSI" + Period;
        }

    }
}