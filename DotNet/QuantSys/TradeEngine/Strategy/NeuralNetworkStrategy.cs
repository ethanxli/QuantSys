using QuantSys.Analytics.Timeseries.Indicators.Oscillators;
using QuantSys.DataStructures;
using QuantSys.MachineLearning.NeuralNetwork;
using QuantSys.MarketData;
using QuantSys.TradeEngine.Simulation.Account;

namespace QuantSys.TradeEngine.Strategy
{
    public class NeuralNetworkStrategy: AbstractStrategy
    {

        public Stage1NeuralNetwork NeuralNetwork { get; set; }

        private MovingQueue<double> dataList;
        private double prevIndicatorTick = double.NaN;

        public NeuralNetworkStrategy(int window)
        {
            dataList = new MovingQueue<double>(window);
            AttachIndicator("IND", new AC(60));
        }

        public override void OnTick(params Tick[] t)
        {
            foreach (var indicator in indicatorList)
            {
                indicator.Value.HandleNextTick(t[0]);
            }

            if (!indicatorList["IND"][0].Equals(double.NaN))
                dataList.Enqueue(indicatorList["IND"][0]);

            if (dataList.Count == dataList.Capacity)
            {
                double nextIndicatorTick = NeuralNetwork.PredictNext(dataList.ToArray());

                //order fire logic
                if (nextIndicatorTick > 0 && nextIndicatorTick > prevIndicatorTick && prevIndicatorTick < 0)
                {
                    if (!IAccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        IAccountManager.PlaceMarketOrder(t[0].Symbol, 10000, Position.PositionSide.Long, .001);
                    }
                }

                //order fire logic
                if (nextIndicatorTick < prevIndicatorTick)
                {
                    if (IAccountManager.ExistsLongPositionForSymbol(t[0].Symbol))
                    {
                        IAccountManager.ClosePosition(t[0].Symbol);
                    }
                }

                if (nextIndicatorTick < 0 && nextIndicatorTick < prevIndicatorTick && prevIndicatorTick > 0)
                {
                    if (!IAccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        IAccountManager.PlaceMarketOrder(t[0].Symbol, 10000, Position.PositionSide.Short, .001);
                    }
                }

                if (nextIndicatorTick > prevIndicatorTick)
                {
                    if (IAccountManager.ExistsShortPositionForSymbol(t[0].Symbol))
                    {
                        IAccountManager.ClosePosition(t[0].Symbol);
                    }
                }

                prevIndicatorTick = nextIndicatorTick;

            }



        }
    }
}
