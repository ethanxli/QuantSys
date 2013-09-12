using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.PortfolioEngine;

namespace QuantSys.Backtester
{
  

    public class TradeRecord
    {
        
        public Symbol Symbol { get; set; }
        public DateTime TransactionDate { get; set; }
        public Position.PositionSide PositionTaken { get; set; }
        public bool IsStop { get; set; }
        public bool IsLimit { get; set; }
        public double TradeSize { get; set; }
        public double TradePrice { get; set; }
        public bool OpenOrClose { get; set; }
        public double PNL { get; set; }


    }
}
