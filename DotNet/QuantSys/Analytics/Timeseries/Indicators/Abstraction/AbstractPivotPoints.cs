using System;
using System.Collections.Generic;
using QuantSys.MarketData;

namespace QuantSys.Analytics.Timeseries.Indicators.Abstraction
{
    public abstract class AbstractPivotPoints
    {
        private Timeframe.Period _period;
        private SortedList<DateTime, Tick> priceData;  
        protected AbstractPivotPoints(Timeframe.Period period)
        {
            _period = period;

            switch (period)
            {
                case Timeframe.Period.T:
                case Timeframe.Period.m1:
                case Timeframe.Period.m15:
                {
                    break;
                }
                case Timeframe.Period.m30:
                case Timeframe.Period.H1:
                case Timeframe.Period.H2:
                case Timeframe.Period.H3:
                case Timeframe.Period.H4:
                case Timeframe.Period.H8:
                {
                    break;
                }
                case Timeframe.Period.D1:
                {
                    break;
                }
                case Timeframe.Period.W1:
                case Timeframe.Period.M1:
                {
                    break;
                }
                default:
                {
                    break;
                }                    

            }
        }



    }
}
