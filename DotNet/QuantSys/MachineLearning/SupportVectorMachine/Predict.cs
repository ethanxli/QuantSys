using System;
using System.Collections.Generic;
using libsvm;

namespace QuantSys.MachineLearning.SupportVectorMachine
{
    public class Predict
    {
        public static string TRAINING_FILE = "C:/Sangar/quantopian/svm/heart_scale";
        public static string TEST_FILE = "C:/Sangar/quantopian/svm/heart_scale";
        public static double gamma = 1.0;
        public static double C = 1.0;
        public static int nr_fold = 5;

        private static readonly svm_problem prob = ProblemHelper.ReadAndScaleProblem(TRAINING_FILE);
        private static readonly svm_problem test = ProblemHelper.ReadAndScaleProblem(TEST_FILE);

        public static void SVMPredict()
        {
            var svm = new C_SVC(prob, KernelHelper.RadialBasisFunctionKernel(gamma), C);
            double accuracy = svm.GetCrossValidationAccuracy(nr_fold);

            for (int i = 0; i < test.l; i++)
            {
                svm_node[] x = test.x[i];
                double y = test.y[i];
                double predict = svm.Predict(x); // returns the predicted value 'y'
                Dictionary<int, double> probabilities = svm.PredictProbabilities(x);
                    // returns the probabilities for each 'y' value
                Console.WriteLine(predict + " :" + probabilities[1]);
            }
            Console.ReadKey();
        }
    }
}