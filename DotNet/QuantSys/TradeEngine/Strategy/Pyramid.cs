using System.Collections.Generic;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.MarketData;
using QuantSys.TradeEngine.AccountManagement;

namespace QuantSys.TradeEngine.Strategy
{
    public class Pyramid: AbstractStrategy
    {
        public IAccountManager IAccountManager { get; set; }

        private double UpSizeChange;
        private double DownSizeChange;

        private double priceLimit;

        public Dictionary<string, AbstractIndicator> Indicators { get { return indicatorList; } }

        public Pyramid(double a = .1, double b = .1, double c = .1 / 100)
        {
            UpSizeChange = a;
            DownSizeChange = b;
            priceLimit = c;
        }

        public override void OnTick(params Tick[] t)
        {
            /*
            if (IAccountManager.ExistsPositionForSymbol(t[0].Symbol))
            {
                Position p = IAccountManager.Portfolio[t[0].Symbol];

                //increase long position size by x% if price has increase by y%
                if (p.Side.Equals(Position.PositionSide.Long))
                {
                    if ((t[0].BidClose - p.PositionPrice) / p.PositionPrice > priceLimit)
                    {
                        IAccountManager.IncreasePosition(t[0], p.Size * UpSizeChange);
                    }

                    if ((t[0].BidClose - p.PositionPrice) / p.PositionPrice < -priceLimit)
                    {
                        IAccountManager.ReducePosition(t[0], p.Size * DownSizeChange);
                    }
                }

                //increase short position size by x% if price has decreased by y%
                else if (p.Side.Equals(Position.PositionSide.Short))
                {
                    if ((p.PositionPrice - t[0].AskClose) / p.PositionPrice > priceLimit)
                    {
                        IAccountManager.IncreasePosition(t[0], p.Size * UpSizeChange);
                    }

                    if ((p.PositionPrice - t[0].AskClose) / p.PositionPrice < -priceLimit)
                    {
                        IAccountManager.ReducePosition(t[0], p.Size * DownSizeChange);
                    }
                }

            }*/

        }
    }
}
