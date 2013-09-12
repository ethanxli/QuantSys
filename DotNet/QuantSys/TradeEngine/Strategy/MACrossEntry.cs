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
    public class MACrossEntry : AbstractStrategy, IStrategy
    {

        private AccountManager _am;
        public AccountManager AccountManager { get { return _am; } set { _am = value; } }

        private ExponentialMovingAverage EMA1 = new ExponentialMovingAverage(5);
        private ExponentialMovingAverage EMA2 = new ExponentialMovingAverage(30);
        private ExponentialMovingAverage EMA3 = new ExponentialMovingAverage(5);
        private ExponentialMovingAverage EMA4 = new ExponentialMovingAverage(30);



        public MACrossEntry(int p1, int p2, int p3, int p4)
            : base()
        {
            EMA1 = new ExponentialMovingAverage(p1);
            EMA2 = new ExponentialMovingAverage(p2);
            EMA3 = new ExponentialMovingAverage(p3);
            EMA4 = new ExponentialMovingAverage(p4);
        }

        public void OnTick(params Tick[] t)
        {
            EMA1.HandleNextTick(t[0].AskClose);
            EMA2.HandleNextTick(t[0].AskClose);
            EMA3.HandleNextTick(t[0].AskClose);
            EMA4.HandleNextTick(t[0].AskClose);

            if (EMA1[0] < EMA2[0] && EMA1[1] > EMA2[1])
            {
                if (_am.ExistsPositionForSymbol(t[0].Symbol)) _am.CloseOrder(t[0]);
                _am.PlaceOrder(new MarketOrder(t[0], Position.PositionSide.Short, 10000));

                //Console.WriteLine("Open Short Trade at " + t[0].AskClose);
            }

            if (EMA3[0] > EMA4[0] && EMA3[1] < EMA4[1] && !_am.ExistsPositionForSymbol(t[0].Symbol))
            {
                if (_am.ExistsPositionForSymbol(t[0].Symbol)) _am.CloseOrder(t[0]);
                _am.PlaceOrder(new MarketOrder(t[0], Position.PositionSide.Long, 10000));

                //Console.WriteLine("Open Short Trade at " + t[0].AskClose);
            }

        }


    }
}
