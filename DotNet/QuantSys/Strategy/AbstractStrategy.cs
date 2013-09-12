using System.Collections.Generic;
using QuantSys.Indicators.Abstraction;
using QuantSys.MarketData;
using QuantSys.PortfolioEngine;

namespace QuantSys.Strategy
{
    public abstract class AbstractStrategy
    {
        public const double DEFAULT_LOT_SIZE = 10000;

        public Dictionary<string, AbstractIndicator> indicatorList;
        public Dictionary<string, AbstractChannel> channelList;

        protected AccountManager AccountManager { get; set; }

        public void SetAccountManager(AccountManager am)
        {
            this.AccountManager = am;
        }
        public AbstractStrategy()
        {
            indicatorList = new Dictionary<string, AbstractIndicator>();
        }

        public void AttachIndicator(string indicatorName, AbstractIndicator i)
        {
            indicatorList.Add(indicatorName, i);
        }

        public void AttachChannel(string indicatorName, AbstractChannel i)
        {
            channelList.Add(indicatorName, i);
        }

        public abstract void OnTick(params Tick[] t);
       
    }
}