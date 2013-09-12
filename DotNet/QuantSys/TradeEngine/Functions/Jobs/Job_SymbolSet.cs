using System;
using System.Collections.Generic;
using System.Threading;
using MathNet.Numerics.LinearAlgebra.Double;
using QuantSys.Indicators.Abstraction;
using QuantSys.MarketData;
using QuantSys.TradeEngine.EventArguments;
using QuantSys.Util;
using QuantSys.Visualization;

namespace QuantSys.TradeEngine.Functions.Jobs
{
    public class Job_SymbolSet : IJob
    {
        private readonly DenseMatrix graphData;
        private readonly AbstractIndicator indicator;
        private readonly List<MarketDataEventArg> mktData;
        private readonly int numTicks;
        private readonly string[] symbols;
        private readonly string timeframe;

        public Job_SymbolSet(string[] symbols, string timeframe, int numTicks, AbstractIndicator indicator)
        {
            this.symbols = symbols;
            this.indicator = indicator;
            this.timeframe = timeframe;
            this.numTicks = numTicks;
            mktData = new List<MarketDataEventArg>();
            graphData = new DenseMatrix(symbols.Length, numTicks);
        }

        public void RunJob(FXSession fxsession)
        {
            foreach (string symbol in symbols)
            {
                //ResponseHandler.GetHistoricPrices(fxsession, symbol, timeframe, numTicks);
                Thread.Sleep(100); // don't overload api
            }
        }

        public bool UpdateJob(object data)
        {
            try
            {
                var m = (MarketDataEventArg) data;
                mktData.Add(m);
                if (mktData.Count != symbols.Length)
                    return false;
                FinishAndProcess();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }

        public void FinishAndProcess()
        {
            for (int i = 0; i < symbols.Length; i++)
            {
                SortedList<DateTime, Tick> d = mktData[i].data.Data;

                for (int j = 0; j < numTicks; j++)
                {
                    graphData[i, j] = indicator.HandleNextTick(d.Values[j]);
                }
            }

            Visualize.GenerateMultiSymbolGraph(symbols, graphData, mktData[0].data.Data.Values[0].Time,
                DateTimeUtils.TimeFrameToTimeSpan(timeframe), "C:\\Users\\Ethan\\Work\\QuantSysdata.html");

            Console.WriteLine("Done Processing symbol set.");
        }
    }
}