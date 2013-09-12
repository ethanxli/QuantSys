using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using QuantSys.Analytics;
using QuantSys.Indicators.Averages;
using QuantSys.MarketData;
using QuantSys.TradeEngine.EventArguments;
using QuantSys.Util;
using QuantSys.Visualization;
using QuantSys.Visualization.Highstocks;

namespace QuantSys.TradeEngine.Functions.Jobs
{
    public class Job_Cointegration : IJob
    {
        private readonly List<MarketDataEventArg> mktData;
        private readonly int numTicks;
        private readonly string[] symbols;
        private readonly string timeframe;

        public Job_Cointegration(string[] symbols, string timeframe, int numTicks)
        {
            this.symbols = symbols;
            this.timeframe = timeframe;
            this.numTicks = numTicks;
            mktData = new List<MarketDataEventArg>();
        }

        public double Spread { get; set; }

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
                        //if (!symbols[j].Substring(0, 3).Equals("USD")) priceData[j, k] = 1/d.Values[k].BidClose;
                        priceData[j, k] = d.Values[k].BidOpen;
                    }
                }

                Vector<double> price1 = priceData.Row(0);
                Vector<double> price2 = priceData.Row(1);
                //Statistics.ApplyFunction((DenseVector)price1, Math.Log);
                //Statistics.ApplyFunction((DenseVector)price2, Math.Log);

                DenseVector norm1 = price1.ToArray().NormalizeZScore();
                DenseVector norm2 = price2.ToArray().NormalizeZScore();

                var newsym = new string[symbols.Length + 4];
                for (int i = 0; i < symbols.Length; i++) newsym[i] = symbols[i];

                newsym[2] = "spread";
                newsym[3] = "EMA5";
                newsym[4] = "EMA15";
                newsym[5] = "EMA30";


                var m = new DenseMatrix(6, norm1.Count);
                m.SetRow(0, norm1);
                m.SetRow(1, norm2);
                m.SetRow(2, (norm1 - norm2).ToArray().NormalizeZScore());
                m.SetRow(3, EMA.CalcEMA(m.Row(2).ToArray(), 5));
                m.SetRow(4, EMA.CalcEMA(m.Row(2).ToArray(), 15));
                m.SetRow(5, EMA.CalcEMA(m.Row(2).ToArray(), 30));

                string filename = symbols[0].Replace('/', '_') + "-" + symbols[1].Replace('/', '_') + ".html";


                ((DenseVector) m.Row(0)).GenerateSimpleGraph("C:\\Sangar\\result.html");

                Visualize.GenerateMultiSymbolGraph(newsym, m, DateTime.Now.AddSeconds(-60*5*300), new TimeSpan(0, 5, 0),
                    "C:\\Sangar\\" + filename);

                FileUpload.UploadFileToFTP("C:\\Sangar\\" + filename, filename);

                Spread = m[2, m.ColumnCount - 1];

                if (Spread > 2.0 && m[2, m.ColumnCount - 2] <= 2.0)
                    Emailer.SendEmail(symbols[0] + "-" + symbols[1] + " Spread Above 2.0", "Test");

                if (Spread < -2.0 && m[2, m.ColumnCount - 2] >= -2.0)
                    Emailer.SendEmail(symbols[0] + "-" + symbols[1] + " Spread Below -2.0", "Test");

                //if (m[2, m.ColumnCount - 1] < 0.5 && m[2, m.ColumnCount - 2] >= 0.5)
                //    Emailer.SendEmail(symbols[0] + "-" + symbols[1] + " Spread Below 0.5", "Test");

                //if (m[2, m.ColumnCount - 1] > -0.5 && m[2, m.ColumnCount - 2] <= -0.5)
                //    Emailer.SendEmail(symbols[0] + "-" + symbols[1] + " Spread Above -0.5", "Test");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }



        public static void Process(FXSession session, string symbol1, string symbol2, string timeframe, int length)
        {


            HistoricPriceGrabber h1 = new HistoricPriceGrabber(session);
            h1.GetLongHistoricPrices(symbol1, timeframe, length);

            while (!h1.Complete)
            {
                Thread.Sleep(100);
            }

            HistoricPriceGrabber h2 = new HistoricPriceGrabber(session);
            h2.GetLongHistoricPrices(symbol2, timeframe, length);

            while (!h2.Complete)
            {
                Thread.Sleep(100);
            }
            //-----------------------

            var dateTimeList = new SortedList<DateTime, int>();

            Quantum q1 = h1.Data;
            Quantum q2 = h2.Data;


            var priceData = new DenseMatrix(2, q1.Data.Count);

            for (int j = 0; j < ((q1.Data.Count <= q2.Data.Count)?q1.Data.Count:q2.Data.Count); j++ )
            {
                dateTimeList.Add(q1.Data.Values[j].Time, 1);
                priceData[0, j] = q1.Data.Values[j].BidClose;
                priceData[1, j] = q2.Data.Values[j].BidClose;
            }

            Vector<double> price1 = priceData.Row(0);
            Vector<double> price2 = priceData.Row(1);
            //Statistics.ApplyFunction((DenseVector)price1, Math.Log);
            //Statistics.ApplyFunction((DenseVector)price2, Math.Log);

            DenseVector norm1 = price1.ToArray().NormalizeZScore();
            DenseVector norm2 = price2.ToArray().NormalizeZScore();

            var newsym = new string[] {symbol1, symbol2, "spread"};

            var m = new DenseMatrix(6, norm1.Count);
            m.SetRow(0, norm1);
            m.SetRow(1, norm2);
            m.SetRow(2, (norm1 - norm2).ToArray().NormalizeZScore());

            string filename = symbol1.Replace('/', '_') + "-" + symbol2.Replace('/', '_') + ".html";


            Visualize.GenerateMultiPaneGraph(newsym, dateTimeList.Keys.ToArray(), m, QSConstants.DEFAULT_DATA_FILEPATH + filename,
                new ChartOption[]{new ChartOption(), new ChartOption(){Layover = true, YPosition = 0}, new ChartOption(){YPosition = 1} }, null, filename + ".json");

            FileUpload.UploadFileToFTP(QSConstants.DEFAULT_DATA_FILEPATH + filename, filename);
            FileUpload.UploadFileToFTP(QSConstants.DEFAULT_DATA_FILEPATH + filename + ".json", filename + ".json");

            double Spread = m[2, m.ColumnCount - 1];

            if (Spread > 2.0 && m[2, m.ColumnCount - 2] <= 2.0)
                Emailer.SendEmail(symbol1 + "-" + symbol2 + " Spread Above 2.0", "Test");

            if (Spread < -2.0 && m[2, m.ColumnCount - 2] >= -2.0)
                Emailer.SendEmail(symbol1 + "-" + symbol2 + " Spread Below -2.0", "Test");

        }


    }
}