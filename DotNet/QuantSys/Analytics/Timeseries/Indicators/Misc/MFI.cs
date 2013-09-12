using System.Linq;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.DataStructures;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Misc
{
    /// <summary>
    /// The Money Flow Index (MFI) is an oscillator that uses both 
    /// price and volume to measure buying and selling pressure. 
    /// Created by Gene Quong and Avrum Soudack, MFI is also known 
    /// as volume-weighted RSI. MFI starts with the typical price 
    /// for each period. Money flow is positive when the typical 
    /// price rises (buying pressure) and negative when the typical 
    /// price declines (selling pressure). A ratio of positive and 
    /// negative money flow is then plugged into an RSI formula to 
    /// create an oscillator that moves between zero and one hundred. 
    /// As a momentum oscillator tied to volume, the Money Flow Index 
    /// (MFI) is best suited to identify reversals and price extremes 
    /// with a variety of signals. 
    /// 
    ///   * 1. Typical Price = (High + Low + Close)/3
    ///   * 2. Raw Money Flow = Typical Price x Volume
    ///   * 3. Money Flow Ratio = (14-period Positive Money Flow)/(14-period Negative Money Flow)
    ///   * 4. Money Flow Index = 100 - 100/(1 + Money Flow Ratio)
    /// </summary>
     
    
    public class MFI : AbstractIndicator
    {

        private MovingQueue<double> PositiveMF;
        private MovingQueue<double> NegativeMF; 
 
        public MFI(int n = 14) :base(n)
        {
            PositiveMF = new MovingQueue<double>(n);
            NegativeMF = new MovingQueue<double>(n);
        }

        public override double HandleNextTick(Tick t)
        {
            double MFITick = double.NaN;

            if (PositiveMF.Count == Period)
            {
                double MFR = PositiveMF.ToArray().Sum()/NegativeMF.ToArray().Sum();
                MFITick = 100 - 100/(1 + MFR);
            }

            double tPrice = (t.BidHigh + t.BidLow + t.BidClose)/3;
            double rawMoneyFlow = t.Volume*tPrice;

            PositiveMF.Enqueue((t.BidClose >= t.BidOpen) ? rawMoneyFlow : 0);
            NegativeMF.Enqueue((t.BidClose < t.BidOpen) ? rawMoneyFlow : 0);

            indicatorData.Enqueue(MFITick);
            return MFITick;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}