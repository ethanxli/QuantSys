using QuantSys.MarketData;

namespace QuantSys.TradeEngine.Simulation.Account.Order
{
    public class LimitOrder
    {
        public Position.PositionSide Side { get; set; }
        public double TriggerPrice { get; set; }
        public bool Trailing { get; set; }
        public Symbol Symbol { get; set; }

        public double TrailSize { get; set; }

        public LimitOrder(Symbol s, Position.PositionSide side, double price, bool trailing = false, double trailsize = 0)
        {
            Symbol = s;
            TriggerPrice = price;
            Side = side;
            Trailing = trailing;
            TrailSize = trailsize;
        }

        public void RecalculateTrail(Tick t)
        {

        }

        public bool IsBuyLimit()
        {
            return Side.Equals(Position.PositionSide.Long);
        }

        public bool IsSellLimit()
        {
            return Side.Equals(Position.PositionSide.Short);
        }
    }
}
