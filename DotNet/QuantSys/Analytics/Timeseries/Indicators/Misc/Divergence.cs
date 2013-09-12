using System.Collections.Generic;
using System.Linq;
using ikvm.extensions;
using QuantSys.Analytics.StatisticalModeling;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.Analytics.Timeseries.Indicators.Averages;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Misc
{
    public class Divergence : AbstractIndicator
    {
        private QSPolyMA MA;
        private AbstractIndicator indicator;

        public Divergence(AbstractIndicator indicator) : base(indicator.Period)
        {
            MA = new QSPolyMA(indicator.Period);
            this.indicator = indicator;
            subIndicators.Add(indicator.toString(), indicator);
        }

        public override double HandleNextTick(Tick t)
        {
            double value = double.NaN;
            MA.HandleNextTick(t);
            indicator.HandleNextTick(t);

            if (!MA[0].Equals(double.NaN))
            {
                double[] MAArray = MA.ToArray();
                double[] indArray = indicator.ToArray();
                SortedList<double, double> tickHighs = new SortedList<double, double>();
                SortedList<double, double> tickLows = new SortedList<double, double>();
                SortedList<double, double> indHighs = new SortedList<double, double>();
                SortedList<double, double> indLows = new SortedList<double, double>();
                for (int i = 2; i < MAArray.Length; i++)
                {
                    //high
                    if (MAArray[i] < MAArray[i - 1] && MAArray[i - 1] > MAArray[i - 2])
                    {
                        tickHighs.Add(i - 1, MAArray[i - 1]);
                    }
                    if (indArray[i] < indArray[i - 1] && indArray[i - 1] > indArray[i - 2])
                    {
                        indHighs.Add(i - 1, indArray[i - 1]);
                    }

                    //low
                    if (MAArray[i] > MAArray[i - 1] && MAArray[i - 1] < MAArray[i - 2])
                    {
                        tickLows.Add(i - 1, MAArray[i - 1]);
                    }
                    if (indArray[i] > indArray[i - 1] && indArray[i - 1] < indArray[i - 2])
                    {
                        indLows.Add(i - 1, indArray[i - 1]);
                    }

                }

                if (tickHighs.Count > 0 && indHighs.Count > 0
                    && tickLows.Count > 0 && indLows.Count > 0)
                {
                    LinearRegression lrTickHighs = new LinearRegression();
                    LinearRegression lrTickLows = new LinearRegression();
                    LinearRegression lrindkHighs = new LinearRegression();
                    LinearRegression lrindLows = new LinearRegression();

                    lrTickHighs.Model(tickHighs.Keys.ToArray(), tickHighs.Values.ToArray());
                    lrTickLows.Model(tickLows.Keys.ToArray(), tickLows.Values.ToArray());
                    lrindkHighs.Model(indHighs.Keys.ToArray(), indHighs.Values.ToArray());
                    lrindLows.Model(indLows.Keys.ToArray(), indLows.Values.ToArray());

                    if (lrTickHighs.X2 > 0 && lrindkHighs.X2 < 0) value = 1;
                    else if (lrTickLows.X2 < 0 && lrindLows.X2 > 0) value = -1;
                    else value = 0;

                    value = lrindkHighs.Fit(indArray[0]);
                }
            }

            indicatorData.Enqueue(value);
            return value;

        }

        public override string ToString()
        {
            return "Divergence" + Period;
        }
    }
}
