using System.Collections.Generic;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.MarketData;
using QuantSys.TradeEngine.AccountManagement;

namespace QuantSys.TradeEngine.Strategy
{
    public abstract class AbstractStrategy
    {
        public const double DEFAULT_LOT_SIZE = 10000;


        public Dictionary<string, AbstractIndicator> indicatorList;
        public Dictionary<string, AbstractChannel> channelList;

        public bool IsLive { get; set; }

        protected IAccountManager IAccountManager;
        //protected IIAccountManager IAccountManager {get;set;}
        public void SetAccountManager(IAccountManager am)
        {
            this.IAccountManager = am;
        }
        public AbstractStrategy()
        {
            indicatorList = new Dictionary<string, AbstractIndicator>();
        }

        protected void AttachIndicator(string indicatorName, AbstractIndicator i)
        {
            indicatorList.Add(indicatorName, i);
        }

        protected void AttachChannel(string indicatorName, AbstractChannel i)
        {
            channelList.Add(indicatorName, i);
        }

        public abstract void OnTick(params Tick[] t);
        
        //public abstract void OnPeek(params Tick[] t);


    }

}