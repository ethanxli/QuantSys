using System.Collections.Generic;
using QuantSys.MarketData;
using QuantSys.PortfolioEngine;

namespace QuantSys.Strategy
{
    public interface IStrategy
    {
        AccountManager AccountManager { get; set; }
        //Dictionary<string, AbstractIndicator> Indicators { get; }
        void OnTick(params Tick[] t);
    }
}