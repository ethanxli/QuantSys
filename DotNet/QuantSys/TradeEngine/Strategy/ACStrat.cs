using System.Linq;
using QuantSys.Analytics.Timeseries.Indicators.Oscillators;
using QuantSys.MarketData;
using QuantSys.TradeEngine.Simulation.Account;

namespace QuantSys.TradeEngine.Strategy
{
    public class ACStrat: AbstractStrategy
    {
        public ACStrat()
        {
            AttachIndicator("AC0", new AC(200, 50, 100));
            AttachIndicator("SMA", new RWI(50));
        }
        public override void OnTick(params Tick[] t)
        {
            foreach (var indicator in indicatorList)
            {
                indicator.Value.HandleNextTick(t[0]);
            }

            if (!IsLive) return;
            
                
            for (int i = 0; i < t.Count(); i++)
            {
                if (indicatorList["AC" + i][0] < 0)
                {
                    if (IAccountManager.ExistsLongPositionForSymbol(t[i].Symbol))
                    {
                        IAccountManager.ClosePosition(t[i].Symbol);
                    }
                }

                if (indicatorList["AC" + i][0] > 0)
                {
                    if (IAccountManager.ExistsShortPositionForSymbol(t[i].Symbol))
                    {
                        IAccountManager.ClosePosition(t[i].Symbol);
                    }
                }


                if (!(t[i].AskClose - t[i].BidClose < .0003)) return;


                if (indicatorList["AC" + i][0] > 0 && indicatorList["AC" + i][1] < 0 && t[0].BidClose > indicatorList["SMA"][0])
                {
                    if (!IAccountManager.ExistsPositionForSymbol(t[i].Symbol))
                    {
                        IAccountManager.PlaceMarketOrder(t[1].Symbol, 10000, Position.PositionSide.Long, .01);
                    }
                }

                if (indicatorList["AC" + i][0] < 0 && indicatorList["AC" + i][1] > 0 && t[0].BidClose < indicatorList["SMA"][0])
                {
                    if (!IAccountManager.ExistsPositionForSymbol(t[i].Symbol))
                    {
                        IAccountManager.PlaceMarketOrder(t[1].Symbol, 10000, Position.PositionSide.Short, .01);
                    }
                }


                
            }



        }
    }
}
