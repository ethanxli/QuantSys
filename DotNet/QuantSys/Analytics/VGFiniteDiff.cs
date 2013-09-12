using System;
using System.Collections.Generic;
using System.Text;
//using NumericalRecipes;

namespace VGModelSandBox
{
    public class VGFiniteDiff
    {
        //Private class member variables required for calculation
        private double lambda_n;
        private double lambda_p;
        private double K, r, q, sigma, nu, theta, T, deltT, deltS, Aj, Bj, Cj, Rj, Hj, fj, omega, a, Bn, Bp, xmin, xmax;
        private int u, N, M;
        private double[,] mainGrid;
        private double[] optPrices, y2;
        private double[] expintN;
        private double[] expintP;
        private double[] expLambdaN;
        private double[] expLambdaP;
        private double[] x_index;
        private double[] spotPrice;

        public VGFiniteDiff(double X, double r, double q, double sigma, double nu, double theta, double T, int u, int N, int M)
        {
            //initialization of global variables
            /* My code uses a grid structure with stock prices running on the y-axis, starting from xmin at the origin and going up
             * to xmax. The time steps run on the x-axis, with the origin representing today (Time=0) and the running along the x-axis
             * away from the origin to the right going to Time=T. Also, please be aware that my labeling is based on Hull's grid labels,
             * and not the Hirsa, Madan paper. As such, M=stock steps, and N=time steps in my code. The variable j controls the y-axis
             * movement along the stock grid, and i controls the time movement along the x-axis. I assume that that we start at time T
             * where i=T and we solve for the option prices at i-1 going backward to i=0. Also, to be computational efficient, I only keep
             * two columns of the main grid matrix in memory at any time, since you only need your current i (the time step you're solving for)
             * and i+1. The option price grid is called mainGrid, and all option values during the calculation are stored there. Don't be
             * fooled by the vector called optPrices; this vector is only used at the last step to load all the finished option prices from
             * time i=0 so that it can be passed to cubic spline method.
             */ 
            this.K = X;
            this.r = r;
            this.q = q;
            this.sigma = sigma;
            this.nu = nu;
            this.theta = theta;
            this.T = T;
            this.u = u;
            this.N = N;
            this.M = M;
            omega = (1 / nu) * Math.Log(1 - theta * nu - Math.Pow(sigma, 2) * nu / 2);
            deltT = T / N;
            xmax = Math.Log(X * 2);
            xmin = Math.Log(X / 3);
            deltS = (xmax - xmin) / M;
            mainGrid = new double[M + 1, 2];
            optPrices = new double[M + 1];
            y2 = new double[M + 1];
            x_index = new double[M + 1];
            for (int k = 0; k < M + 1; k++)
                x_index[k] = xmin + (k * deltS);

            spotPrice = new double[M + 1];
            lambda_n = Math.Pow((Math.Pow(theta, 2) / Math.Pow(sigma, 4)) + (2 / (sigma * sigma * nu)), 0.5) + (theta / (sigma * sigma));
            lambda_p = Math.Pow((Math.Pow(theta, 2) / Math.Pow(sigma, 4)) + (2 / (sigma * sigma * nu)), 0.5) - (theta / (sigma * sigma));
            a = (r - q + omega) * (deltT / (2 * deltS));
            Bn = (deltT / (nu * deltS * lambda_n)) * (1 - Math.Exp(-lambda_n * deltS));
            Bp = (deltT / (nu * deltS * lambda_p)) * (1 - Math.Exp(-lambda_p * deltS));
            PrepPreCalc();
            FinalPayoff();
        }

        private void PrepPreCalc()
        {
            //Pre-calculates the four main vectors based on expint
            expintN = new double[M + 1];
            expintP = new double[M + 1];
            expLambdaN = new double[M + 1];
            expLambdaP = new double[M + 1];

            for (int k = 0; k < M + 1; k++)
            {
                expintN[k] = NR.Expint(1, (k + 1) * deltS * lambda_n);
                expintP[k] = NR.Expint(1, (k + 1) * deltS * lambda_p);
                expLambdaN[k] = Math.Exp(-lambda_n * (k + 1) * deltS);
                expLambdaP[k] = Math.Exp(-lambda_p * (k + 1) * deltS);
                spotPrice[k] = Math.Exp(x_index[k]);
            }
        }

        private void FinalPayoff()
        {
            //This method builds the terminal payoff boundary on the grid
            if (u == 0)
            {
                for (int j = 0; j <= M; j++)
                    mainGrid[j, 1] = Math.Max(0, K - spotPrice[j]);
            }
            else
            {
                for (int j = M; j >= 0; j--)
                    mainGrid[j, 1] = Math.Max(0, spotPrice[j] - K);
            }
        }

        private double vgB(int j)
        {
            //Calculates the B coefficient at each step
            double u1 = NR.Expint(1, j * deltS * lambda_n);
            double u2 = NR.Expint(1, (M - j) * deltS * lambda_p);

            return 1 + r * deltT + Bn + Bp + (deltT / nu) * (u1 + u2);
        }

        private double vgR(int j, int i)
        {
            //Calculates the R coefficient at each step
            double sum1 = 0, sum2 = 0, sum3 = 0, sum4 = 0;

            for (int k = 1; k <= j - 1; k++)
                sum1 += (mainGrid[j - k, i] - mainGrid[j, i] - k * (mainGrid[j - k - 1, i] - mainGrid[j - k, i])) * (expintN[k - 1] - expintN[k]);
            for (int k = 1; k <= j - 1; k++)
                sum2 += ((mainGrid[j - k - 1, i] - mainGrid[j - k, i]) / (lambda_n * deltS)) * (expLambdaN[k - 1] - expLambdaN[k]);
            for (int k = 1; k <= M - j - 1; k++)
                sum3 += (mainGrid[j + k, i] - mainGrid[j, i] - k * (mainGrid[j + k + 1, i] - mainGrid[j + k, i])) * (expintP[k - 1] - expintP[k]);
            for (int k = 1; k <= M - j - 1; k++)
                sum4 += ((mainGrid[j + k + 1, i] - mainGrid[j + k, i]) / (lambda_p * deltS)) * (expLambdaP[k - 1] - expLambdaP[k]);

            return sum1 + sum2 + sum3 + sum4 + (K * NR.Expint(1, j * deltS * lambda_n)) - (spotPrice[j] * NR.Expint(1, j * deltS * (lambda_n + 1)));
        }

        private double x_j(int i)
        {
            //Calculates the exercise boundary for each step
            double xi, min_xi = x_index[M], test;

            if (i == N) return Math.Log(K);

            for (int j = M; j >= 0; j--)
            {
                xi = x_index[j];
                test = mainGrid[j, i] - Math.Max(0, K - spotPrice[j]);

                if (test >= 0)
                {
                    min_xi = xi;
                }
                else
                {
                    break;
                }
            }
            return min_xi;
        }

        private double vgH(int l, int j, int i)
        {
            //Calculates the Heaviside term at each step, takes the exercise boundary as input from the x_j function
            double sum1 = 0, sum2 = 0;
            double u1 = r * K - q * spotPrice[j];

            if (l <= j) return 0;

            for (int k = l - j; k <= M - j - 1; k++)
                sum1 += (1 / nu) * (mainGrid[j + k, i] - k * (mainGrid[j + k + 1, i] - mainGrid[j + k, i])) * (expintP[k - 1] - expintP[k]);
            for (int k = l - j; k <= M - j - 1; k++)
                sum2 += ((mainGrid[j + k + 1, i] - mainGrid[j + k, i]) / (lambda_p * nu * deltS)) * (expLambdaP[k - 1] - expLambdaP[k]);

            double u2 = (1 / nu) * (K * NR.Expint(1, (l - j) * deltS * lambda_p) - spotPrice[j] * NR.Expint(1, (l - j) * deltS * (lambda_p - 1)));

            return u1 - sum1 - sum2 + u2;
        }

        public void BuildGrid()
        {
            //Builds the main grid
            double testXi, xi;
            int heavySwitch, l;
            double[] A, B, C, rh, sol;

            //Creates the boundary conditions running on the top and bottom of the grid
            for (int i = 0; i <= 1; i++)
            {
                if (u == 1)
                {
                    mainGrid[0, i] = 0.0;
                    mainGrid[M, i] = spotPrice[M] - K;
                }
                else
                {
                    mainGrid[0, i] = K - spotPrice[0];
                    mainGrid[M, i] = 0.0;
                }
            }

            for (int i = N - 1; i >= 0; i--) //This is time step backward recurssion
            {
                A = new double[M - 1];
                B = new double[M - 1];
                C = new double[M - 1];
                rh = new double[M - 1];
                for (int j = 1; j < M; j++) //At each time step, run up the stock axis
                {
                    xi = x_index[j];
                    Aj = a - Bn;
                    Bj = vgB(j);
                    Cj = a + Bp;
                    fj = mainGrid[j, 1];
                    Rj = vgR(j, 1);
                    testXi = x_j(1);
                    l = (int)Math.Floor(((testXi - xmin) / deltS));
                    Hj = vgH(l, j, 1);
                    heavySwitch = xi < testXi ? 1 : 0;

                    if ((j != 1) && (j != M - 1))
                    {
                        rh[j - 1] = fj + ((deltT / nu) * Rj) + (deltT * heavySwitch * Hj);
                        A[j - 1] = Aj;
                        B[j - 1] = Bj;
                        C[j - 1] = -Cj;
                    }
                    else
                    {
                        if (j == 1)
                        {
                            rh[j - 1] = (fj + ((deltT / nu) * Rj) + (deltT * heavySwitch * Hj)) - (Aj * mainGrid[j - 1, 0]);
                            B[j - 1] = Bj;
                            C[j - 1] = -Cj;
                        }
                        else if (j == M - 1)
                        {
                            rh[j - 1] = (fj + ((deltT / nu) * Rj) + (deltT * heavySwitch * Hj)) + (Cj * mainGrid[j + 1, 0]);
                            A[j - 1] = Aj;
                            B[j - 1] = Bj;
                        }
                    }
                }
                NR.TriDiag(ref A, ref B, ref C, ref rh, out sol);
                for (int n = 1; n < M; n++)
                {
                    //Checks against parity violations
                    if (u == 1)
                        mainGrid[n, 0] = sol[n - 1];
                    else
                        mainGrid[n, 0] = sol[n - 1] < K - spotPrice[n] ? K - spotPrice[n] : sol[n - 1];
                    //Recopies the current time prices to time+1; this is to save space in memory, because mainGrid is only a Mx2 matrix
                    if (i != 0)
                        mainGrid[n, 1] = mainGrid[n, 0];
                }
            }

            //Once everything is finished, loads the option prices from time=0 into a vector and passes it to the cubic spline
            for (int j = 0; j <= M; j++)
                optPrices[j] = mainGrid[j, 0];
            NR.Spline(ref spotPrice, ref optPrices, 1e30, 1e30, out y2);

        }

        //Call this method to actually get an option price for a certain stock level
        public double GetPrice(double Sx)
        {
            return NR.Splint(ref spotPrice, ref optPrices, ref y2, Sx);
        }

    }
}