using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.Indicators.Averages;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Channels
{
    public class QSVWChannel : AbstractChannel
    {
        private EMA EMA;
        private MovingQueue<double> LRValues;
        private double[] X;
        private double STDEV;

        public QSVWChannel(int n = 20, int l = 30, double dev = 1.5)
            : base(n)
        {
            EMA = new EMA(n);
            LRValues = new MovingQueue<double>(l);
            X = new double[l];
            for (int i = 0; i < X.Count(); i++) X[i] = i;
            STDEV = dev;
        }

        public override void HandleNextTick(Tick t)
        {

        }

        public override string ToString()
        {
            return "QSPoly Bands" + Period;
        }
    }

}
