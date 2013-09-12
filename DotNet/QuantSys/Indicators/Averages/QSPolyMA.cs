using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using QuantSys.Analytics;
using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Averages
{
    public class QSPolyMA : AbstractIndicator
    {

        private readonly MovingQueue<double> tickdata;
        private double[] X;

        private AbstractIndicator indicator;

        public QSPolyMA(int N, AbstractIndicator indicator = null)
            : base(N)
        {
            tickdata = new MovingQueue<double>(N);
            this.indicator = indicator;
            X = new double[N];
            for (int i = 0; i < X.Count(); i++) X[i] = i;
        }
        
        public override double HandleNextTick(Tick t)
        {
            if (indicator != null) return HandleNextTick(indicator.HandleNextTick(t));
            return HandleNextTick(t.BidClose);
        }

        public double HandleNextTick(double currentTick)
        {
            if (!currentTick.Equals(double.NaN))
            {
                double value = double.NaN;
                tickdata.Enqueue(currentTick);

                if (tickdata.Capacity.Equals(tickdata.Count))
                {
                    Vector x_data = new Vector(X);
                    Vector y_data = new Vector(tickdata.ToArray());

                    var poly = new PolynomialRegression(x_data, y_data, 2);
                    value = poly.Fit(Period);
                }
                indicatorData.Enqueue(value);
                return value;
            }

            return double.NaN;
        }


        public override string ToString()
        {
            return "QSPolyMA" + Period;
        }

    }
}
