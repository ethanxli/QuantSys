using QuantSys.Analytics.StatisticalModeling;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.Analytics.Timeseries.Indicators.Averages;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Channels
{
    /// <summary>
    /// Middle Band = 20-day simple moving average (SMA)
    /// Upper Band = 20-day SMA + (20-day standard deviation of price x 2) 
    /// Lower Band = 20-day SMA - (20-day standard deviation of price x 2)
    /// </summary>
    class BollingerBands : AbstractChannel
    {
        private SMA MIDDLE;
        public BollingerBands(int n = 20): base(n)
        {
            MIDDLE = new SMA(n);
        }

        public override void HandleNextTick(Tick t)
        {
            MiddleData.Enqueue(MIDDLE.HandleNextTick(t));
            var stdev = MIDDLE.ToArray().StandardDeviation();
            HighData.Enqueue(MIDDLE[0] + stdev * 2);
            LowData.Enqueue(MIDDLE[0] - stdev * 2);    
        }

        public override string ToString()
        {
            return "Bollinger Bands" + Period;
        }
    }
}
