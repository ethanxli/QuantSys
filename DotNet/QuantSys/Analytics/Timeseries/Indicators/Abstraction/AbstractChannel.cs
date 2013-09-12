using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Abstraction
{
    public abstract class AbstractChannel
    {
        public double HI(int index){
            return (HighData.Count > index) ? HighData.ToArray()[HighData.Count - index - 1] : double.NaN;
        }
        public double LOW(int index)
        {
            return (LowData.Count > index) ? LowData.ToArray()[LowData.Count - index - 1] : double.NaN;
        }
        public double MID(int index)
        {
            return (MiddleData.Count > index) ? MiddleData.ToArray()[MiddleData.Count - index - 1] : double.NaN;
        }

        protected MovingQueue<double> HighData;
        protected MovingQueue<double> MiddleData;
        protected MovingQueue<double> LowData;

        public int Period;
        protected AbstractChannel(int n)
        {
            HighData = new MovingQueue<double>(n);
            MiddleData = new MovingQueue<double>(n);
            LowData = new MovingQueue<double>(n);
            Period = n;
        }

        public abstract void HandleNextTick(Tick t);
        
        public abstract override string ToString();

    }
}
