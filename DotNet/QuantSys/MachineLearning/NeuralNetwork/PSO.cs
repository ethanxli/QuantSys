using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.Randomize;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.PSO;
using Encog.Util.Error;

namespace QuantSys.MachineLearning.NeuralNetwork
{
    class PSO
    {
        private IMLTrain train;
        private BasicNetwork network;
        public PSO()
        {
            network = new BasicNetwork();
            network.AddLayer(new BasicLayer(5));
            network.AddLayer(new BasicLayer(1));
            network.AddLayer(new BasicLayer(1));
            network.Structure.FinalizeStructure();
            network.Reset();

            IMLDataSet dataSet = new BasicMLDataSet();
            dataSet.Add(new BasicMLData(new double[] { 1.0, 4.0, 3.0, 4.0, 5.0}) , new BasicMLData(new double[] { 2.0, 4.0, 6.0 , 8.0, 10} ));
            
            train = new NeuralPSO(network, new RangeRandomizer(0, 10), new TrainingSetScore(dataSet),5);
            
        }

        public void Iterate()
        {
            train.Iteration();
        }

        public void Compute()
        {
            var output = network.Compute(new BasicMLData(new double[] {0.5, 1, 1.5, 2, 2.5}));
        }



    }
}
