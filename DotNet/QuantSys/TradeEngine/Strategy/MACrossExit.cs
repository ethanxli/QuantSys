using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.MarketData;
using QuantSys.PortfolioEngine;
using QuantSys.PortfolioEngine.Order;
using QuantSys.Analytics.Indicators;

namespace QuantSys.Strategy
{
    public class MACrossExit : AbstractStrategy, IStrategy
    {

        private AccountManager _am;
        public AccountManager AccountManager { get { return _am; } set { _am = value; } }

        private ExponentialMovingAverage EMA1 = new ExponentialMovingAverage(5);
        private ExponentialMovingAverage EMA2 = new ExponentialMovingAverage(30);


        public MACrossExit(int p1, int p2)
            : base()
        {
            EMA1 = new ExponentialMovingAverage(p1);
            EMA2 = new ExponentialMovingAverage(p2);
        }

        public void OnTick(params Tick[] t)
        {
            EMA1.HandleNextTick(t[0].AskClose);
            EMA2.HandleNextTick(t[0].AskClose);

            if (EMA1[0] > EMA2[0] && EMA1[1] < EMA2[1] && _am.ExistsPositionForSymbol(t[0].Symbol))
            {
                _am.CloseOrder(t[0]);
                //Console.WriteLine("Close short Trade at " + t[0].AskClose);
            }


        }


    }
}
