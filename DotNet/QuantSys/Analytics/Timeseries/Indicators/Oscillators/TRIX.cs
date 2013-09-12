using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.Analytics.Timeseries.Indicators.Averages;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Oscillators
{
    /// <summary>
    ///     1. Single-Smoothed EMA = 15-period EMA of the closing price
    ///     2. Double-Smoothed EMA = 15-period EMA of Single-Smoothed EMA
    ///     3. Triple-Smoothed EMA = 15-period EMA of Double-Smoothed EMA
    ///     4. TRIX = 1-period percent change in Triple-Smoothed EMA
    /// </summary>
    public class TRIX : AbstractIndicator
    {
        private readonly EMA EMA1;
        private readonly EMA EMA2;
        private readonly EMA EMA3;

        private readonly int _n;

        public TRIX(int n = 15) : base(n)
        {
            EMA1 = new EMA(n);
            EMA2 = new EMA(n);
            EMA3 = new EMA(n);
        }

        public override double HandleNextTick(Tick t)
        {
            double ema1 = EMA1.HandleNextTick(t);
            double ema2 = EMA2.HandleNextTick(ema1);
            double ema3 = EMA3.HandleNextTick(ema2);

            double value = 100 * (ema3 - EMA3[1]) / ema3;
            indicatorData.Enqueue(value);

            return value;
        }

        public override string ToString()
        {
            return "TRIX" + Period;
        }
    }
}