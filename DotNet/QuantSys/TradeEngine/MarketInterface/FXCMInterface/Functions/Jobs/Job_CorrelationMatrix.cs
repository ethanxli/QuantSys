using System;
using System.Collections.Generic;
using System.Threading;
using MathNet.Numerics.LinearAlgebra.Double;
using QuantSys.Analytics;
using QuantSys.Analytics.StatisticalModeling;
using QuantSys.MarketData;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface.EventArguments;
using QuantSys.Visualization;

namespace QuantSys.TradeEngine.MarketInterface.FXCMInterface.Functions.Jobs
{
    public class Job_CorrelationMatrix : IJob
    {
        public enum CovarianceType
        {
            Price = 1,
            RawReturn = 2,
            LogReturn = 3
        }

        private readonly CovarianceType cType;

        private readonly DenseMatrix correlation;
        private readonly DenseMatrix covariance;
        private readonly List<MarketDataEventArg> mktData;
        private readonly int numTicks;
        private readonly string[] symbols;
        private readonly string timeframe;

        public Job_CorrelationMatrix(string[] symbols, string timeframe, int numTicks, CovarianceType c)
        {
            this.symbols = symbols;
            this.timeframe = timeframe;
            this.numTicks = numTicks;
            correlation = new DenseMatrix(symbols.Length, symbols.Length, 0.0);
            covariance = new DenseMatrix(symbols.Length, symbols.Length, 0.0);
            cType = c;
            mktData = new List<MarketDataEventArg>();
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
            try
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
                    for (int j = 0; j < symbols.Length; j++)
                    {
                        double[] pDatai = priceData.Row(i).ToArray();
                        double[] pDataj = priceData.Row(j).ToArray();

                        switch (cType)
                        {
                            case CovarianceType.LogReturn:
                            {
                                pDatai = priceData.Row(i).ToArray().LogRateOfReturn();
                                pDataj = priceData.Row(j).ToArray().LogRateOfReturn();
                                break;
                            }
                            case CovarianceType.RawReturn:
                            {
                                pDatai = priceData.Row(i).ToArray().RawRateOfReturn();
                                pDataj = priceData.Row(j).ToArray().RawRateOfReturn();
                                break;
                            }
                        }
                        correlation[i, j] = StatisticsExtension.Correlation(pDatai, pDataj);
                        covariance[i, j] = StatisticsExtension.Covariance((DenseVector) priceData.Row(i),
                            (DenseVector) priceData.Row(j));
                    }
                }

                Visualize.GenerateHeatMatrix(symbols, correlation, "C:\\Users\\Ethan\\Work\\QuantSysdata.html");
                Console.WriteLine("Finished Generating Correlation Matrix.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}