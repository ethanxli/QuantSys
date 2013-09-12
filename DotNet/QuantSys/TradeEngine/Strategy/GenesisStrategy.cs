using System;
using QuantSys.Analytics.Timeseries.Indicators.Averages;
using QuantSys.Analytics.Timeseries.Indicators.Channels;
using QuantSys.Analytics.Timeseries.Indicators.Misc;
using QuantSys.Analytics.Timeseries.Indicators.Transforms;
using QuantSys.MarketData;
using QuantSys.TradeEngine.AccountManagement;
using QuantSys.TradeEngine.Simulation.Account;

namespace QuantSys.TradeEngine.Strategy
{
    public class GenesisStrategy : AbstractStrategy
    {
        private double indNumber;
        private double rindNumber;

        private QSPolyChannel QSP;

        public GenesisStrategy()
        {
            AttachIndicator("GENESIS", new Genesis(40));
            AttachIndicator("RGENESIS", new ReversalGenesis(40));
            AttachIndicator("EMA", new EMA(5));
            AttachIndicator("HVOL", new PercentileRank(80, new HistoricalVol(40)));
            AttachIndicator("ROC", new PercentileRank(400, new ROC(50)));
            AttachIndicator("GDEMA", new GDEMA(252, 0.75));

            QSP = new QSPolyChannel();
            indNumber = Genesis.indNumber;
            rindNumber = ReversalGenesis.indNumber;
        }

        private double STOPLEVEL;
        private double LIMITLEVEL;

        public override void OnTick(params Tick[] t)
        {
            STOPLEVEL = 50 * t[0].BidOpen/10000;
            LIMITLEVEL = 100 * t[0].BidOpen/10000;
            int POSIZE = (int) (((AccountManager) IAccountManager).CurrentBalance);

            QSP.HandleNextTick(t[0]);
            foreach (var indicator in indicatorList)
            {
                indicator.Value.HandleNextTick(t[0]);
            }

            //Check Spread Cost before trading
            if ((t[0].AskClose - t[0].BidClose <= .0005))
            {

                //Volatility high
                if (indicatorList["HVOL"][0] > 90)
                {

                   

                    if (indicatorList["GENESIS"][0] < -1 * (indNumber - 0.5) &&
                        indicatorList["GENESIS"][1] > -1 * (indNumber - 0.5) &&
                        indicatorList["EMA"][0]  < indicatorList["GDEMA"][0])
                    {
                        if (!IAccountManager.ExistsPositionForSymbol(t[0].Symbol))
                        {
                            IAccountManager.PlaceMarketOrder(t[0].Symbol, POSIZE, Position.PositionSide.Short, STOPLEVEL, LIMITLEVEL);
                        }
                    }
                }

            }

            if (IAccountManager.ExistsLongPositionForSymbol(t[0].Symbol))
            {

            }

            if (IAccountManager.ExistsShortPositionForSymbol(t[0].Symbol))
            {

            }


        }
    }
}
