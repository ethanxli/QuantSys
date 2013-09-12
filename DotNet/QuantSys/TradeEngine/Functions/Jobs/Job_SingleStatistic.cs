using System;
using System.Collections.Generic;
using System.Threading;
using MathNet.Numerics.LinearAlgebra.Double;
using QuantSys.MarketData;
using QuantSys.TradeEngine.EventArguments;

namespace QuantSys.TradeEngine.Functions.Jobs
{
    public class Job_SingleStatistic
    {
        private readonly Func<DenseVector, double> function;
        private readonly List<MarketDataEventArg> mktData;
        private readonly int numTicks;
        private readonly string[] symbols;
        private readonly string timeframe;
        public DenseVector results;

        public Job_SingleStatistic(string[] symbols, string timeframe, int numTicks, Func<DenseVector, double> function)
        {
            this.symbols = symbols;
            this.timeframe = timeframe;
            this.numTicks = numTicks;
            mktData = new List<MarketDataEventArg>();
            results = new DenseVector(symbols.Length);
            this.function = function;
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
                var m = (MarketDataEventArg)data;
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
            var priceData = new DenseMatrix(symbols.Length, numTicks);

            for (int j = 0; j < symbols.Length; j++)
            {
                SortedList<DateTime, Tick> d = mktData[j].data.Data;
                for (int k = 0; k < d.Count; k++)
                {
                    priceData[j, k] = d.Values[k].BidClose;
                }
            }

            for (int i = 0; i < symbols.Length; i++)
            {
                results[i] = function((DenseVector) priceData.Row(i));
            }
        }
    }
}