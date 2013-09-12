using QuantSys.Analytics.WaveTransformer;
using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Transforms
{
    public class DWT: AbstractIndicator
    {
        private MovingQueue<double> priceData;
        private SignalTransform st;
        private int detailLevel;
        private AbstractIndicator indicator;

        public DWT(int n, int detailLevel, AbstractIndicator indicator = null) : base(n)
        {
            priceData = new MovingQueue<double>(n);    
            st = new SignalTransform();
            this.detailLevel = detailLevel;
            this.indicator = indicator;
        }

        public override double HandleNextTick(Tick t)
        {
            if (indicator != null) return HandleNextTick(indicator.HandleNextTick(t));
            return HandleNextTick(t.BidClose);
        }

        public double HandleNextTick(double d)
        {
            double returnValue = double.NaN;

            priceData.Enqueue(d);

            if (priceData.Count.Equals(priceData.Capacity))
            {
                returnValue = st.Run(priceData.ToArray(), detailLevel);
                indicatorData.Enqueue(returnValue);
            }

            return returnValue;            
        }
        public override string ToString()
        {
            return "DWT";
        }
    }
}
