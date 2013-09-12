using QuantSys.Analytics.Timeseries.Indicators.Averages;
using QuantSys.Analytics.Timeseries.Indicators.Channels;
using QuantSys.Analytics.Timeseries.Indicators.Misc;
using QuantSys.Analytics.Timeseries.Indicators.Transforms;
using QuantSys.MarketData;
using QuantSys.TradeEngine.Simulation.Account;

namespace QuantSys.TradeEngine.Strategy
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

                if (IAccountManager.ExistsLongPositionForSymbol(t[0].Symbol))
                {

                    if (indicatorList["EMA"][0] < channel.LOW(0))
                        IAccountManager.ClosePosition(t[0].Symbol);

                }
                if (IAccountManager.ExistsShortPositionForSymbol(t[0].Symbol))
                {

                    if (indicatorList["EMA"][0] > channel.HI(0))
                        IAccountManager.ClosePosition(t[0].Symbol);
                }

                //Volatility high
                if (indicatorList["HVOL"][0] > 40)
                {
                    if (!IAccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        if (indicatorList["EMA"][0] > channel.HI(0) && indicatorList["EMA"][1] < channel.HI(1))
                            IAccountManager.PlaceMarketOrder(t[1].Symbol, 10000, Position.PositionSide.Long, STOPLEVEL);
                    }

                    if (!IAccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        if (indicatorList["EMA"][0] < channel.LOW(0) && indicatorList["EMA"][1] > channel.LOW(1))
                            IAccountManager.PlaceMarketOrder(t[1].Symbol, 10000, Position.PositionSide.Short, STOPLEVEL);
                    }

                }

            }

            prevTick = t[0];
        }
    }
}
