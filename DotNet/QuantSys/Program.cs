using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Encog.App.Quant.Loader.OpenQuant.Data;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using QuantSys.Analytics;
using QuantSys.Analytics.StatisticalModeling.Cointegration;
using QuantSys.Analytics.Timeseries.Indicators.Misc;
using QuantSys.Analytics.Timeseries.Indicators.Oscillators;
using QuantSys.MachineLearning.GeneticAlgorithm;
using QuantSys.MachineLearning.GeneticAlgorithm.Genes;
using QuantSys.MachineLearning.GeneticAlgorithm.Genes.Constraints;
using QuantSys.MachineLearning.NeuralNetwork;
using QuantSys.MarketData;
using QuantSys.TradeEngine;
using QuantSys.TradeEngine.AccountManagement;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface.Functions.Jobs;
using QuantSys.TradeEngine.Strategy;
using QuantSys.Util;
using QuantSys.Visualization;
using QuantSys.Analytics.StatisticalModeling;

namespace QuantSys
{
    public class Program
    {
        private static void Main()
        {


            //TestLiveCointegration.Run();
            /*
            IndicatorMatrix im = new IndicatorMatrix("EUR/USD");
            im.LoadData();
            im.Execute();
            */

            //TestIndicator.TestMA(QSConstants.DEFAULT_DATA_FILEPATH + "GBPUSD15M.xml", 14000, new QSPolyMA(40), new ZLEMA(40));
            //TestIndicator.TestChannelLive(new QSPolyChannel(), "EUR/USD", "m5", 5000 );
            //TestIndicator.TestChannel(new KirshenbaumBands(20), QSConstants.DEFAULT_DATA_FILEPATH + "EURUSD15M.xml", 10000);
            //TestIndicator.TestMA(QSConstants.DEFAULT_DATA_FILEPATH + "EURUSD1H.xml", 15000,  new SMA(25), new DWT(25, 5));
            //TestIndicator.TestGraph(new ReversalGenesis(50),  QSConstants.DEFAULT_DATA_FILEPATH + "AUDUSD1H.xml", 15000);
            //TestIndicator.TestGraph(new HurstIndicator(256), QSConstants.DEFAULT_DATA_FILEPATH + "AUDUSD1H.xml", 15000);
            //TestIndicator.TestGraphLive(new Genesis(30), "m30", "EUR/USD", 10000);
            //TestIndicator.TestGraphLive(new PercentileRank(250, new SMA(200)), "H1", "EUR/USD", 1000);
            //TestIndicator.TestGraphLive(new PercentileRank(250, new SMA(200,new HistoricalVol(50))), "H1", "EUR/USD", 100000);
            //TestIndicator.TestGraphLive(new WilliamsR(), "H1", "EUR/USD", 100000);
            //TestIndicator.TestGraph(new PercentileRank(252, new SMA(251)), QSConstants.DEFAULT_DATA_FILEPATH + "EURUSD1H.xml", 10000);
            Quantum q1 = Quantum.ExcelToQuantum(QSConstants.DEFAULT_DATA_FILEPATH + "EURUSD1H.xml", "EUR/USD");
            Quantum q2 = Quantum.ExcelToQuantum(QSConstants.DEFAULT_DATA_FILEPATH + "GBPUSD1H.xml", "GBP/USD");
            Quantum q3 = Quantum.ExcelToQuantum(QSConstants.DEFAULT_DATA_FILEPATH + "AUDUSD1H.xml", "AUD/USD");
            List<Quantum> lq = new List<Quantum>();
            lq.Add(q1);
            lq.Add(q2);
            lq.Add(q3);
            MultiQuantum mq = MultiQuantum.OrganizeMultiQuantum(lq);
            List <List<Tick>> list = mq.RevertToList();
            double[] dat1 = list[0].ToArray().Select(x => x.BidClose).ToArray().NormalizeZScore();
            double[] dat2 = list[1].ToArray().Select(x => x.BidClose).ToArray().NormalizeZScore();
            double[] dat3 = list[2].ToArray().Select(x => x.BidClose).ToArray().NormalizeZScore();

            DenseVector d11 = new DenseVector(dat1);
            DenseVector d12 = new DenseVector(dat2);
            DenseVector d13 = new DenseVector(dat3);

            DenseVector fe = ((.48 * d11) + (-0.22 * d12) + (-.46 * d13));

            Visualize.GenerateSimpleGraph(fe, "result.html");


            Console.Read();

            Func<Chromosome, double> fitnessFunc = new Func<Chromosome, double>(
                chromosome =>
                {
                    double weight1 = ((RealCodedGene)chromosome[0]).GeneValue;
                    double weight2 = ((RealCodedGene)chromosome[1]).GeneValue;
                    double weight3 = ((RealCodedGene)chromosome[2]).GeneValue;

                    double w1mod = weight1 /(weight1 + weight2 + weight3);
                    double w2mod = weight2 / (weight1 + weight2 + weight3);
                    double w3mod = weight3 / (weight1 + weight2 + weight3);

                    DenseVector d1 = new DenseVector(dat1);
                    DenseVector d2 = new DenseVector(dat2);
                    DenseVector d3 = new DenseVector(dat3);

                    double stdev = ((w1mod * d1) + (w2mod * d2) + (w3mod * d3)).StandardDeviation();

                    return (stdev > 0) ? (1/stdev) : 0.000001;
                }
                );

            Random r = new Random();
            Gene g1 = new RealCodedGene(0, r, new GeneConstraint((x => (double)x < 1.0 && (double)x > -1.0))  {HI = 1.0, LOW = -1.0});
            Gene g2 = new RealCodedGene(0, r, new GeneConstraint((x => (double)x < 1.0 && (double)x > -1.0)) { HI = 1.0, LOW = -1.0 });
            Gene g3 = new RealCodedGene(0, r, new GeneConstraint((x => (double)x < 1.0 && (double)x > -1.0)) { HI = 1.0, LOW = -1.0 });
            List<Gene> cfootprint = new List<Gene>();
            cfootprint.Add(g1);
            cfootprint.Add(g2);
            cfootprint.Add(g3);

            GeneticAlgorithm ga = new GeneticAlgorithm(
                    fitnessFunc,cfootprint, 30, 200, 10
                );

            ga.Run();

            Console.Read();


            var ba1 = new BacktestEngine(0, 3010, true);
            ba1.LoadData(QSConstants.DEFAULT_DATA_FILEPATH + "EURUSD1H.xml", "EUR/USD");
            ba1.LoadData(QSConstants.DEFAULT_DATA_FILEPATH + "GBPUSD1H.xml", "GBP/USD");
            //ba.LoadDataLive("EUR/USD", "m5", 200000);
            //ba.LoadDataLive("GBP/USD", "m5", 20000);
            ba1.OrganizeData();
            ba1.LoadStrategy(new Cointegration());
            ba1.Execute();



            Console.Read();
            

            object[,] denseMatrix;
            ExcelUtil.Open(QSConstants.DEFAULT_DATA_FILEPATH + @"GBPUSD1H.xml", out denseMatrix);

            var mData = new List<double>();

            var predictor = new AC(60);

            for (int i = denseMatrix.GetLength(0); i > 1; i--)
            {
                DateTime dateTime = (DateTime)denseMatrix[i, 1];
                var t = new Tick(
                        0,
                        (double)denseMatrix[i, 6],
                        (double)denseMatrix[i, 7],
                        (double)denseMatrix[i, 8],
                        (double)denseMatrix[i, 9],
                        0,
                        (double)denseMatrix[i, 2],
                        (double)denseMatrix[i, 3],
                        (double)denseMatrix[i, 4],
                        (double)denseMatrix[i, 5],
                        (double)denseMatrix[i, 10],
                        dateTime
                        );

                double d = predictor.HandleNextTick(t);
                if (!d.Equals(double.NaN))
                    mData.Add(d);
            }

            int windowSize = 5;
            int iterations = 10000;
            int trainLength = 5000;
            int validateLength = 1000;
            double[] trainData = mData.ToArray().Take(trainLength).ToArray();
            double[] validateData = mData.ToArray().Skip(trainLength).ToArray().Take(validateLength).ToArray();

            Stage1NeuralNetwork nn = new Stage1NeuralNetwork(windowSize, iterations, trainData, validateData);
            nn.Execute(1);

            NeuralNetworkStrategy nns = new NeuralNetworkStrategy(windowSize) { NeuralNetwork = nn };

            var ba = new BacktestEngine(5000, 9200, true);            
            //ba.LoadData(QSConstants.DEFAULT_DATA_FILEPATH + "GBPUSD15M.xml", "EUR/USD");
            ba.LoadData(QSConstants.DEFAULT_DATA_FILEPATH + "NZDUSD1H.xml", "GBP/USD");
            //ba.LoadDataLive("EUR/USD", "m5", 200000);
            //ba.LoadDataLive("GBP/USD", "m5", 20000);
            ba.OrganizeData();
            ba.LoadStrategy(nns);
            ba.Execute();
            

            Console.Read();

            /*
            Func<Chromosome, double> function = (chromosome =>
            {
                double fitness = 0.001;

                ba.ResetAccount();
                ba.ResetStrategies();
                ba.LoadStrategy(new CustomStrategy(
                       (int)(double)chromosome[0].GeneValue,
                       (int)(double)chromosome[1].GeneValue,
                       (int)(double)chromosome[2].GeneValue,
                       (int)(double)chromosome[3].GeneValue,
                       (int)(double)chromosome[4].GeneValue,
                       (int)(double)chromosome[5].GeneValue,
                       (int)(double)chromosome[6].GeneValue,
                       (int)(double)chromosome[7].GeneValue,
                       (int)(double)chromosome[8].GeneValue
                    ));
                ba.Execute();


                fitness += ba.ret;

                return (fitness > 0) ? fitness : 0.001;
            }
                );

            var GA = new GeneticAlgorithm(function,
                new List<Gene>
                {
                    new Gene(50, new Gene.Constraint(5,200)),
                    new Gene(50, new Gene.Constraint(5,200)),
                    new Gene(50, new Gene.Constraint(5,200)),
                    new Gene(50, new Gene.Constraint(5,500)),
                    new Gene(50, new Gene.Constraint(1,100)),
                    new Gene(50, new Gene.Constraint(1,100)),
                    new Gene(50, new Gene.Constraint(5,500)),
                    new Gene(50, new Gene.Constraint(5,500)),
                    new Gene(50, new Gene.Constraint(1,100))
                }) { Generations = 1000, Trials = 10 }
                ;

            GA.InitializePopulation();
            GA.Run();

            Console.Read();
            */




            /*
            object[,] denseMatrix;
            ExcelUtil.Open(Constants.DEFAULT_DATA_FILEPATH + @"EURUSD1H.xml", out denseMatrix);

            var mData = new List<double>();

            var predictor = new RSI(40);

            for (int i = denseMatrix.GetLength(0); i > 1; i--)
            {
                DateTime dateTime = (DateTime) denseMatrix[i, 1];
                var t = new Tick(
                        0,
                        (double) denseMatrix[i, 6],
                        (double) denseMatrix[i, 7],
                        (double) denseMatrix[i, 8],
                        (double) denseMatrix[i, 9],
                        0,
                        (double) denseMatrix[i, 2],
                        (double) denseMatrix[i, 3],
                        (double) denseMatrix[i, 4],
                        (double) denseMatrix[i, 5],
                        (double) denseMatrix[i, 10],
                        dateTime
                        );

                double d = predictor.HandleNextTick(t);
                if (!d.Equals(double.NaN))
                    mData.Add(d);
            }

            int windowSize = 5;
            int iterations = 5000;
            int trainLength = 5000;
            int validateLength = 1000;
            double[] trainData = mData.ToArray().Take(trainLength).ToArray();
            double[] validateData = mData.ToArray().Skip(trainLength).ToArray().Take(validateLength).ToArray();

            Stage1NeuralNetwork nn = new Stage1NeuralNetwork(windowSize, iterations, trainData, validateData);
            nn.Execute(1);

            NeuralNetworkStrategy nns = new NeuralNetworkStrategy(windowSize){NeuralNetwork = nn};
            */

            /*
            bool seao = true;
            if (seao)
            { 

                var ba = new BacktestEngine(0, 50000, true);
                ba.LoadDataLive("EUR/USD", "m30", 80000);
                
                //ba.LoadStrategy(nns);
                ba.LoadStrategy(new RSIEntry(50){LongEntry = 60, ShortEntry = 40});
                ba.LoadStrategy(new RSIExit(50){ LongExit = 70, ShortExit = 30 });

                //ba.LoadStrategy(new Pyramid(.1, .05, .001));
                ba.Execute();

                Console.Read();
            }

            BacktestEngine[] barray = new BacktestEngine[2];

            Func<Chromosome, double> function = (chromosome =>
            {
                double fitness = 0.001;
                foreach (BacktestEngine bx in barray)
                {
                    bx.ResetAccount();
                    bx.ResetStrategies();
                    bx.LoadStrategy(new CustomStrategy());
                    bx.LoadStrategy(new Pyramid((double) chromosome[0].GeneValue,
                        (double) chromosome[1].GeneValue,
                        (double) chromosome[2].GeneValue));
                    bx.Execute();

                    if (bx.ret > 100)
                        fitness += 100;
                    else
                        fitness += bx.ret;
                }

                return (fitness > 0) ? fitness/5 : 0.001;
            }
                );

            var GA = new GeneticAlgorithm(function,
                new List<Gene>
                {
                    new Gene(5, new Gene.Constraint(0,1.0)),
                    new Gene(5, new Gene.Constraint(0,1.0)),
                    new Gene(5, new Gene.Constraint(0,1.0/100.0))
                }) {Generations = 1000, Trials = 1}
                ;

            GA.InitializePopulation();
            GA.Run();

            Console.Read();

            /*
            b.Output = true;
            b.ResetAccount();
            b.ResetStrategies();
            b.LoadStrategy(new RSIEntry()
            {
                EntryTarget1 = (double)GA.maxC[0].GeneValue,
                EntryTarget2 = (double)GA.maxC[1].GeneValue
            });
            b.LoadStrategy(new RSIExit()
            {
                ExitTarget1 = (double)GA.maxC[2].GeneValue,
                ExitTarget2 = (double)GA.maxC[3].GeneValue
            });
            

            b.Execute();
            */
            //Console.Read();
            
        
            //object[,] data;

            /*
            ExcelUtil.Open("C:\\Users\\EL65628\\Work\\QuantSys\\data\\UCUM.xls", out data);
            DenseMatrix d = ExcelUtil.ToMatrix(data, 2, 1283, 1, 2, true);

            DenseVector normchf = Statistics.NormalizeZScore(d.Column(0).ToArray());
            DenseVector normmxn = Statistics.NormalizeZScore(d.Column(1).ToArray());

            DenseVector kurtmxn = new DenseVector(Statistics.AggregateWindow(d.Column(1).ToArray(),
                Statistics.Kurtosis, 50, false, false));
            
            DenseVector corre = new DenseVector((Statistics.AggregateWindow(
                Statistics.RawRateOfReturn(d.Column(0).ToArray()),
                Statistics.RawRateOfReturn(d.Column(1).ToArray()), 
                Statistics.Correlation,
                80, false, true)));

            //DenseMatrix dNew = new DenseMatrix(3, 1282);
            //dNew.SetRow(0, (DenseVector)d.Column(0).Normalize(100));
            //dNew.SetRow(1, (DenseVector)d.Column(1).Normalize(100));
            //dNew.SetRow(2, corre);

            Visualize.GenerateGraph(corre, "C:\\Users\\EL65628\\Work\\QuantSys\\data\\correlation.html");
            Visualize.GenerateGraph(kurtmxn, "C:\\Users\\EL65628\\Work\\QuantSys\\data\\kurtosis.html");
            //Visualize.GenerateGraph(normmxn, "C:\\Users\\EL65628\\Work\\QuantSys\\data\\diff2.html");
            Visualize.GenerateGraph((DenseVector)(Statistics.NormalizeZScore((-1.5 * normmxn + .9 * normchf).ToArray())), "C:\\Users\\EL65628\\Work\\QuantSys\\data\\diff3.html");

            string[] symbols = { "usd/chf", "usd/mxn" ,"correlation"};
            //Visualize.GenerateMultiSymbolGraph(symbols, dNew, new DateTime(), new TimeSpan(1, 0, 0), "C:\\Users\\EL65628\\Work\\QuantSys\\data\\diff.html");
           
            
            int[] vectors = { 5, 3, 2, 4, 6, 7, 8, 9, 10, 13, 14, 15, 16, 17, 18, 19, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54 };

            string vecstring = "1 0 0 0 1 0 1 1 0 0 0 0 1 0 1 0 1 1 1 1 1 0 1 1 1 0 0 0 1 1 1 0 0 0 1 0 0 0 0 1 1 1 1 0 0 0 1 1 0";


            string[] vectemp = vecstring.Split(' ');
            List<int> vec = new List<int>();

            for (int i = 0; i < vectors.Length; i++)
            {
                if (vectemp[i]=="1") vec.Add(vectors[i]);
            }

            //int[] vect = {13, 19, 25, 27 };
            int[] vect = { 17, 18, 22, 27, 40, 49};

            ExcelUtil.Open("C:\\Users\\EL65628\\Work\\QuantSys\\data\\EURUSD1D.xml", out data);
            TwoStageNN twoStageNn = new TwoStageNN(50, 200, data, vect);
            twoStageNn.Execute();
            

            //GeneticAlgorithm g = new GeneticAlgorithm();
            //g.Run();

            Console.ReadLine();

            PortfolioOptimizer.Run();
            */

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            string[] currencies =
            {
                "EUR/USD",
                "GBP/USD",
                "USD/CAD",
                "USD/CHF",
                "AUD/USD",
                "NZD/USD",
                "USD/JPY",
                "USD/MXN",
                "USD/ZAR",
                "USD/PLN",
                "USD/TRY",
                "USD/DKK",
                "USD/SEK",
                "USD/NOK"
            };

            var currencypairs = new List<string[]>();


            for (int i = 0; i < currencies.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (currencies[i] != currencies[j])
                    {
                        string[] temp = {currencies[i], currencies[j]};
                        currencypairs.Add(temp);
                        Console.WriteLine(currencies[i] + " " + currencies[j]);
                    }
                }
            }


            /////////////////////////////////////////////////////////
            /*
            FXSession fxsession1 = new FXSession();

            string[] tempgroup = { "AUD/USD", "NZD/USD" };

            Thread oThread1 = new Thread(new ThreadStart(fxsession1.InitializeSession));
            oThread1.Start();

            while (fxsession1.LoginStatus.Equals(FXSession.LOGIN_STATUS.NOT_LOGGED_IN))
            {
                Thread.Sleep(1000);
                Console.Write(".");
            }

            if (fxsession1.LoginStatus.Equals(FXSession.LOGIN_STATUS.LOGGED_IN))
            {
                //Job_SymbolSet job = new Job_SymbolSet(ex, "H1", 150, new EMA(14));
                //Job_CorrelationMatrix job = new Job_CorrelationMatrix(currencies, "D1", 300, Job_CorrelationMatrix.CovarianceType.RawReturn);

                Job_Cointegration job = new Job_Cointegration(tempgroup, "m30", 300);
                    fxsession1.PlaceJob(job);
                    job.RunJob(fxsession1);
                    Thread.Sleep(1000);
            }

            Console.ReadLine();
            
            */
            //////////////////////////////////////////////////////////////////

            while (true)
            {
                try
                {
                    var fxsession = new FXSession();


                    var oThread = new Thread(fxsession.InitializeSession);
                    oThread.Start();

                    while (!fxsession.LoggedIn)
                    {
                        Thread.Sleep(1000);
                        Console.Write(".");
                    }

                    if (fxsession.LoggedIn)
                    {
                        //Job_SymbolSet job = new Job_SymbolSet(ex, "H1", 150, new EMA(14));
                        //Job_CorrelationMatrix job = new Job_CorrelationMatrix(currencies, "D1", 300, Job_CorrelationMatrix.CovarianceType.RawReturn);

                        //Job_Cointegration.Process(fxsession, "USD/DKK", "USD/CHF", "m30", 3000);
                        //Console.Read();

                        foreach (var group in currencypairs)
                        {
                            if ((!group[0].Substring(0, 3).Equals("USD") && !group[1].Substring(0, 3).Equals("USD")) ||
                                (group[0].Substring(0, 3).Equals("USD") && group[1].Substring(0, 3).Equals("USD")))
                            {

                                try
                                {
                                    Job_Cointegration.Process(fxsession, group[0], group[1], "m30", 3000);
                                    Thread.Sleep(1000);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                    }

                    fxsession.EndSession();
                    oThread.Abort();
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine(DateTime.Now.ToString());

                Thread.Sleep(1000*60*30);
            }

            Console.ReadLine();
        }
    }
}