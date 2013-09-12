using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.Indicators.Averages;
using QuantSys.Indicators.Channels;
using QuantSys.Indicators.Misc;
using QuantSys.Indicators.Transforms;
using QuantSys.MarketData;
using QuantSys.PortfolioEngine;
using QuantSys.PortfolioEngine.Order;
using QuantSys.Strategy;

namespace QuantSys.Strategy
{
    public class ChannelStrategy: AbstractStrategy
    {
        private QSPolyChannel channel;

        private Tick prevTick;

        private const double STOPLEVEL = 0.005;

        public ChannelStrategy()
        {
            channel = new QSPolyChannel();
            AttachIndicator("EMA", new EMA(15));
            AttachIndicator("HVOL", new PercentileRank(100, new HistoricalVol(20)));
        }
        public override void OnTick(params Tick[] t)
        {
            channel.HandleNextTick(t[0]);

            foreach (var indicator in indicatorList)
            {
                indicator.Value.HandleNextTick(t[0]);
            }

            if (prevTick != null)
            {

                if (AccountManager.ExistsLongPositionForSymbol(t[0].Symbol))
                {

                    if (indicatorList["EMA"][0] < channel.LOW(0))
                        AccountManager.CloseOrder(t[0]);

                }
                if (AccountManager.ExistsShortPositionForSymbol(t[0].Symbol))
                {

                    if (indicatorList["EMA"][0] > channel.HI(0))
                        AccountManager.CloseOrder(t[0]);
                }

                //Volatility high
                if (indicatorList["HVOL"][0] > 40)
                {
                    if (!AccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        if (indicatorList["EMA"][0] > channel.HI(0) && indicatorList["EMA"][1] < channel.HI(1))
                            AccountManager.PlaceMarketOrder(
                                new MarketOrder(t[0], Position.PositionSide.Long, 10000), STOPLEVEL);
                    }

                    if (!AccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        if (indicatorList["EMA"][0] < channel.LOW(0) && indicatorList["EMA"][1] > channel.LOW(1))
                            AccountManager.PlaceMarketOrder(
                                new MarketOrder(t[0], Position.PositionSide.Short, 10000), STOPLEVEL);
                    }

                }

            }

            prevTick = t[0];
        }
    }
}
