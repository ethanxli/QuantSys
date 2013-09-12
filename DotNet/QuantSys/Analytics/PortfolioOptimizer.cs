using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.SolverFoundation.Services;
using Microsoft.SolverFoundation.Solvers;
using QuantSys.TradeEngine.Simulation.Account;

namespace QuantSys.Analytics
{
    /* FX Optimal Risky Portfolio
     * Rebalance at the beginning of every market
     * stdev: vol of currencies based on time of day average for last x days
     * 3AM - London Market
     * 8AM - NY Market
     * 7PM - TOK/SYD Market
     * 
     * Trades
     * USD/JPY
     * USD/CHF
     * GBP/USD
     * EUR/USD
     * NZD/USD
     * AUD/USD
     * USD/CAD
     */

    internal class PortfolioOptimizer
    {
        private DenseMatrix covariance;
        private double minReturn;
        private Portfolio portfolio;

        public double resultReturn;
        public double resultStdev;
        public DenseVector resultWeights;
        private DenseVector returns;

        public PortfolioOptimizer(Portfolio p, double minReturn, DenseMatrix cov, DenseVector returns)
        {
            portfolio = p;
            this.returns = returns;
            covariance = cov;
            this.minReturn = minReturn;
            resultWeights = new DenseVector(returns.Count);
        }

        public static void Run()
        {
            List<Position> list = new List<Position>();

            /*
            list.Add(new Position());
            list.Add(new Position("bond"));
            list.Add(new Position("stock"));
             * */
            Portfolio p = new Portfolio();

            double[] returns = {0.000, 0.13, -0.13};
            DenseVector returns1 = new DenseVector(returns);

            double[] stdev = {0, 7.4, 7.4};
            double[,] covariance = {{1, -.4, -.45}, {-.4, 1, .35}, {-0.45, 0.35, 1}};

            DenseMatrix covariance1 = StatisticsExtension.CorrelationToCovariance(new DenseMatrix(covariance),
                                                                                  new DenseVector(stdev));


            PortfolioOptimizer po = new PortfolioOptimizer(p, .09002, covariance1, returns1);
            po.BuildRiskModel();
            Console.ReadLine();
        }


        public bool BuildRiskModel()
        {
            int m = portfolio.NumPositions;

            InteriorPointSolver solver = new InteriorPointSolver();

            int[] allocations = new int[m];
            int counter = 0;

            foreach (Position p in portfolio.Positions.Values)
            {
                solver.AddVariable(p.Symbol, out allocations[counter]);
                solver.SetBounds(allocations[counter], -1, 1);
                counter++;
            }

            int expectedReturn;
            solver.AddRow("expectedReturn", out expectedReturn);

            // expected return must beat the minimum asked

            solver.SetBounds(expectedReturn, minReturn, double.PositiveInfinity);

            int unity;
            solver.AddRow("Investments sum to one", out unity);
            solver.SetBounds(unity, -1, 1);

            // expected return is a weighted linear combination of investments.
            // unity is a simple sum of the investments

            for (int invest = m; 0 <= --invest;)
            {
                solver.SetCoefficient(expectedReturn, allocations[invest], returns[invest]);
                solver.SetCoefficient(unity, allocations[invest], 1);
            }

            // The variance of the result is a quadratic combination of the covariants and allocations.

            int variance;
            solver.AddRow("variance", out variance);
            for (int invest = m; 0 <= --invest;)
            {
                for (int jnvest = m; 0 <= --jnvest;)
                {
                    solver.SetCoefficient(variance, covariance[invest, jnvest], allocations[invest], allocations[jnvest]);
                }
            }

            // the goal is to minimize the variance, given the linear lower bound on asked return.

            solver.AddGoal(variance, 0, true);

            InteriorPointSolverParams lpParams = new InteriorPointSolverParams();

            solver.Solve(lpParams);
            if (solver.Result != LinearResult.Optimal)
                return false;

            for (int i = 0; i < m; i++)
            {
                
            }

            counter = 0;
            foreach (Position p in portfolio.Positions.Values)
            {
                Console.Write(p.Symbol + " " + (double)solver.GetValue(allocations[counter++]) + " ");
                Console.WriteLine();

            }

            Console.WriteLine((double) solver.GetValue(expectedReturn) + " " +
                              Math.Sqrt((double) solver.Statistics.Primal) + "\n");


            return true;
        }
    }
}