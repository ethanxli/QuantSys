//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//

using System;
using System.Linq;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.RBF;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Temporal;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.SCG;
using Encog.Neural.RBF;
using Encog.Util.Arrayutil;
using MathNet.Numerics.LinearAlgebra.Double;
using QuantSys.Analytics;
using QuantSys.Analytics.StatisticalModeling;
using QuantSys.Util;
using QuantSys.Visualization;

namespace QuantSys.MachineLearning.NeuralNetwork
{
    public class Stage1NeuralNetwork
    {
        private const double normalizeLo = -1;
        private const double normalizeHi = 1;

        private readonly int WindowSize;
        private readonly int iterations;
        private readonly NormalizeArray normalizeArray;
        private readonly double[] predictionData;
        private readonly double[] trainingData;

        public double[] OutputData;
        private double[] _normalizedPredictionData;
        private double[] _normalizedTrainingData;
        public BasicNetwork network;

        public double NextPrediction;
        private int write;

        public Stage1NeuralNetwork(int windowSize, int iterations, double[] trainingData, double[] predictData)
        {
            WindowSize = windowSize;
            this.iterations = iterations;
            this.trainingData = trainingData;
            predictionData = predictData;
            normalizeArray = new NormalizeArray {NormalizedHigh = normalizeHi, NormalizedLow = normalizeLo};
        }

        public void Execute(int write)
        {
            this.write = write;
            _normalizedTrainingData = NormalizeData(trainingData);
            _normalizedPredictionData = NormalizeData(predictionData);
            network = CreateNetwork();
            IMLDataSet training = GenerateTraining();
            Train(training);
            Predict();
        }


        public double[] NormalizeData(double[] data)
        {
            return normalizeArray.Process(data);
        }


        public BasicNetwork CreateNetwork()
        {
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(WindowSize));
            network.AddLayer(new BasicLayer(new ActivationLinear(), true, WindowSize/2 + 1));
            network.AddLayer(new BasicLayer(1));
            network.Structure.FinalizeStructure();
            network.Reset();

            var rbfnetwork = new RBFNetwork(WindowSize, WindowSize/2 + 1, 1, RBFEnum.Gaussian);


            return network;
        }

        public IMLDataSet GenerateTraining()
        {
            var result = new TemporalMLDataSet(WindowSize, 1);
            var desc = new TemporalDataDescription(TemporalDataDescription.Type.Raw, true, true);
            result.AddDescription(desc);

            for (int i = WindowSize; i < _normalizedTrainingData.Length; i++)
            {
                var point = new TemporalPoint(1) {Sequence = i};
                point.Data[0] = _normalizedTrainingData[i];
                result.Points.Add(point);
            }
            result.Generate();
            return result;
        }


        public void Train(IMLDataSet training)
        {
            //SVDTraining train = new SVDTraining(network, training);
            ITrain train = new ScaledConjugateGradient(network, training);


            int epoch = 1;

            do
            {
                train.Iteration();
                if ((epoch)%(iterations/10) == 0) Console.Write(".");
                epoch++;
            } while (epoch < iterations);
        }


        public void Predict()
        {
            double error = 0;
            int c = 0;

            var d = new DenseMatrix(2, _normalizedPredictionData.Length - WindowSize);
            int count = 0;
            for (int i = WindowSize; i < _normalizedPredictionData.Length; i++)
            {
                // calculate based on actual data
                IMLData input = new BasicMLData(WindowSize);
                for (int j = 0; j < input.Count; j++)
                {
                    input.Data[j] = _normalizedPredictionData[(i - WindowSize) + j];
                }

                IMLData output = network.Compute(input);
                double prediction = output.Data[0];


                error += Math.Pow((normalizeArray.Stats.DeNormalize(prediction) - predictionData[i])/predictionData[i],
                    2);
                c++;
                d[0, count] = predictionData[i];
                d[1, count] = normalizeArray.Stats.DeNormalize(prediction);
                count++;
            }

            error /= c;
            error = Math.Pow(error, .5);
            Console.WriteLine(error);

            OutputData = d.Row(1).ToArray();

            string[] symbols = {"actual", "predicted"};
            Visualize.GeneratePredictionGraph(symbols, d, new DateTime(), new TimeSpan(24, 0, 0),
                QSConstants.DEFAULT_DATA_FILEPATH + write + ".html");
        }



        public void Predict(double[] newPredictData)
        {
            double error = 0;
            int c = 0;

            double[] newNormalizedData = normalizeArray.Process(newPredictData);

            var d = new DenseMatrix(2, newNormalizedData.Length - WindowSize + 1, double.NaN);
            int count = 0;
            for (int i = WindowSize; i < newNormalizedData.Length; i++)
            {
                // calculate based on actual data
                IMLData input = new BasicMLData(WindowSize);
                for (int j = 0; j < input.Count; j++)
                {
                    input.Data[j] = newNormalizedData[(i - WindowSize) + j];
                }

                IMLData output = network.Compute(input);
                double prediction = output.Data[0];


                error += Math.Pow((normalizeArray.Stats.DeNormalize(prediction) - newPredictData[i])/newPredictData[i],
                    2);
                c++;
                d[0, count] = newPredictData[i];
                d[1, count] = normalizeArray.Stats.DeNormalize(prediction);
                count++;
            }

            ///////////////////////////////////////////////////////////////////////////////

            var lastData = new double[WindowSize];
            int count1 = 0;
            for (int i = newNormalizedData.Length - WindowSize; i < newNormalizedData.Length; i++)
            {
                lastData[count1++] = newNormalizedData[i];
            }
            IMLData input1 = new BasicMLData(WindowSize);

            for (int j = 0; j < input1.Count; j++)
            {
                input1.Data[j] = lastData[j];
            }


            IMLData output1 = network.Compute(input1);
            d[1, count] = normalizeArray.Stats.DeNormalize(output1.Data[0]);


            /////////////////////////////////////////////////////////////////////////////////


            error /= c;
            error = Math.Pow(error, .5);
            Console.WriteLine(error);

            OutputData = d.Row(1).ToArray();

            string[] symbols = {"actual", "predicted"};
            Visualize.GeneratePredictionGraph(symbols, d, new DateTime(), new TimeSpan(24, 0, 0),
                QSConstants.DEFAULT_DATA_FILEPATH + write + ".html");

            Console.WriteLine("ST1 Correlation: " +
                              StatisticsExtension.Correlation(
                                  d.Row(0).ToArray().Take(d.ColumnCount - 1).ToArray().RawRateOfReturn(),
                                  d.Row(1).ToArray().Take(d.ColumnCount - 1).ToArray().RawRateOfReturn()));
        }


        public double PredictNext(double[] inputData)
        {

            double[] newNormalizedData = normalizeArray.Process(inputData);
            IMLData input = new BasicMLData(WindowSize);
            input.Data = newNormalizedData;
            IMLData output1 = network.Compute(input);
            double outputDenormalized = normalizeArray.Stats.DeNormalize(output1.Data[0]);
            return outputDenormalized;
        }

    }
}