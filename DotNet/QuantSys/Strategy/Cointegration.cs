using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using QuantSys.Analytics;
using QuantSys.DataStructures;
using QuantSys.Indicators.Abstraction;
using QuantSys.Indicators.Averages;
using QuantSys.MarketData;
using QuantSys.PortfolioEngine;
using QuantSys.PortfolioEngine.Order;
using QuantSys.TradeEngine;
using QuantSys.TradeEngine.Functions;

namespace QuantSys.Strategy
{
    public class Cointegration : AbstractStrategy
    {
        private MovingQueue<double> data1;
        private MovingQueue<double> data2;

        private int counter;
        public int n { get; set; }
        private bool live = false;

        public FXSession session { get; set; }

        private TEMA TEMA;
        public Cointegration(bool live = false)
        {
            n = 3000;
            data1 = new MovingQueue<double>(n);
            data2 = new MovingQueue<double>(n);
            this.live = live;
            TEMA = new TEMA(50);
        }

        public Dictionary<string, AbstractIndicator> Indicators { get { return indicatorList; } }

        public override void OnTick(params Tick[] t)
        {
            data1.Enqueue(t[0].BidClose);
            data2.Enqueue(t[1].BidClose);

            double STOP = 0.005;

            if (counter++ >= n)
            {
                var d1 = new DenseVector(data1.ToArray().NormalizeZScore());
                var d2 = new DenseVector(data2.ToArray().NormalizeZScore());

                double spread = (d1 - d2).ToArray().NormalizeZScore()[d1.Count - 1];
                TEMA.HandleNextTick(spread);

                //t[0] hit stop / limit
                if ( (AccountManager.ExistsShortPositionForSymbol(t[1].Symbol) &&
                    !AccountManager.ExistsLongPositionForSymbol(t[0].Symbol))
                    ||
                    (AccountManager.ExistsLongPositionForSymbol(t[1].Symbol) &&
                    !AccountManager.ExistsShortPositionForSymbol(t[0].Symbol))
                    )
                {
                    if(!AccountManager.GetStopOrder(t[1].Symbol).Trailing)
                        AccountManager.ModifyTrailingStop(t[1], STOP/2);
                }

                //t[1] positiong hit stop/limit
                if ((AccountManager.ExistsShortPositionForSymbol(t[0].Symbol) &&
                    !AccountManager.ExistsLongPositionForSymbol(t[1].Symbol))
                    ||
                    (AccountManager.ExistsLongPositionForSymbol(t[0].Symbol) &&
                    !AccountManager.ExistsShortPositionForSymbol(t[1].Symbol))
                    )
                {
                    if (!AccountManager.GetStopOrder(t[0].Symbol).Trailing)
                        AccountManager.ModifyTrailingStop(t[0], STOP/2);
                }


                if (spread < 0.5 && AccountManager.ExistsShortPositionForSymbol(t[0].Symbol) &&
                         AccountManager.ExistsLongPositionForSymbol(t[1].Symbol))
                {
                    AccountManager.CloseOrder(t[0]);
                    AccountManager.CloseOrder(t[1]);

                    
                    Console.WriteLine("Closed Pair Trade.");

                    if (live)
                    {
                        //////TESING LIVE TRADING/////////////////////////////////////////////////////////////////
                        OrderPlacement op = new OrderPlacement(session);
                        OrderPlacement.OrderObject orderObject = op.prepareParamsFromLoginRules(t[0].Symbol.SymbolString);
                        op.CreateTrueMarketCloseOrder(orderObject.OfferID, orderObject.AccountID, "", 1000, "Buy");

                        orderObject = op.prepareParamsFromLoginRules(t[1].Symbol.SymbolString);
                        op.CreateTrueMarketCloseOrder(orderObject.OfferID, orderObject.AccountID, "", 1000, "Sell");
                        //////TESING LIVE TRADING/////////////////////////////////////////////////////////////////
                    }
                }
                if (spread > 0.5 && AccountManager.ExistsLongPositionForSymbol(t[0].Symbol) &&
                        AccountManager.ExistsShortPositionForSymbol(t[1].Symbol))
                {
                    AccountManager.CloseOrder(t[0]);
                    AccountManager.CloseOrder(t[1]);


                    Console.WriteLine("Closed Pair Trade.");

                    if (live)
                    {
                        //////TESING LIVE TRADING/////////////////////////////////////////////////////////////////
                        OrderPlacement op = new OrderPlacement(session);
                        OrderPlacement.OrderObject orderObject = op.prepareParamsFromLoginRules(t[0].Symbol.SymbolString);
                        op.CreateTrueMarketCloseOrder(orderObject.OfferID, orderObject.AccountID, "", 1000, "Sell");

                        orderObject = op.prepareParamsFromLoginRules(t[1].Symbol.SymbolString);
                        op.CreateTrueMarketCloseOrder(orderObject.OfferID, orderObject.AccountID, "", 1000, "Buy");
                        //////TESING LIVE TRADING/////////////////////////////////////////////////////////////////
                    }
                }


                //short d1, long d2 if spread > 2.0
                if (TEMA[0] > 3 && TEMA[0] < TEMA[1] && !AccountManager.ExistsPositionForSymbol(t[0].Symbol) &&
                    !AccountManager.ExistsPositionForSymbol(t[1].Symbol))
                {
                    AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Short, 10000), STOP);
                    AccountManager.PlaceMarketOrder(new MarketOrder(t[1], Position.PositionSide.Long, 10000), STOP);


                    Console.WriteLine("Placed Pair Trade.");

                    if (live)
                    {
                        //////TESING LIVE TRADING/////////////////////////////////////////////////////////////////
                        OrderPlacement op = new OrderPlacement(session);
                        OrderPlacement.OrderObject orderObject = op.prepareParamsFromLoginRules(t[0].Symbol.SymbolString);
                        op.CreateTrueMarketOrder(orderObject.AccountID, orderObject.OfferID, 1000, "Sell");

                        orderObject = op.prepareParamsFromLoginRules(t[1].Symbol.SymbolString);
                        op.CreateTrueMarketOrder(orderObject.AccountID, orderObject.OfferID, 1000, "Buy");
                        /////////////////////////////////////////////////////////////////////////////////////////////
                    }
                }

                if (TEMA[0] < -3 && TEMA[0] > TEMA[1] && !AccountManager.ExistsPositionForSymbol(t[0].Symbol) &&
                  !AccountManager.ExistsPositionForSymbol(t[1].Symbol))
                {
                    AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Long, 10000), STOP);
                    AccountManager.PlaceMarketOrder(new MarketOrder(t[1], Position.PositionSide.Short, 10000), STOP);


                    Console.WriteLine("Placed Pair Trade.");

                    if (live)
                    {
                        //////TESING LIVE TRADING/////////////////////////////////////////////////////////////////
                        OrderPlacement op = new OrderPlacement(session);
                        OrderPlacement.OrderObject orderObject = op.prepareParamsFromLoginRules(t[0].Symbol.SymbolString);
                        op.CreateTrueMarketOrder(orderObject.AccountID, orderObject.OfferID, 1000, "Buy");

                        orderObject = op.prepareParamsFromLoginRules(t[1].Symbol.SymbolString);
                        op.CreateTrueMarketOrder(orderObject.AccountID, orderObject.OfferID, 1000, "Sell");
                        /////////////////////////////////////////////////////////////////////////////////////////////
                    }
                }

            }
        }

    }
}