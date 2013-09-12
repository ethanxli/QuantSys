using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using QuantSys.Analytics;
using QuantSys.Analytics.StatisticalModeling;

namespace QuantSys.MachineLearning.NeuralNetwork
{
    public class TwoStageNN
    {
        private readonly int cycles;
        private readonly object[,] data;
        private readonly int[] vectors;
        private readonly int window;

        public double RMSE;
        public double correlation;

        public TwoStageNN(int window, int cycles, object[,] data, int[] vectors)
        {
            this.window = window;
            this.cycles = cycles;
            this.data = data;
            this.vectors = vectors;
        }

        public void Execute()
        {
            var nnSet = new Stage1NeuralNetwork[vectors.Length];

            int trainLength = 2000;
            int validationLength = 500;
            int predictLength = 500;
            int useLength = 3000;

            var totalData = new DenseMatrix(vectors.Length, useLength);
            var outputData = new DenseMatrix(vectors.Length, validationLength - window);

            /////////////////populate the actual price data we want to predict
            var pricingData = new double[useLength];

            for (int i = 2; i < 2 + useLength; i++)
            {
                pricingData[i - 2] = (double) data[useLength + 3 - i, 5];
            }


            double[] returnpricingData = pricingData.RawRateOfReturn();
            for (int i = 0; i < returnpricingData.Length; i++) pricingData[i + 1] = returnpricingData[i];
            pricingData[0] = 0;


            ////////////////////////training and validation////////////////////////
            for (int i = 2; i < 2 + useLength; i++)
            {
                for (int j = 0; j < vectors.Length; j++)
                {
                    totalData[j, i - 2] = (double) data[useLength + 3 - i, vectors[j]];
                }
            }


            for (int j = 0; j < vectors.Length; j++)
            {
                double[] train = totalData.Row(j).ToArray().Take(trainLength).ToArray();
                double[] validate =
                    totalData.Row(j).ToArray().Skip(trainLength).ToArray().Take(validationLength).ToArray();
                nnSet[j] = new Stage1NeuralNetwork(window, cycles, train, validate);
                nnSet[j].Execute(j);
                outputData.SetRow(j, nnSet[j].OutputData);
            }

            var s1 = new Stage2NeuralNetwork(vectors.Length, cycles, outputData,
                pricingData.Skip(trainLength).ToArray().Take(validationLength).ToArray().Skip(window).ToArray());
            s1.Execute();

            //////////////////////////////////////////////////////////////////////////
            //////////////////////////////////prediction/////////////////////////////
            var predictedData = new DenseMatrix(vectors.Length, predictLength - window + 1);

            var lastPredData = new double[vectors.Length];

            for (int j = 0; j < vectors.Length; j++)
            {
                double[] predictData =
                    totalData.Row(j)
                        .ToArray()
                        .Skip(trainLength + validationLength)
                        .ToArray()
                        .Take(predictLength)
                        .ToArray();
                nnSet[j].Predict(predictData);
                predictedData.SetRow(j, nnSet[j].OutputData);
                lastPredData[j] = nnSet[j].NextPrediction;
            }

            s1.Predict(predictedData,
                pricingData.ToArray()
                    .Skip(trainLength + validationLength)
                    .ToArray()
                    .Take(predictLength)
                    .ToArray()
                    .Skip(window)
                    .ToArray());

            correlation = s1.outputCorre;
            RMSE = s1.outputRMSE;
        }
    }
}