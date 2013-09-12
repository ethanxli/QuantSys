using System;
using System.Linq;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.RBF;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.RBF;
using Encog.Util.Arrayutil;
using MathNet.Numerics.LinearAlgebra.Double;
using QuantSys.Analytics;
using QuantSys.Util;
using QuantSys.Visualization;

namespace QuantSys.MachineLearning.NeuralNetwork
{
    public class Stage2NeuralNetwork
    {
        private const double normalizeLo = -1;
        private const double normalizeHi = 1;
        private readonly int inputs;
        private readonly int iterations;

        private readonly NormalizeArray[] normalizeArrayInput;
        private readonly NormalizeArray normalizeArrayOutput;

        private readonly double[] predictionData;
        private readonly DenseMatrix trainingData;

        public double[] OutputData;
        private double[] _normalizedPredictionData;
        private DenseMatrix _normalizedTrainingData;
        public BasicNetwork network;

        public double outputCorre;
        public double outputRMSE;

        public Stage2NeuralNetwork(int inputs, int iterations, DenseMatrix trainingData, double[] predictData)
        {
            this.inputs = inputs;
            this.iterations = iterations;
            this.trainingData = trainingData;
            predictionData = predictData;
            normalizeArrayInput = new NormalizeArray[trainingData.RowCount];
            for (int i = 0; i < normalizeArrayInput.Length; i++)
                normalizeArrayInput[i] = new NormalizeArray {NormalizedHigh = normalizeHi, NormalizedLow = normalizeLo};
            normalizeArrayOutput = new NormalizeArray {NormalizedHigh = normalizeHi, NormalizedLow = normalizeLo};
        }

        public void Execute()
        {
            _normalizedTrainingData = NormalizeData(trainingData);
            _normalizedPredictionData = NormalizeData(predictionData);
            network = CreateNetwork();
            IMLDataSet training = GenerateTraining();
            Train(training);
            Predict();
        }


        public DenseMatrix NormalizeData(DenseMatrix data)
        {
            var normalizedData = new DenseMatrix(data.RowCount, data.ColumnCount);

            for (int i = 0; i < data.RowCount; i++)
            {
                normalizedData.SetRow(i, normalizeArrayInput[i].Process(data.Row(i).ToArray()));
            }

            return normalizedData;
        }

        public double[] NormalizeData(double[] data)
        {
            return normalizeArrayOutput.Process(data);
        }


        public BasicNetwork CreateNetwork()
        {
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(inputs));
            network.AddLayer(new BasicLayer(new ActivationLinear(), true, inputs/2 + 1));
            network.AddLayer(new BasicLayer(1));
            network.Structure.FinalizeStructure();
            network.Reset();

            /////////////////////////////////

            var rbfnetwork = new RBFNetwork(inputs, inputs/2 + 1, 1, RBFEnum.Gaussian);

            return network;
        }

        public IMLDataSet GenerateTraining()
        {
            var input = new double[trainingData.ColumnCount][];
            var ideal = new double[_normalizedPredictionData.Length][];

            for (int i = 0; i < trainingData.ColumnCount; i++)
            {
                input[i] = _normalizedTrainingData.Column(i).ToArray();
                double[] predict = {_normalizedPredictionData[i]};
                ideal[i] = predict;
            }

            var result = new BasicMLDataSet(input, ideal);
            return result;
        }


        public void Train(IMLDataSet training)
        {
            ITrain train = new ResilientPropagation(network, training);
            //SVDTraining train = new SVDTraining(network, training);

            int epoch = 1;

            do
            {
                train.Iteration();
                if ((epoch)%(iterations/10) == 0) Console.Write(".");
                epoch++;
            } while (epoch < iterations*100);
        }


        public void Predict()
        {
            double error = 0;
            int c = 0;

            var d = new DenseMatrix(2, _normalizedPredictionData.Length);
            int count = 0;
            for (int i = 0; i < _normalizedPredictionData.Length; i++)
            {
                // calculate based on actual data
                IMLData input = new BasicMLData(inputs);
                for (int j = 0; j < input.Count; j++)
                {
                    input.Data[j] = _normalizedTrainingData[j, i];
                }

                IMLData output = network.Compute(input);
                double prediction = output.Data[0];


                error +=
                    Math.Pow(
                        (normalizeArrayOutput.Stats.DeNormalize(prediction) - predictionData[i])/predictionData[i], 2);
                c++;
                d[0, count] = predictionData[i];
                d[1, count] = normalizeArrayOutput.Stats.DeNormalize(prediction);
                count++;
            }

            error /= c;
            error = Math.Pow(error, .5);
            Console.WriteLine(error);

            string[] symbols = {"actual", "predicted"};
            Visualize.GeneratePredictionGraph(symbols, d, new DateTime(), new TimeSpan(24, 0, 0),
                "C:\\Sangar\\resultfinal.html");
        }


        public void Predict(DenseMatrix newPredictData, double[] newPredictPrices)
        {
            double error = 0;
            int c = 0;

            var newNormalizedPredictData = new DenseMatrix(newPredictData.RowCount, newPredictData.ColumnCount,
                double.NaN);

            for (int i = 0; i < newPredictData.RowCount; i++)
            {
                newNormalizedPredictData.SetRow(i, normalizeArrayInput[i].Process(newPredictData.Row(i).ToArray()));
            }

            double[] normalizedPrices = normalizeArrayOutput.Process(newPredictPrices);

            var d = new DenseMatrix(2, normalizedPrices.Length + 1, double.NaN);
            int count = 0;
            for (int i = 0; i < normalizedPrices.Length; i++)
            {
                // calculate based on actual data
                IMLData input = new BasicMLData(inputs);
                for (int j = 0; j < input.Count; j++)
                {
                    input.Data[j] = newNormalizedPredictData[j, i];
                }

                IMLData output = network.Compute(input);
                double prediction = output.Data[0];


                error +=
                    Math.Pow(
                        (normalizeArrayOutput.Stats.DeNormalize(prediction) - newPredictPrices[i])/newPredictPrices[i],
                        2);
                c++;
                d[0, count] = newPredictPrices[i];
                d[1, count] = normalizeArrayOutput.Stats.DeNormalize(prediction);
                count++;
            }

            /////////////////////////////////////////////////////////////////

            IMLData input1 = new BasicMLData(inputs);
            for (int j = 0; j < input1.Count; j++)
            {
                input1.Data[j] = newNormalizedPredictData[j, newNormalizedPredictData.ColumnCount - 1];
            }

            IMLData output1 = network.Compute(input1);
            d[1, count] = normalizeArrayOutput.Stats.DeNormalize(output1.Data[0]);


            /////////////////////////////////////////////////////////////////


            error /= c;
            error = Math.Pow(error, .5);
            Console.WriteLine(error);

            string[] symbols = {"actual", "predicted"};
            Visualize.GeneratePredictionGraph(symbols, d, new DateTime(), new TimeSpan(24, 0, 0),
                "C:\\Sangar\\resultfinal.html");

            outputCorre =
                StatisticsExtension.Correlation(d.Row(0).ToArray().Take(d.ColumnCount - 1).ToArray().RawRateOfReturn(),
                    d.Row(1).ToArray().Take(d.ColumnCount - 1).ToArray().RawRateOfReturn());

            Console.WriteLine("ST2 Correlation: " + outputCorre);

            outputRMSE = error;

            Console.WriteLine("Predicted return for D+1:" +
                              (d[1, d.ColumnCount - 1] - d[1, d.ColumnCount - 2])/d[1, d.ColumnCount - 2]*100 +
                              " percent");
        }
    }
}