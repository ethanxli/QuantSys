using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MathNet.Numerics.LinearAlgebra.Double;
using QuantSys.Analytics;
using QuantSys.MarketData;
using QuantSys.PortfolioEngine;
using QuantSys.Strategy;
using QuantSys.TradeEngine;
using QuantSys.TradeEngine.Functions;
using QuantSys.Util;
using QuantSys.Visualization;
using QuantSys.Visualization.Highstocks;

namespace QuantSys.Backtester
{
    public class BacktestEngine
    {
        private readonly int _dataStart;
        private readonly int _backTestLength;
        private List<Quantum> _dataSet;
        public MultiQuantum _multiQuantum;

        private AccountManager _accountManager;
        private List<AbstractStrategy> _strategies;

        //private DateTime? startDate;
        //private DateTime? endDate;
        public double ret = 0;

        private DenseMatrix performanceMatrix;

        public BacktestEngine(int start, int backtestLength, bool output)
        {
            _dataStart = start;
            _backTestLength = backtestLength;
            _accountManager = new AccountManager();
            _dataSet = new List<Quantum>();
            _strategies = new List<AbstractStrategy>();
            Output = output;

        }

        public bool Output { get; set; }

        public void ResetAccount()
        {
            _accountManager = new AccountManager();
        }

        public void ResetStrategies()
        {
            _strategies = new List<AbstractStrategy>();
        }

        public void LoadStrategy(AbstractStrategy s)
        {
            s.SetAccountManager(_accountManager);
            _strategies.Add(s);
        }

        public void OrganizeData()
        {
            _multiQuantum = MultiQuantum.OrganizeMultiQuantum(_dataSet);
        }

        public void LoadDataLive(string symbol, string timeframe, int ticks)
        {
            FXSession session  = new FXSession();
            session.InitializeSession();
            while (!session.LoggedIn)
            {
                Thread.Sleep(100);
            }
            HistoricPriceGrabber price = new HistoricPriceGrabber(session);
            price.GetLongHistoricPrices(symbol, timeframe, ticks);

            while (!price.Complete)
            {
                Thread.Sleep(100);
            }

            _dataSet.Add(price.Data);

            session.EndSession();
        }

        public void LoadData(string filename, string symbol)
        {
            Quantum q = Quantum.ExcelToQuantum(filename, symbol, _dataStart);
            _dataSet.Add(q);
        }

        public void Execute()
        {
            //-----------------for performance/visualization purposes---------------
            int numberOfRows = 6; //ohlc data and balance/performance data
            numberOfRows += _strategies[0].indicatorList.Count;
            performanceMatrix = new DenseMatrix(numberOfRows, _backTestLength);
            //-----------------------------------------------------------------------

            for (int i = 0; i < _backTestLength; i++)
            {
                var ticks = _multiQuantum[i].ToArray();

                //call onTick for each strategy
                foreach (AbstractStrategy s in _strategies)
                {
                    s.OnTick(ticks);
                }

                // calculate equity and balance performance for each tick
                _accountManager.OnTick(ticks);


                //------------------------------Performance------------------/
                performanceMatrix[0, i] = ticks[0].BidOpen;
                performanceMatrix[1, i] = ticks[0].BidHigh;
                performanceMatrix[2, i] = ticks[0].BidLow;
                performanceMatrix[3, i] = ticks[0].BidClose;
                performanceMatrix[4, i] = _accountManager.CurrentBalance;
                performanceMatrix[5, i] = _accountManager.CurrentEquity;
                int counter = 6;
                foreach (var indicator in _strategies[0].indicatorList.Values)
                {
                    performanceMatrix[counter, i] = indicator[0];
                    counter++;
                }
                //-----------------------------------------------------------/


            }

            CalculatePerformance();
        }


        public void CalculatePerformance()
        {


            double[] performance = performanceMatrix.Row(4).ToArray();
            double sharpeRatio = performance.ToArray().SharpeRatio();
            double averageRet = performance.ToArray().AverageRawReturn();

            ret = 100 * (performance[performance.Length - 1] - performance[0]) / performance[0];

            if (Output)
            {
                List<string> symbols = new List<string>();
                symbols.Add("Performance");
                symbols.Add("Balance");
                symbols.Add("Equity");

                List<ChartOption> options = new List<ChartOption>();
                options.Add(new ChartOption() { ChartType = "spline", Height = 300, YPosition = 0 });
                options.Add(new ChartOption() { ChartType = "spline", Height = 200, YPosition = 1 });
                options.Add(new ChartOption() { ChartType = "spline", Height = 0, YPosition = 1, Layover = true });

                int yPos = 3;
                foreach (var indicator in _strategies[0].indicatorList.Values)
                {
                    options.Add(new ChartOption() { ChartType = "spline", Height = 200, YPosition = yPos });
                    symbols.Add(indicator.ToString());
                    yPos++;
                }


                Visualize.GenerateMultiPaneGraph(
                    symbols.ToArray(),
                    _multiQuantum.Keys.ToArray(),
                    performanceMatrix,
                    QSConstants.DEFAULT_DATA_FILEPATH + "result.html",
                    options.ToArray(),
                    _accountManager.Flags.ToArray()
                    );

                Console.WriteLine("Total return: " +
                                  100*(performance[performance.Length - 1] - performance[0])/performance[0] + "%");
                Console.WriteLine("Maximum drawdown: " + 100*performance.ToArray().MaximumDrawdown(true) + "%");
                Console.WriteLine("Average return " + 100*averageRet + "%");
                Console.WriteLine("Sharpe ratio " + sharpeRatio);

                double trades = _accountManager.Trades.Count;
                double totalProfit = _accountManager.Trades.Sum(d1 => d1 > 0 ? d1 : 0);
                double totalLoss = _accountManager.Trades.Sum(d1 => d1 < 0 ? d1 : 0);
                double ProfitCount = _accountManager.Trades.Count(d1 => d1 > 0);
                double LossCount = _accountManager.Trades.Count(d1 => d1 < 0);

                Console.WriteLine("Number of trades closed: " + trades);
                Console.WriteLine("% Profit trades: " + ProfitCount/trades);
                Console.WriteLine("% Loss trades: " + LossCount / trades);
                Console.WriteLine("Average Profit/trade: " + totalProfit/ProfitCount);
                Console.WriteLine("Average Loss/trade: " + totalLoss / LossCount);
            }
        }
    }
}