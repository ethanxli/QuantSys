using QuantSys.Analytics;
using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Averages
{
    public class EMA : AbstractIndicator
    {
        private readonly double alpha;
        private readonly MovingQueue<double> tickdata;
        private int iteration;

        private AbstractIndicator indicator;

        public EMA(int N, AbstractIndicator indicator = null) : base(N)
        {
            alpha = 2.0/(N + 1);
            tickdata = new MovingQueue<double>(N);
            this.indicator = indicator;
        }
        
        public override double HandleNextTick(Tick t)
        {
            if (indicator != null) return HandleNextTick(indicator.HandleNextTick(t));
            return HandleNextTick(t.BidClose);
        }

        public double HandleNextTick(double currentTick)
        {
            double value = double.NaN;

            if (!currentTick.Equals(double.NaN))
            {
                tickdata.Enqueue(currentTick);

                if (tickdata.Count.Equals(tickdata.Capacity))
                {
                    if(indicatorData.Count.Equals(0))
                        value = tickdata.ToArray().Average();
                    else
                        value = currentTick*alpha + this[0]*(1 - alpha);

                    indicatorData.Enqueue(value);
                }

            }

            return value;
        }

        public override string ToString()
        {
            return "EMA" + Period;
        }


        public static double[] CalcEMA(double[] data, int n)
        {
            var result = new double[data.Length];
            double alpha = 2.0/(n + 1);

            double initEMA = 0;
            for (int i = 0; i < n; i++)
            {
                result[i] = double.NaN;
                initEMA += data[i];
            }

            initEMA /= n;

            result[n] = initEMA;

            for (int i = n + 1; i < data.Length; i++)
            {
                result[i] = data[i]*alpha + result[i - 1]*(1 - alpha);
            }

            return result;
        }
    }
}