using System;
using System.Collections.Generic;
using QuantSys.Backtester;
using QuantSys.MarketData;
using QuantSys.PortfolioEngine.Order;
using QuantSys.Visualization.Highstocks;

namespace QuantSys.PortfolioEngine
{
    /// <summary>
    /// AccountManager places orders, checks stops/limits and
    /// calculates portfolio statistics.
    /// </summary>
    public class AccountManager
    {
        public Portfolio Portfolio{ get { return _portfolio; }}
        public List<HighstockFlag> Flags{get { return _flags; }} 
        public List<double> Trades{get { return _tradeProfits; }}
        public double CurrentBalance { get { return _portfolio.CurrentBalance; } }
        public double CurrentEquity { get { return _portfolio.CurrentEquity;  }}

        private BacktestPerformance _performance;

        private readonly Portfolio _portfolio;

        private Dictionary<Symbol, StopOrder> _outStandingStopOrders;
        private Dictionary<Symbol, LimitOrder> _outStandingLimitOrders;

        private List<HighstockFlag> _flags;
        private List<double> _tradeProfits;
 
        public AccountManager()
        {
            _portfolio = new Portfolio();
            _flags = new List<HighstockFlag>();
            _outStandingStopOrders = new Dictionary<Symbol, StopOrder>();
            _outStandingLimitOrders = new Dictionary<Symbol, LimitOrder>();
            _tradeProfits = new List<double>();
            _performance = new BacktestPerformance();
        }

        #region StopOrder Logic

        public StopOrder GetStopOrder(Symbol s)
        {
            if (_outStandingStopOrders.ContainsKey(s))
            {
                return _outStandingStopOrders[s];
            }

            return null;
        }
        public void PlaceStopOrder(StopOrder stop)
        {
            _outStandingStopOrders.Add(stop.Symbol, stop);
        }
        public void ModifyStopOrder(Tick stop, double newPrice)
        {
            if (_outStandingStopOrders.ContainsKey(stop.Symbol))
            {
                _outStandingStopOrders[stop.Symbol].TriggerPrice = newPrice;
            }

            
            //Flags for visualization purposes
            _flags.Add(new HighstockFlag(
                   "MS",""+newPrice, stop.Time
                ));
           
        }
        private void CloseStopOrder(Tick t, StopOrder s)
        {
            _tradeProfits.Add(_portfolio.ClosePosition(t, s.TriggerPrice));

            _outStandingStopOrders.Remove(s.Symbol);


            //remove limit order because stop was closed
            if(_outStandingLimitOrders.ContainsKey(t.Symbol))
                _outStandingLimitOrders.Remove(t.Symbol);


            //Flags for visualization purposes
            _flags.Add(new HighstockFlag(
                                "CS",
                                "Closed Stop Order",
                                t.Time
                            ));
        }

        public void ModifyTrailingStop(Tick stop, double trailSize)
        {
            if (_outStandingStopOrders.ContainsKey(stop.Symbol))
            {
                _outStandingStopOrders[stop.Symbol].Trailing = true;
                _outStandingStopOrders[stop.Symbol].TrailSize = trailSize;
            }
        }
        #endregion


        #region LimitOrder Logic

        public LimitOrder GetLimitOrder(Symbol s)
        {
            if (_outStandingLimitOrders.ContainsKey(s))
            {
                return _outStandingLimitOrders[s];
            }

            return null;
        }

        public void PlaceLimitOrder(LimitOrder limit)
        {
            _outStandingLimitOrders.Add(limit.Symbol, limit);
        }

        public void ModifyLimitOrder(Tick limit, double newPrice)
        {
            if (_outStandingLimitOrders.ContainsKey(limit.Symbol))
            {
                _outStandingLimitOrders[limit.Symbol].TriggerPrice = newPrice;
            }            
        }

        private void CloseLimitOrder(Tick t, LimitOrder l)
        {
            _tradeProfits.Add(_portfolio.ClosePosition(t, l.TriggerPrice));

            _outStandingLimitOrders.Remove(l.Symbol);

            //remove stop order because limit was closed
            if (_outStandingStopOrders.ContainsKey(t.Symbol))
                _outStandingStopOrders.Remove(t.Symbol);


            //Flags for visualization purposes
            _flags.Add(new HighstockFlag(
                                "CL",
                                "Closed Limit Order",
                                t.Time
                            ));
        }

        private void ModifyTrailingLimit(double trailSize)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void PlaceMarketOrder(MarketOrder order, double stopEntry = double.NaN, double limitEntry = double.NaN)
        {

            //------------------------------------
            // Take the position.
            //------------------------------------

            _portfolio.TakePosition(order.Symbol, order.Price, order.Side, order.Size, order.OrderDate);

            //------------------------------------
            // Place Stop/Limits if defined
            //------------------------------------

            if (!stopEntry.Equals(double.NaN))
            {
                StopOrder stopOrder = new StopOrder(order.Symbol, order.Side, ((order.Side.Equals(Position.PositionSide.Long)) ? order.Price - stopEntry : order.Price + stopEntry));
                PlaceStopOrder(stopOrder);
            }

            if (!limitEntry.Equals(double.NaN))
            {
                LimitOrder limitOrder = new LimitOrder(order.Symbol, order.Side, ((order.Side.Equals(Position.PositionSide.Long)) ? order.Price + limitEntry : order.Price - limitEntry));
                PlaceLimitOrder(limitOrder);
            }

            //------------------------------------
            // Flags for visualization purposes
            //------------------------------------

            _flags.Add(new HighstockFlag(
                    (order.Side==Position.PositionSide.Long)?"B":"S",
                    ((order.Side == Position.PositionSide.Long) ? "Bought " : "Sold ") + order.Side+" at "+ order.Price + " on "+order.OrderDate.ToString(),
                    order.OrderDate
                ));

            //Console.WriteLine(order.OrderDate.ToString() + "ORDER--------");
        }

        public void IncreasePosition(Tick t, double size)
        {
            _portfolio.IncreasePosition(t, size);
        }

        public void ReducePosition(Tick t, double size)
        {
            _portfolio.ReducePosition(t, size);
        }

        public void CloseOrder(Tick t)
        {
            _tradeProfits.Add(_portfolio.ClosePosition(t,
                (_portfolio.Positions[t.Symbol].isLong?t.BidClose:t.AskClose)));

            //Remove Stops and Limits for position
            if (_outStandingStopOrders.ContainsKey(t.Symbol))
                _outStandingStopOrders.Remove(t.Symbol);

            if (_outStandingLimitOrders.ContainsKey(t.Symbol))
                _outStandingLimitOrders.Remove(t.Symbol);

            //Flags for visualization purposes
            _flags.Add(new HighstockFlag(
                                "C",
                                "Closed Order",
                                t.Time
                            ));
        }


        public void OnTick(params Tick[] ticks)
        {
  
        //-----------------------------------------------------------
        // On each tick, we must check for stop and limit order hits.
        //-----------------------------------------------------------

            foreach (Tick t in ticks)
            {

                //------------------------------------------------------
                // Stop Order Check
                //------------------------------------------------------

                if (_outStandingStopOrders.ContainsKey(t.Symbol))
                {
                    StopOrder stopOrder = _outStandingStopOrders[t.Symbol];

                    //If trailing, need to relcalculate trigger price.
                    if(stopOrder.Trailing) stopOrder.RecalculateTrail(t);

                    //Trigger stop if price drops -below- Buy Stop Trigger
                    if (stopOrder.IsBuyStop())
                    {
                        //net stop to close all
                        if (t.BidClose <= stopOrder.TriggerPrice)
                        {
                            CloseStopOrder(t, stopOrder);
                        }
                    }

                    //Trigger stop if price rises -above- Sell Stop Trigger
                    else if (stopOrder.IsSellStop())
                    {
                        if (t.AskClose >= stopOrder.TriggerPrice)
                        {
                            CloseStopOrder(t, stopOrder);

                        }
                    }
                }


                //------------------------------------------------------
                // Limit Order Check
                //------------------------------------------------------

                if (_outStandingLimitOrders.ContainsKey(t.Symbol))
                {
                    LimitOrder limitOrder = _outStandingLimitOrders[t.Symbol];

                    //If trailing, need to relcalculate trigger price.
                    if (limitOrder.Trailing) limitOrder.RecalculateTrail(t);

                    //Trigger limit if price goes -above- Buy Limit Trigger
                    if (limitOrder.IsBuyLimit())
                    {
                        //net limit to close all
                        if (t.BidClose >= limitOrder.TriggerPrice)
                        {
                            CloseLimitOrder(t, limitOrder);
                        }
                    }

                    //Trigger limit if price drops -below- Limit Stop Trigger
                    else if (limitOrder.IsSellLimit())
                    {
                        if (t.AskClose <= limitOrder.TriggerPrice)
                        {
                            CloseLimitOrder(t, limitOrder);
                        }
                    }
                }

            }


        //-----------------------------------------------------------
        // Adjust portfolio equity/margin
        //-----------------------------------------------------------

        _portfolio.AdjustPortfolioEquityAndMargin(ticks);

        }
        
        public bool ExistsPositionForSymbol(Symbol s)
        {
            return _portfolio.ExistsPositionForSymbol(s);
        }

        public bool ExistsLongPositionForSymbol(Symbol s)
        {
            return _portfolio.ExistsPositionForSymbol(s) && _portfolio[s].Side.Equals(Position.PositionSide.Long);
        }

        public bool ExistsShortPositionForSymbol(Symbol s)
        {
            return _portfolio.ExistsPositionForSymbol(s) && _portfolio[s].Side.Equals(Position.PositionSide.Short);
        }
    }
}