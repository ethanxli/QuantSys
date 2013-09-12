using System;
using QuantSys.MarketData;

namespace QuantSys.TradeEngine.Simulation.Account.Order
{
    public class MarketOrder
    {
        public MarketOrder(Tick t, Position.PositionSide side, double orderSize)
        {
            Symbol = t.Symbol;
            Side = side;
            Price = (side.Equals(Position.PositionSide.Long)) ? t.AskClose : t.BidClose;
            Size = orderSize;
            OrderDate = t.Time;
        }

        public Symbol Symbol { get; set; }
        public Position.PositionSide Side { get; set; }
        public double Price { get; set; }
        public double Size { get; set; }
        public DateTime OrderDate { get; set; }
    }
}