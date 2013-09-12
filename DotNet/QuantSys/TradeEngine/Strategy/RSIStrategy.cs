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
    //simple moving average strategy. buy/sell on period1 cross period2

    public class RSIStrategy : AbstractStrategy, IStrategy
    {
        
        private AccountManager _am;
        public AccountManager AccountManager { get { return _am; } set { _am = value; } }


        public RSIStrategy() : base()
        {
            AttachIndicator("RSI14", new RSI());
        }


        public void OnTick(params Tick[] t)
        {
            double rsi1 = indicatorList["RSI14"].HandleNextTick(t[0]);
            double prevRSI = ((RSI)indicatorList["RSI14"])[1];

            if (rsi1 >= 70 && prevRSI < 70 && !_am.ExistsPositionForSymbol(t[0].Symbol))
            {
                _am.PlaceOrder(new MarketOrder(t[0], Position.PositionSide.Long, _am.CurrentBalance ));

                Console.WriteLine("Placed Long Trade at " + t[0].AskClose);
            }

            if (rsi1 <= 30 && prevRSI > 30 && !_am.ExistsPositionForSymbol(t[0].Symbol))
            {
                _am.PlaceOrder(new MarketOrder(t[0], Position.PositionSide.Short, _am.CurrentBalance ));

                Console.WriteLine("Placed Short Trade at " + t[0].AskClose);
            }

            if (rsi1 < 65 && prevRSI > 65 && _am.ExistsPositionForSymbol(t[0].Symbol))
            {
                _am.CloseOrder(t[0]);

                Console.WriteLine("Closed Long Trade at " + t[0].AskClose);
            }

            if (rsi1 >30 &&  prevRSI < 30 && _am.ExistsPositionForSymbol(t[0].Symbol))
            {
                _am.CloseOrder(t[0]);

                Console.WriteLine("Closed Short Trade at " + t[0].AskClose);
            }

        }


    }
}
