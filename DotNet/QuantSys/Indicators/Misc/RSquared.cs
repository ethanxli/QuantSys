using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.Analytics;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Misc
{
    public class RSquared : AbstractIndicator
    {
        private DenseVector Y;
        private MovingQueue<double> X;
        private AbstractIndicator indicator;

        public RSquared(int n, AbstractIndicator indicator = null) : base(n)
        {
            X = new MovingQueue<double>(n);
            Y = new DenseVector(n);
            int counter = 0;
            for (double i = (double)-Y.Count / 2 + 0.5; i < (double)Y.Count / 2; i++) Y[counter++] = i;
            this.indicator = indicator;
        }

        public override double HandleNextTick(Tick t)
        {
            if (indicator != null) return HandleNextTick(indicator.HandleNextTick(t));
            return HandleNextTick(t.BidClose);
        }

        public double HandleNextTick(double d)
        {
            double value = double.NaN;

            X.Enqueue(d);

            if (X.Count.Equals(X.Capacity))
            {
                double cov = StatisticsExtension.Covariance(new DenseVector(X.ToArray()), Y);
                value = cov / (X.ToArray().Variance() * Y.Variance());
            }

            indicatorData.Enqueue(value);
            return value;
        }
        public override string ToString()
        {
            return "RSquared" + Period;
        }
    }
}
