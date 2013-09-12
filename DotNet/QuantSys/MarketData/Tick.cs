using System;
using QuantSys.PortfolioEngine;

namespace QuantSys.MarketData
{
    public class Tick
    {
        #region members

        public double CurrentAsk { get; set; }
        public double AskOpen { get; set; }
        public double AskHigh { get; set; }
        public double AskLow { get; set; }
        public double AskClose { get; set; }

        public double CurrentBid { get; set; }
        public double BidOpen { get; set; }
        public double BidHigh { get; set; }
        public double BidLow { get; set; }
        public double BidClose { get; set; }

        public double Volume { get; set; }
        public DateTime Time { get; set; }

        public Symbol Symbol { get; set; }
        public Timeframe Period { get; set; }

        #endregion

        public Tick(
            double currentbid = double.NaN,
            double bidopen = double.NaN,
            double bidhigh = double.NaN,
            double bidlow = double.NaN,
            double bidclose = double.NaN,
            double currentask = double.NaN,
            double askopen = double.NaN,
            double askhigh = double.NaN,
            double asklow = double.NaN,
            double askclose = double.NaN,
            double volume = double.NaN,
            DateTime dateTime = new DateTime()
            )
        {
            CurrentBid = currentbid;
            BidOpen = bidopen;
            BidHigh = bidhigh;
            BidLow = bidlow;
            BidClose = bidclose;
            CurrentAsk = currentask;
            AskOpen = askopen;
            AskHigh = askhigh;
            AskLow = asklow;
            AskClose = askclose;
            Volume = volume;
            Time = dateTime;
        }
    }
}