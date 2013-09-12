using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections;
using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Abstraction
{
    public abstract class AbstractIndicator
    {
        public double this[int index]
        {
            get { return (indicatorData.Count > index) ? indicatorData.ToArray()[indicatorData.Count - index - 1] : double.NaN; }
        }

        public int SubIndicatorSize{get { return subIndicators.Count; }}
        public Dictionary<string, AbstractIndicator> SubIndicators { get { return subIndicators; } } 
        protected Dictionary<string, AbstractIndicator> subIndicators;


        protected MovingQueue<double> indicatorData;
        public int Period;

        protected AbstractIndicator(int n)
        {
            subIndicators = new Dictionary<string, AbstractIndicator>();
            indicatorData = new MovingQueue<double>(n);
            Period = n;
        }

        public double[] ToArray()
        {
            return indicatorData.ToArray();
        }

        public abstract double HandleNextTick(Tick t);
        
        public abstract override string ToString();

    }

}
