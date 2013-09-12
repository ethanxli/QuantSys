using QuantSys.Analytics.Timeseries.Indicators.Oscillators;
using QuantSys.MarketData;
using QuantSys.TradeEngine.AccountManagement;
using QuantSys.TradeEngine.Simulation.Account;

namespace QuantSys.TradeEngine.Strategy
{
    //simple moving average strategy. buy/sell on period1 cross period2

    public class RSIStrat : AbstractStrategy
    {
        public double LongEntry { get; set; }
        public double ShortEntry { get; set; }

        public RSIStrat(int n = 14)
        {
            AttachIndicator("RSI", new RSI(n));
        }

        public IAccountManager IAccountManager
        {
            get; set;
        }


        public override void OnTick(params Tick[] t)
        {
            double rsi1 = indicatorList["RSI"].HandleNextTick(t[0]);
            double prevRSI = ((RSI)indicatorList["RSI"])[1];

            if (rsi1 >= LongEntry && prevRSI < LongEntry)
            {
                if (IAccountManager.ExistsPositionForSymbol(t[0].Symbol))
                {
                    IAccountManager.ClosePosition(t[0].Symbol);
                    IAccountManager.PlaceMarketOrder(t[0].Symbol, 10000, Position.PositionSide.Long, .01);   
                }
            }

            if (rsi1 <= ShortEntry && prevRSI > ShortEntry && !IAccountManager.ExistsPositionForSymbol(t[0].Symbol))
            {
                if (IAccountManager.ExistsPositionForSymbol(t[0].Symbol))
                {
                    IAccountManager.ClosePosition(t[0].Symbol);
                    IAccountManager.PlaceMarketOrder(t[0].Symbol, 10000, Position.PositionSide.Short, .01); 
                }
            }

        }
    }
}