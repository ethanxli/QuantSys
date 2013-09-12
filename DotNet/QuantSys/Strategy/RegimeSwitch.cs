using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.Indicators.Misc;
using QuantSys.Indicators.Oscillators;
using QuantSys.Indicators.Transforms;
using QuantSys.MarketData;
using QuantSys.PortfolioEngine;
using QuantSys.PortfolioEngine.Order;

namespace QuantSys.Strategy
{
    public class RegimeSwitch: AbstractStrategy
    {
        public AccountManager AccountManager { get; set; }

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
                    if (AccountManager.ExistsPositionForSymbol(t[0].Symbol) && AccountManager.Portfolio.Positions[t[0].Symbol].Side.Equals(Position.PositionSide.Short))
                    {
                        AccountManager.CloseOrder(t[0]);
                        AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Long, 10000), STOPLEVEL);
                    } else if(!AccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Short, 10000), STOPLEVEL);
                    }

                }

                if (RSI <= 30 && prevRSI > 30)
                {
                    if (AccountManager.ExistsPositionForSymbol(t[0].Symbol) && AccountManager.Portfolio.Positions[t[0].Symbol].Side.Equals(Position.PositionSide.Long))
                    {
                        AccountManager.CloseOrder(t[0]);
                        AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Short, 10000), STOPLEVEL);
                    } else if(!AccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Long, 10000), STOPLEVEL);
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
                    if (AccountManager.ExistsPositionForSymbol(t[0].Symbol) && AccountManager.Portfolio.Positions[t[0].Symbol].Side.Equals(Position.PositionSide.Short))
                    {
                        AccountManager.CloseOrder(t[0]);
                        AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Long, 10000), STOPLEVEL);
                    }
                    else if (!AccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Long, 10000), STOPLEVEL);
                    }

                }

                if (MACD < MACDSignal && prevMACD > MACDSignal && MACD > 0)
                {
                    if (AccountManager.ExistsPositionForSymbol(t[0].Symbol) && AccountManager.Portfolio.Positions[t[0].Symbol].Side.Equals(Position.PositionSide.Long))
                    {
                        AccountManager.CloseOrder(t[0]);
                        AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Short, 10000), STOPLEVEL);
                    }
                    else if (!AccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Short, 10000), STOPLEVEL);
                    }
                }
            }
        
        }
    }
}
