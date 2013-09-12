using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.SRLevels
{   
    /// <summary>
    /// Pivot Point (P) = (High + Low + Close)/3
    /// Support 1 (S1) = (P x 2) - High
    /// Support 2 (S2) = P  -  (High  -  Low)
    /// Resistance 1 (R1) = (P x 2) - Low
    /// Resistance 2 (R2) = P + (High  -  Low)
    /// </summary>
    public class PivotLevels : AbstractPivotPoints
    {
        public PivotLevels(Timeframe.Period p) : base(p)
        {
            
        }


    }
}
