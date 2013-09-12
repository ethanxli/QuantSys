using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.Indicators.Misc;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Oscillators
{
    //Random Walk Index
    public class RWI : AbstractIndicator
    {
        private MovingQueue<double> TR;
        private MovingQueue<Tick> priceData;
        private Tick prevTick = null;

        public RWI(int n) : base(n)
        {
            subIndicators.Add("RWHI", new GenericContainer(n));
            subIndicators.Add("RWLO", new GenericContainer(n));
            TR = new MovingQueue<double>(n);
            priceData = new MovingQueue<Tick>(n);
        }
        public override double HandleNextTick(Tick t)
        {
            double value = double.NaN;
            double RWIH = double.NaN;
            double RWIL = double.NaN;

            if (prevTick != null)
            {
                double m1 = t.BidHigh - t.BidLow;
                double m2 = Math.Abs(t.BidHigh - prevTick.BidClose);
                double m3 = Math.Abs(t.BidLow - prevTick.BidClose);
                double tr = Math.Max(m1, Math.Max(m2, m3));
                TR.Enqueue(tr);
            }
            else
            {
                TR.Enqueue(t.BidHigh - t.BidLow);
            }

            if (priceData.Count.Equals(priceData.Capacity))
            {
                List<double> maxRWIHI = new List<double>(Period - 1);
                List<double> maxRWILO = new List<double>(Period - 1);

                for (int k = 0; k < Period - 1; k++)
                {
                    double avgTR = 0;
                    for (int i = k; i < Period; i++) avgTR += TR.ToArray()[i];
                    avgTR /= (Period - k);

                    double rwiHI = 1/Math.Sqrt(Period-k)*(t.BidHigh - priceData.ToArray()[k].BidLow)/avgTR;
                    double rwiLO = 1/Math.Sqrt(Period-k)*(priceData.ToArray()[k].BidHigh - t.BidLow)/avgTR;

                    maxRWIHI.Add(rwiHI);
                    maxRWILO.Add(rwiLO);
                }
                RWIH = maxRWIHI.Max();
                RWIL = maxRWILO.Max();

            }
            ((GenericContainer)subIndicators["RWHI"]).HandleNextTick(RWIH);
            ((GenericContainer)subIndicators["RWLO"]).HandleNextTick(RWIL);

            prevTick = t;
            priceData.Enqueue(t);

            indicatorData.Enqueue(double.NaN);
            return double.NaN;
        }

        public override string ToString()
        {
            return "RWI" + Period;
        }
    }
}
