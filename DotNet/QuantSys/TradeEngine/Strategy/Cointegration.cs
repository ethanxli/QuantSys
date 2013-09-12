using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using QuantSys.Analytics.StatisticalModeling;
using QuantSys.Analytics.StatisticalModeling.Cointegration;
using QuantSys.Analytics.Timeseries.Indicators.Averages;
using QuantSys.Analytics.Timeseries.Indicators.Misc;
using QuantSys.DataStructures;
using QuantSys.MarketData;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface;
using QuantSys.TradeEngine.Simulation.Account;

namespace QuantSys.TradeEngine.Strategy
{
    public class Cointegration : AbstractStrategy
    {
        private MovingQueue<double> data1;
        private MovingQueue<double> data2;

        private int counter;
        public int n { get; set; }

        public FXSession session { get; set; }

        private TEMA TEMA;
        public Cointegration()
        {
            n = 3000;
            data1 = new MovingQueue<double>(n);
            data2 = new MovingQueue<double>(n);
            TEMA = new TEMA(50);
            AttachIndicator("Container", new GenericContainer(n));
        }
        
        public override void OnTick(params Tick[] t)
        {
            
            data1.Enqueue(t[0].BidClose);
            data2.Enqueue(t[1].BidClose);

            double STOP = 1.005;

            if (counter++ >= n)
            {
                var d1 = new DenseVector(data1.ToArray().NormalizeZScore());
                var d2 = new DenseVector(data2.ToArray().NormalizeZScore());

                double spread;
                //spread= (d1 - d2).ToArray().NormalizeZScore()[d1.Count - 1];


                List<double[]> mat = new List<double[]>();
                mat.Add(d1.ToArray());
                mat.Add(d2.ToArray());
                List<MaxEigenData> maxEigen = new List<MaxEigenData>();
                double[] maxEigenValues;
                double[,] eigenMatrix;

                JohansenHelper.DoMaxEigenValueTest(mat, 5, out maxEigen, out maxEigenValues, out eigenMatrix);

                spread = (maxEigenValues[0]*d1 - maxEigenValues[1]*d2).ToArray().NormalizeZScore()[d1.Count-1];


                TEMA.HandleNextTick(spread);

                ((GenericContainer)indicatorList["Container"]).HandleNextTick(spread);

                /*
                //t[0] hit stop / limit
                if ( (IAccountManager.ExistsShortPositionForSymbol(t[1].Symbol) &&
                    !IAccountManager.ExistsLongPositionForSymbol(t[0].Symbol))
                    ||
                    (IAccountManager.ExistsLongPositionForSymbol(t[1].Symbol) &&
                    !IAccountManager.ExistsShortPositionForSymbol(t[0].Symbol))
                    )
                {
                    if(!IAccountManager.GetStopOrder(t[1].Symbol).Trailing)
                        IAccountManager.ModifyTrailingStop(t[1], STOP/2);
                }

                //t[1] positiong hit stop/limit
                if ((IAccountManager.ExistsShortPositionForSymbol(t[0].Symbol) &&
                    !IAccountManager.ExistsLongPositionForSymbol(t[1].Symbol))
                    ||
                    (IAccountManager.ExistsLongPositionForSymbol(t[0].Symbol) &&
                    !IAccountManager.ExistsShortPositionForSymbol(t[1].Symbol))
                    )
                {
                    if (!IAccountManager.GetStopOrder(t[0].Symbol).Trailing)
                        IAccountManager.ModifyTrailingStop(t[0], STOP/2);
                }
                */

                if (spread < -1 && IAccountManager.ExistsShortPositionForSymbol(t[0].Symbol) &&
                         IAccountManager.ExistsLongPositionForSymbol(t[1].Symbol))
                {
                    IAccountManager.ClosePosition(t[0].Symbol);
                    IAccountManager.ClosePosition(t[1].Symbol);

                    
                    Console.WriteLine("Closed Pair Trade.");

                }
                if (spread > 1 && IAccountManager.ExistsLongPositionForSymbol(t[0].Symbol) &&
                        IAccountManager.ExistsShortPositionForSymbol(t[1].Symbol))
                {
                    IAccountManager.ClosePosition(t[0].Symbol);
                    IAccountManager.ClosePosition(t[1].Symbol);


                    Console.WriteLine("Closed Pair Trade.");

                }


                //short d1, long d2 if spread > 2.0
                if (TEMA[0] < 3  && TEMA[1] > 3 && !IAccountManager.ExistsPositionForSymbol(t[0].Symbol) &&
                    !IAccountManager.ExistsPositionForSymbol(t[1].Symbol))
                {
                    IAccountManager.PlaceMarketOrder(t[0].Symbol, 10000, Position.PositionSide.Short, STOP);
                    IAccountManager.PlaceMarketOrder(t[1].Symbol, 10000, Position.PositionSide.Long, STOP);


                    Console.WriteLine("Placed Pair Trade.");

                }

                if (TEMA[0] > -3 && TEMA[1] < -3 && !IAccountManager.ExistsPositionForSymbol(t[0].Symbol) &&
                  !IAccountManager.ExistsPositionForSymbol(t[1].Symbol))
                {
                    IAccountManager.PlaceMarketOrder(t[0].Symbol, 10000, Position.PositionSide.Long, STOP);
                    IAccountManager.PlaceMarketOrder(t[1].Symbol, 10000, Position.PositionSide.Short, STOP);


                    Console.WriteLine("Placed Pair Trade.");

                }

            }
        }

    }
}