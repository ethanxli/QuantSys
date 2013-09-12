namespace QuantSys.Analytics.StatisticalModeling
{
    public class ARMA
    {
        /*
        public virtual void FitByMLE(int numIterationsLDS, int numIterationsOpt, 
            double consistencyPenalty,
            Optimizer.OptimizationCallback optCallback)
        {
            var thisAsMLEEstimable = this as IMLEEstimable;
            if (thisAsMLEEstimable == null)
                throw new ApplicationException("MLE not supported for this model.");

            int optDimension = NumParametersOfType(Model.ParameterState.Free);
            int numConsequential = NumParametersOfType(Model.ParameterState.Consequential);
            int numIterations = numIterationsLDS + numIterationsOpt;

            var trialParameterList = new Vector[numIterationsLDS];
            var trialCubeList = new Vector[numIterationsLDS];

            var hsequence = new HaltonSequence(optDimension);

            for (int i = 0; i < numIterationsLDS; ++i)
            {
                Vector smallCube = hsequence.GetNext();
                Vector cube = CubeInsert(smallCube);
                trialParameterList[i] = thisAsMLEEstimable.CubeToParameter(cube);
                trialCubeList[i] = cube;
            }

            var logLikes = new double[numIterationsLDS];
            for (int i = 0; i < numIterationsLDS; ++i)
            {
                Vector tparms = trialParameterList[i];
                if (numConsequential > 0)
                {
                    tparms = ComputeConsequentialParameters(tparms);
                    trialParameterList[i] = tparms;
                }

                double ll = LogLikelihood(tparms, consistencyPenalty);
                logLikes[i] = ll;

                if (optCallback != null)
                    lock (logLikes)
                        optCallback(tparms, ll, i*100/numIterations, false);
            }

            // Step 1: Just take the best value.
            Array.Sort(logLikes, trialParameterList);
            var Parameters = trialParameterList[numIterationsLDS - 1];

            // Step 2: Take some of the top values and use them to create a simplex, then optimize
            // further in natural parameter space with the Nelder Mead algorithm.
            // Here we optimize in cube space, reflecting the cube when necessary to make parameters valid.
            var simplex = new List<Vector>();
            for (int i = 0; i <= optDimension; ++i)
                simplex.Add(FreeParameters(thisAsMLEEstimable.ParameterToCube(
                trialParameterList[numIterationsLDS - 1 - i])));
            var nmOptimizer = new NelderMead {Callback = optCallback, StartIteration = numIterationsLDS};
            currentPenalty = consistencyPenalty;
            nmOptimizer.Minimize(NegativeLogLikelihood, simplex, numIterationsOpt);
            Parameters = ComputeConsequentialParameters(
              thisAsMLEEstimable.CubeToParameter(CubeFix(CubeInsert(nmOptimizer.ArgMin))));

            ComputeResidualsAndOutputs();
        }
         */ 
    }



}
