using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ikvm.extensions;
using MathNet.Numerics.LinearAlgebra.Double;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.MarketData;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface.Functions;
using QuantSys.Util;
using QuantSys.Visualization.Highstocks;

namespace QuantSys.Visualization
{
    internal static class TestIndicator
    {

        public static void TestMultiSymbolGraph(this AbstractMultiSymbolIndicator ind, string[] filename, int length)
        {
            

            List<Quantum> lq = new List<Quantum>();

            foreach (string s in filename)
            {
                lq.Add(Quantum.ExcelToQuantum(s, s, 0));
            }
            MultiQuantum multiQuantum = MultiQuantum.OrganizeMultiQuantum(lq);


            var dz = new DenseMatrix(4 + 1 + ind.SubIndicatorSize, multiQuantum.Length);
            List<string> names = new List<string>();
            names.Add("symbol");
            names.Add(ind.ToString());
            foreach (var indicator in ind.SubIndicators) names.Add(indicator.Key);

            //chartoptions
            ChartOption[] chartOptions = new ChartOption[names.Count];
            chartOptions[0] = new ChartOption() { Height = 400, YPosition = 0 };
            chartOptions[1] = new ChartOption() { Height = 200, YPosition = 1 };
            for (int i = 2; i < chartOptions.Length; i++)
                chartOptions[i] = new ChartOption() { Height = 0, YPosition = 1, Layover = true };

            int counter = 0;
            foreach (List<Tick> t in multiQuantum)
            {
                dz[0, counter] = t[0].BidOpen;
                dz[1, counter] = t[0].BidHigh;
                dz[2, counter] = t[0].BidLow;
                dz[3, counter] = t[0].BidClose;

                dz[4, counter] = ind.HandleNextTicks(t.ToArray());

                int icounter = 5;
                foreach (var subind in ind.SubIndicators.Values)
                {
                    dz[icounter, counter] = subind[0];
                    icounter++;
                }

                counter++;
            }


            Visualize.GenerateMultiPaneGraph(names.ToArray(), multiQuantum.Keys.ToArray(), dz, QSConstants.DEFAULT_DATA_FILEPATH + @"results.html",
                chartOptions);

            Console.WriteLine("Done Generating Graph for " + ind.ToString());
        }


        public static void TestGraph(this AbstractIndicator ind, string filename, int length)
        {
            Quantum q = Quantum.ExcelToQuantum(filename, "symbol", 0);

            var dz = new DenseMatrix(4+1+ind.SubIndicatorSize, q.Data.Count);
            List<string> names = new List<string>();
            names.Add("symbol");
            names.Add(ind.ToString());
            foreach(var indicator in ind.SubIndicators) names.Add(indicator.Key);

            //chartoptions
            ChartOption[] chartOptions = new ChartOption[names.Count];
            chartOptions[0] = new ChartOption() {Height = 400, YPosition = 0};
            chartOptions[1] = new ChartOption() { Height = 200, YPosition = 1};
            for(int i = 2; i < chartOptions.Length; i++)
                chartOptions[i] = new ChartOption(){Height = 0, YPosition = 1, Layover = true};

            int counter = 0;
            foreach (Tick tick in q)
            {
                dz[0, counter] = tick.BidOpen;
                dz[1, counter] = tick.BidHigh;
                dz[2, counter] = tick.BidLow;
                dz[3, counter] = tick.BidClose;

                dz[4, counter] = ind.HandleNextTick(tick);

                int icounter = 5;
                foreach (var subind in ind.SubIndicators.Values)
                {
                    dz[icounter, counter] = subind[0];
                    icounter++;
                }

                counter++;
            }


            Visualize.GenerateMultiPaneGraph(names.ToArray(), q.Data.Keys.ToArray(), dz, QSConstants.DEFAULT_DATA_FILEPATH + @"results.html",
                chartOptions);

            Console.WriteLine("Done Generating Graph for " + ind.ToString());
        }



        public static void TestGraphLive(this AbstractIndicator ind, string timeframe, string symbol, int length)
        {
            //------------grab data
            FXSession session = new FXSession();
            session.InitializeSession();

            HistoricPriceEngine h = new HistoricPriceEngine(session);
            h.GetLongHistoricPrices(symbol, timeframe, length);

            while (!h.Complete)
            {
                Thread.Sleep(100);
            }
            //-----------------------

            Quantum q = h.Data;
            var dz = new DenseMatrix(4 + 1 + ind.SubIndicatorSize, q.Data.Count);
            List<string> names = new List<string>();
            names.Add("symbol");
            names.Add(ind.ToString());
            foreach (var indicator in ind.SubIndicators) names.Add(indicator.Key);

            //chartoptions
            ChartOption[] chartOptions = new ChartOption[names.Count];
            chartOptions[0] = new ChartOption() { Height = 400, YPosition = 0 };
            chartOptions[1] = new ChartOption() { Height = 200, YPosition = 1 };
            for (int i = 2; i < chartOptions.Length; i++)
                chartOptions[i] = new ChartOption() { Height = 0, YPosition = 1, Layover = true };

            int counter = 0;
            foreach (Tick tick in q)
            {
                dz[0, counter] = tick.BidOpen;
                dz[1, counter] = tick.BidHigh;
                dz[2, counter] = tick.BidLow;
                dz[3, counter] = tick.BidClose;

                dz[4, counter] = ind.HandleNextTick(tick);

                int icounter = 5;
                foreach (var subind in ind.SubIndicators.Values)
                {
                    dz[icounter, counter] = subind[0];
                    icounter++;
                }

                counter++;
            }


            Visualize.GenerateMultiPaneGraph(names.ToArray(), q.Data.Keys.ToArray(), dz, QSConstants.DEFAULT_DATA_FILEPATH + @"results.html",
                chartOptions);

            Console.WriteLine("Done Generating Graph for " + ind.ToString());
        }


        public static void TestMA(string filename, int length, params AbstractIndicator[] ind)
        {
            Quantum q = Quantum.ExcelToQuantum(filename, "symbol", 0);

            var dz = new DenseMatrix(4 + ind.Count(), q.Data.Count);
            List<string> names = new List<string>();
            names.Add("symbol");
            foreach (var indicator in ind) names.Add(indicator.toString());

            //chartoptions
            ChartOption[] chartOptions = new ChartOption[names.Count];
            chartOptions[0] = new ChartOption() { Height = 500, YPosition = 0 };
            for (int i = 1; i < chartOptions.Length; i++)
                chartOptions[i] = new ChartOption() { Height = 0, YPosition = 0, Layover = true };

            int counter = 0;
            foreach (Tick tick in q)
            {
                dz[0, counter] = tick.BidOpen;
                dz[1, counter] = tick.BidHigh;
                dz[2, counter] = tick.BidLow;
                dz[3, counter] = tick.BidClose;


                int icounter = 4;
                foreach (var subind in ind)
                {
                    subind.HandleNextTick(tick);
                    dz[icounter, counter] = subind[0];
                    icounter++;
                }

                counter++;
            }


            Visualize.GenerateMultiPaneGraph(names.ToArray(), q.Data.Keys.ToArray(), dz, QSConstants.DEFAULT_DATA_FILEPATH + @"results.html",
                chartOptions);

            Console.WriteLine("Done Generating Graph for " + ind.ToString());
        }

        public static void TestChannel(this AbstractChannel ind, string filename, int length)
        {

            Quantum q = Quantum.ExcelToQuantum(filename, "symbol", 0);

            var dz = new DenseMatrix(4 + 3, q.Data.Count);
            
            List<string> names = new List<string>();
            names.Add("symbol");
            names.Add("HIGH");
            names.Add(ind.ToString());
            names.Add("LOW");

            //chartoptions
            ChartOption[] chartOptions = new ChartOption[names.Count];
            chartOptions[0] = new ChartOption() { Height = 500, YPosition = 0 };
            chartOptions[1] = new ChartOption() { Height = 0, YPosition = 0 , Layover = true};
            chartOptions[2] = new ChartOption() { Height = 0, YPosition = 0 , Layover = true};
            chartOptions[3] = new ChartOption() { Height = 0, YPosition = 0 , Layover = true};

            int counter = 0;
            foreach (Tick tick in q)
            {
                ind.HandleNextTick(tick);
                dz[0, counter] = tick.BidOpen;
                dz[1, counter] = tick.BidHigh;
                dz[2, counter] = tick.BidLow;
                dz[3, counter] = tick.BidClose;
                dz[4, counter] = ind.HI(0);
                dz[5, counter] = ind.MID(0);
                dz[6, counter] = ind.LOW(0);

                counter++;
            }

            Visualize.GenerateMultiPaneGraph(new[] { "data", "high", ind.ToString(), "low" }, q.Data.Keys.ToArray(), dz, QSConstants.DEFAULT_DATA_FILEPATH + @"results.html"
                , new ChartOption[]
                {
                    new ChartOption(){Height = 500}, 
                    new ChartOption(){Height = 0, Layover = true, YPosition = 0},
                    new ChartOption(){Height = 0, Layover = true, YPosition = 0},
                    new ChartOption(){Height = 0, Layover = true, YPosition = 0}
                });

            Console.WriteLine("Done Generating Graph for " + ind.ToString());
        }


        public static void TestChannelLive(this AbstractChannel ind, string symbol, string timeframe, int length)
        {
            //------------grab data
            FXSession session = new FXSession();
            session.InitializeSession();
            while (!session.LoggedIn)
            {
                Thread.Sleep(100);
            }

            HistoricPriceEngine h = new HistoricPriceEngine(session);
            h.GetLongHistoricPrices(symbol, timeframe, length);

            while (!h.Complete)
            {
                Thread.Sleep(100);
            }
            //-----------------------

            var highList = new List<double>();
            var medList = new List<double>();
            var lowList = new List<double>();

            var dataList = new List<double>();
            var dateTimeList = new SortedList<DateTime, int>();

            Quantum q = h.Data;

            int count = 0;
            foreach (Tick t in q)
            {
                try{

                    ind.HandleNextTick(t);
                    highList.Add(ind.HI(0));
                    medList.Add(ind.MID(0));
                    lowList.Add(ind.LOW(0));

                    dataList.Add(t.BidClose);
                    dateTimeList.Add(t.Time, 1);
                }
                catch (Exception e)
                {
                    e.printStackTrace();
                }
                if (count++ > length) break;
            }

            var dz = new DenseMatrix(4, medList.Count);
            dz.SetRow(0, new DenseVector(dataList.ToArray()));
            dz.SetRow(1, new DenseVector(highList.ToArray()));
            dz.SetRow(2, new DenseVector(medList.ToArray()));
            dz.SetRow(3, new DenseVector(lowList.ToArray()));

            Visualize.GenerateMultiPaneGraph(new[] { "data", "high", ind.ToString(), "low" }, dateTimeList.Keys.ToArray(), dz, QSConstants.DEFAULT_DATA_FILEPATH + @"results.html"
                , new ChartOption[]
                {
                    new ChartOption(){Height = 500}, 
                    new ChartOption(){Height = 0, Layover = true, YPosition = 0},
                    new ChartOption(){Height = 0, Layover = true, YPosition = 0},
                    new ChartOption(){Height = 0, Layover = true, YPosition = 0}
                });

            Console.WriteLine("Done Generating Graph for " + ind.ToString());
            }
        }
}
