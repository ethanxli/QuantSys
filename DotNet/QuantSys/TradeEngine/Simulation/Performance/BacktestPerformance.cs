using System.Collections.Generic;

namespace QuantSys.TradeEngine.Simulation.Performance
{
    public class BacktestPerformance
    {
        public bool OutputToConsole { get; set; }
        public bool OutputGraph { get; set; }

        public double StartingBalance { get; set; }
        public double EndingBalance { get; set; }
        public double TotalPerformance { get; set; }

        public double MaxDrawdownPercent { get; set; }
        public double AverageDrawdownPercent { get; set; }

        public double AverageProfit { get; set; }
        public double AverageLoss { get; set; }
        public double MaxProfitTrade { get; set; }
        public double MinProfitTrade { get; set; }
        public double MaxLossTrade { get; set; }
        public double MinLossTrade { get; set; }

        public int TradesClosed { get; set; }
        public int TotalTrades { get; set; }
        public int ProfitTrades { get; set; }
        public int LossTrades { get; set; }
        public int StopsHit { get; set; }

        public double AnnualizedSharpeRatio { get; set; }
        public double AnnualizedSortinoRatio { get; set; }
        public double AnnualizedVolatility { get; set; }

        private List<TradeRecord> _records;

        public BacktestPerformance()
        {
            
        }


        

    }
}
