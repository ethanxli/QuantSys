using System;
using QuantSys.MarketData;

namespace QuantSys.TradeEngine.Simulation.Account
{

    public class Position
    {
        public enum PositionSide
        {
            Long = 1,
            Short = 2
        };

        public bool isLong {get { return Side.Equals(PositionSide.Long); }}
        public bool isShort { get { return Side.Equals(PositionSide.Short); } }

        public PositionSide Side;

        public Symbol Symbol { get; set; }
        public double Size { get; set; }
        public double PositionPrice { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }

        public Position(Symbol symbol, double openPrice, double size, PositionSide side, DateTime openDate)
        {
            Symbol = symbol;
            PositionPrice = openPrice;
            Side = side;
            Size = size;
            OpenDate = openDate;
        }

        public double ClosePosition(Tick t, double price)
        {
            CloseDate = t.Time;

            if (Side.Equals(PositionSide.Long))
                return Size * (price - PositionPrice);
            else
                return Size * (PositionPrice - price);
        }

        public double ReducePosition(Tick t, double size)
        {
            double returnAmount = 0;
            double newSize = this.Size - size;
            switch (Side)
            {
                case PositionSide.Long:
                {
                    returnAmount = size * (t.BidClose - PositionPrice);
                    break;
                }
                case PositionSide.Short:
                {
                    returnAmount = size * (PositionPrice - t.AskClose);
                    break;
                }
            }

            this.Size = newSize;
            return returnAmount;
        }

        public void IncreasePosition(Tick t, double size)
        {
            double newSize = this.Size + size;
            switch (Side)
            {
                case PositionSide.Long:
                    {
                        PositionPrice = (this.Size * PositionPrice + size * t.AskClose)/newSize;
                        break;
                    }
                case PositionSide.Short:
                    {
                        PositionPrice = (this.Size * PositionPrice + size * t.BidClose) / newSize;
                        break;
                    }
            }

            this.Size = newSize;
        }
    }
}