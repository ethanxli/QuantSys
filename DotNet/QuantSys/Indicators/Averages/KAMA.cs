using System;
using System.Linq;
using Encog.App.Quant.Indicators;
using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Indicators.Averages
{
    //Kaufman's Adaptive Moving Average (KAMA)
    /// <summary>
    /// KAMA is calculated by the formula:
    ///  KAMA_{i} = KAMA_{i-1} + sc \times {Price - KAMA_{i-1}}
    ///  where:
    /// operatorname{KAMA_{i}} is the value of KAMA in the current period.
    /// operatorname{KAMA_{i—1}} is the value of KAMA in the previous period.
    /// operatorname{Price} is the price in the current period.
    /// operatorname{sc} is the smoothing constant calculated each period by the formula:
    /// sc_{i} = (ER_{i} \times {(fastest - slowest) + slowest})^2
    /// and
    /// fastest = \dfrac{2}{\text {Fastest MA Period} + 1}  
    /// slowest = \dfrac{2}{\text {Slowest MA period} + 1}
    /// ER_{i} = \dfrac{|Price_{t} - Price_{t-n}|}{\sum_{i}^{i-n} |Price_{t} - Price_{t-1}|}
    /// http://fxcodebase.com/wiki/index.php/Kaufman's_Adaptive_Moving_Average_(KAMA)
    /// </summary>
    public class KAMA : AbstractIndicator
    {
        private MovingQueue<double> priceData;
        private AbstractIndicator indicator;

        
        public KAMA(int n, AbstractIndicator indicator = null) : base(n)
        {
            priceData = new MovingQueue<double>(n);
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

            if (!d.Equals(double.NaN))
            {
                priceData.Enqueue(d);

                if (priceData.Count.Equals(priceData.Capacity))
                {
                    double sum = 0;
                    double[] array = priceData.ToArray();
                    for (int i = 1; i < array.Length; i++)
                    {
                        sum += Math.Abs(array[i] - array[i - 1]);
                    }
                    double ER = Math.Abs(d - priceData.ToArray()[0])/sum;

                    double alpha = Math.Pow(ER*0.6015 + 0.0645, 2);

                    if (indicatorData.Count.Equals(0))
                        value = priceData.Average();
                    else
                        value = alpha*d + (1 - alpha)*this[1];

                    indicatorData.Enqueue(value);
                }
            }

            return value;
        }
        public override string ToString()
        {
            return "KAMA" + Period;
        }
    }
}
