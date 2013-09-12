using System;
using System.Linq;
using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Oscillators
{
    /// <summary>
    /// The Ultimate Oscillator is a momentum oscillator designed to 
    /// capture momentum across three different time frames. The multiple time 
    /// frame objective seeks to avoid the pitfalls of other oscillators. Many 
    /// momentum oscillators surge at the beginning of a strong advance and then 
    /// form bearish divergence as the advance continues. This is because they are 
    /// stuck with one time frame. The Ultimate Oscillator attempts to correct 
    /// this fault by incorporating longer time frames into the basic formula. 
    /// Williams identified a buy signal a based on a bullish divergence and 
    /// a sell signal based on a bearish divergence. 
    /// 
    ///     BP = Close - Minimum(Low or Prior Close).
    ///     TR = Maximum(High or Prior Close)  -  Minimum(Low or Prior Close)
    ///     Average7 = (7-period BP Sum) / (7-period TR Sum)
    ///     Average14 = (14-period BP Sum) / (14-period TR Sum)
    ///     Average28 = (28-period BP Sum) / (28-period TR Sum)
    ///     UO = 100 x [(4 x Average7)+(2 x Average14)+Average28]/(4+2+1)
    /// </summary>
    public class UltimateOscillator : AbstractIndicator
    {
        private readonly MovingQueue<double> BP1;
        private readonly MovingQueue<double> BP2;
        private readonly MovingQueue<double> BP3;
        private readonly MovingQueue<double> TR1;
        private readonly MovingQueue<double> TR2;
        private readonly MovingQueue<double> TR3;
        private Tick prevTick;


        public UltimateOscillator() : this(7, 14, 28)
        {

        }

        public UltimateOscillator(int p1 = 7, int p2 = 14, int p3 = 28):base(p3)
        {
            prevTick = null;
            BP1 = new MovingQueue<double>(p1);
            TR1 = new MovingQueue<double>(p1);
            BP2 = new MovingQueue<double>(p2);
            TR2 = new MovingQueue<double>(p2);
            BP3 = new MovingQueue<double>(p3);
            TR3 = new MovingQueue<double>(p3);

        }

        public override double HandleNextTick(Tick t)
        {
            double value = double.NaN;
            double a1 = BP1.ToArray().Sum() / TR1.ToArray().Sum();
            double a2 = BP2.ToArray().Sum() / TR2.ToArray().Sum();
            double a3 = BP3.ToArray().Sum() / TR3.ToArray().Sum();

            if (BP3.Count == BP3.Capacity)
            {
                value = 100*((4*a1) + (2*a2) + a3)/(4 + 2 + 1);
            }

            if (prevTick != null)
            {
                double TR = Math.Max(t.BidHigh, prevTick.BidClose) - Math.Min(t.BidLow, prevTick.BidClose);
                double BP = t.BidClose - Math.Min(t.BidLow, prevTick.BidClose);

                BP1.Enqueue(BP);
                BP2.Enqueue(BP);
                BP3.Enqueue(BP);

                TR1.Enqueue(TR);
                TR2.Enqueue(TR);
                TR3.Enqueue(TR);
            }

            prevTick = t;
            indicatorData.Enqueue(value);
            return value;
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}