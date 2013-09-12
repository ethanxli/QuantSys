using System;
using System.Collections.Generic;
using QuantSys.MarketData;

namespace QuantSys.TradeEngine.Simulation.Account
{
    public class Portfolio
    {
        public const double STARTING_BALANCE = 10000;
        public const double MARGIN_REQUIREMENT = 0.2;

        public int NumPositions
        {
            get { return _positions.Count; }
        }

        public Position this[Symbol symbol]
        {
            get { return _positions[symbol]; }
        }

        public Dictionary<Symbol, Position> Positions { get { return _positions; } } 

        private Dictionary<Symbol, Position> _positions;

        public Portfolio(double startingBalance = STARTING_BALANCE)
        {
            CurrentBalance = startingBalance;
            StartingBalance = startingBalance;
            CurrentMargin = startingBalance;
            CurrentEquity = startingBalance;
            _positions = new Dictionary<Symbol, Position>();
        }

        public double CurrentEquity { get; set; }
        public double CurrentBalance { get; set; }
        public double CurrentMargin { get; set; }
        public double StartingBalance { get; set; }


        public bool ExistsPositionForSymbol(Symbol s)
        {
            return _positions.ContainsKey(s);
        }

        public bool TakePosition(Symbol s, double price, Position.PositionSide side, double size, DateTime date)
        {
            //Check if we can take the position, based on margin requirement

            //If there's a position open for s, adjust position accordingly

            _positions.Add(s, new Position(s, price, size, side, date));


            return true;
        }

        //Balance is update when positions are closed
        public double ClosePosition(Tick t, double price)
        {
            if (ExistsPositionForSymbol(t.Symbol))
            {
                double changeInBalance = _positions[t.Symbol].ClosePosition(t, price);
                _positions.Remove(t.Symbol);
                AdjustPortfolioBalance(changeInBalance);
                return changeInBalance;
            }
            return 0;
        }

        public bool ReducePosition(Tick t, double size)
        {
            if (ExistsPositionForSymbol(t.Symbol))
            {
                double changeInBalance = _positions[t.Symbol].ReducePosition(t, size);
                AdjustPortfolioBalance(changeInBalance);
                return true;
            }
            return false;
        }

        public bool IncreasePosition(Tick t, double size)
        {
            if (ExistsPositionForSymbol(t.Symbol))
            {
                _positions[t.Symbol].IncreasePosition(t, size);
                return true;
            }
            return false;
        }

        private void AdjustPortfolioBalance(double change)
        {
            CurrentBalance += change;
        }


        //Portfolio Equity and Margin is adjusted on tick
        public void AdjustPortfolioEquityAndMargin(params Tick[] ticks)
        {
            double tempEquity = CurrentBalance;

            foreach (Tick t in ticks)
            {
                if (ExistsPositionForSymbol(t.Symbol))
                {
                    tempEquity += _positions[t.Symbol].Size*
                                  ((_positions[t.Symbol].Side.Equals(Position.PositionSide.Long))
                                      ? t.BidClose - _positions[t.Symbol].PositionPrice
                                      : _positions[t.Symbol].PositionPrice - t.AskClose);
                }
            }

            CurrentEquity = tempEquity;

            //if margin falls below requirement, need to close all positions.
        }
    }
}