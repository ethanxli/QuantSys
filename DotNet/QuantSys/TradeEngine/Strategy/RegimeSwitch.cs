using QuantSys.Analytics.Timeseries.Indicators.Misc;
using QuantSys.Analytics.Timeseries.Indicators.Oscillators;
using QuantSys.Analytics.Timeseries.Indicators.Transforms;
using QuantSys.MarketData;
using QuantSys.TradeEngine.AccountManagement;
using QuantSys.TradeEngine.Simulation.Account;

namespace QuantSys.TradeEngine.Strategy
{
    public class RegimeSwitch: AbstractStrategy
    {
        public IAccountManager IAccountManager { get; set; }

        private const double VOL = 0.5;
        private const double STOPLEVEL = 0.002;

        public RegimeSwitch()
        {
            AttachIndicator("PRankHistVol", new PercentileRank(252, new HistoricalVol(200)));
            AttachIndicator("MACD", new MACD());
            AttachIndicator("RSI", new UltimateOscillator());
        }

        public override void OnTick(params Tick[] t)
        {

            foreach (var indicator in indicatorList)
            {
                indicator.Value.HandleNextTick(t[0]);
            }

            //Check Spread Cost before trading
            if (t[0].AskClose - t[0].BidClose > STOPLEVEL) return;

            //High Vol. Use Oscillating strat
            if (indicatorList["PRankHistVol"][0] < VOL)
            {
                double RSI = indicatorList["RSI"][0];
                double prevRSI = indicatorList["RSI"][1];

                if (RSI >= 70 && prevRSI < 70)
                {
                    if (IAccountManager.ExistsShortPositionForSymbol(t[0].Symbol))
                    {
                        IAccountManager.ClosePosition(t[0].Symbol);
                        IAccountManager.PlaceMarketOrder(t[0].Symbol, 10000, Position.PositionSide.Long, STOPLEVEL);
                    } else if(!IAccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        IAccountManager.PlaceMarketOrder(t[0].Symbol, 10000, Position.PositionSide.Long, STOPLEVEL);
                    }

                }

                if (RSI <= 30 && prevRSI > 30)
                {
                    if (IAccountManager.ExistsLongPositionForSymbol(t[0].Symbol))
                    {
                        IAccountManager.ClosePosition(t[0].Symbol);
                        IAccountManager.PlaceMarketOrder(t[0].Symbol, 10000, Position.PositionSide.Short, STOPLEVEL);
                    } else if(!IAccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        IAccountManager.PlaceMarketOrder(t[0].Symbol, 10000, Position.PositionSide.Short, STOPLEVEL);
                    }

                }
            }

            //Low Vol. Use Trend following
            else if (indicatorList["PRankHistVol"][0] > VOL) 
            {
                double MACD = indicatorList["MACD"][0];
                double prevMACD = indicatorList["MACD"][1];
                double MACDSignal = ((MACD)indicatorList["MACD"]).SubIndicators["Signal"][0];

                if (MACD > MACDSignal && prevMACD < MACDSignal && MACD<0)
                {
                    if (IAccountManager.ExistsShortPositionForSymbol(t[0].Symbol))
                    {
                        IAccountManager.ClosePosition(t[0].Symbol);
                        IAccountManager.PlaceMarketOrder(t[0].Symbol, 10000, Position.PositionSide.Long, STOPLEVEL);
                    }
                    else if (!IAccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        IAccountManager.PlaceMarketOrder(t[0].Symbol, 10000, Position.PositionSide.Long, STOPLEVEL);
                    }

                }

                if (MACD < MACDSignal && prevMACD > MACDSignal && MACD > 0)
                {
                    if (IAccountManager.ExistsLongPositionForSymbol(t[0].Symbol))
                    {
                        IAccountManager.ClosePosition(t[0].Symbol);
                        IAccountManager.PlaceMarketOrder(t[0].Symbol, 10000, Position.PositionSide.Short, STOPLEVEL);
                    }
                    else if (!IAccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        IAccountManager.PlaceMarketOrder(t[0].Symbol, 10000, Position.PositionSide.Short, STOPLEVEL);
                    }
                }
            }
        
        }
    }
}
