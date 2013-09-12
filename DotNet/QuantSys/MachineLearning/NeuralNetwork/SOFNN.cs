using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace QuantSys.MachineLearning.NeuralNetwork
{
    internal class SOFNN
    {
        public DenseMatrix out_C;
        public DenseMatrix out_O;
        public DenseMatrix out_Sigma;

        public SOFNN(double sz, double d, double t, double ks)
        {
            SigmaZero = sz;
            delta = d;
            threshold = t;
            k_sigma = ks;
        }

        private double SigmaZero { get; set; }
        private double delta { get; set; }
        private double threshold { get; set; }
        private double k_sigma { get; set; }

        public void Train(DenseMatrix X, DenseVector d, DenseVector Kd)
        {
            int R = X.RowCount;
            int N = X.ColumnCount;
            int U = 0; //the number of neurons in the structure


            var c = new DenseMatrix(R, 1);
            var sigma = new DenseMatrix(R, 1);

            var Q = new DenseMatrix((R + 1), (R + 1));
            var O = new DenseMatrix(1, (R + 1));
            var pT_n = new DenseMatrix((R + 1), 1);

            double maxPhi = 0;
            int maxIndex;

            var Psi = new DenseMatrix(N, 1);

            Console.WriteLine("Running...");
            //for each observation n in X
            for (int i = 0; i < N; i++)
            {
                Console.WriteLine(100*(i/(double) N) + "%");

                var x = new DenseVector(R);
                X.Column(i, x);

                //if there are neurons in structure,
                //update structure recursively.
                if (U == 0)
                {
                    c = (DenseMatrix) x.ToColumnMatrix();
                    sigma = new DenseMatrix(R, 1, SigmaZero);
                    U = 1;
                    Psi = CalculatePsi(X, c, sigma);
                    UpdateStructure(X, Psi, d, ref Q, ref O);
                    pT_n =
                        (DenseMatrix)
                            (CalculateGreatPsi((DenseMatrix) x.ToColumnMatrix(), (DenseMatrix) Psi.Row(i).ToRowMatrix()))
                                .Transpose();
                }
                else
                {
                    StructureRecurse(X, Psi, d, i, ref Q, ref O, ref pT_n);
                }


                bool KeepSpinning = true;
                while (KeepSpinning)
                {
                    //Calculate the error and if-part criteria
                    double ee = pT_n.Multiply(O)[0, 0];

                    double approximationError = Math.Abs(d[i] - ee);

                    DenseVector Phi;
                    double SumPhi;
                    CalculatePhi(x, c, sigma, out Phi, out SumPhi);

                    maxPhi = Phi.Maximum();
                    maxIndex = Phi.MaximumIndex();

                    if (approximationError > delta)
                    {
                        if (maxPhi < threshold)
                        {
                            var tempSigma = new DenseVector(R);
                            sigma.Column(maxIndex, tempSigma);

                            double minSigma = tempSigma.Minimum();
                            int minIndex = tempSigma.MinimumIndex();
                            sigma[minIndex, maxIndex] = k_sigma*minSigma;
                            Psi = CalculatePsi(X, c, sigma);
                            UpdateStructure(X, Psi, d, ref Q, ref O);
                            var psi = new DenseVector(Psi.ColumnCount);
                            Psi.Row(i, psi);

                            pT_n =
                                (DenseMatrix)
                                    CalculateGreatPsi((DenseMatrix) x.ToColumnMatrix(), (DenseMatrix) psi.ToRowMatrix())
                                        .Transpose();
                        }
                        else
                        {
                            //add a new neuron and update strucutre

                            double distance = 0;
                            var cTemp = new DenseVector(R);
                            var sigmaTemp = new DenseVector(R);

                            //foreach input variable
                            for (int j = 0; j < R; j++)
                            {
                                distance = Math.Abs(x[j] - c[j, 0]);
                                int distanceIndex = 0;

                                //foreach neuron past 1
                                for (int k = 1; k < U; k++)
                                {
                                    if ((Math.Abs(x[j] - c[j, k])) < distance)
                                    {
                                        distanceIndex = k;
                                        distance = Math.Abs(x[j] - c[j, k]);
                                    }
                                }

                                if (distance < Kd[j])
                                {
                                    cTemp[j] = c[j, distanceIndex];
                                    sigmaTemp[j] = sigma[j, distanceIndex];
                                }
                                else
                                {
                                    cTemp[j] = x[j];
                                    sigmaTemp[j] = distance;
                                }
                            }
                            //end foreach

                            c = (DenseMatrix) c.InsertColumn(c.ColumnCount - 1, cTemp);
                            sigma = (DenseMatrix) sigma.InsertColumn(sigma.ColumnCount - 1, sigmaTemp);
                            Psi = CalculatePsi(X, c, sigma);
                            UpdateStructure(X, Psi, d, ref Q, ref O);
                            U++;
                            KeepSpinning = false;
                        }
                    }
                    else
                    {
                        if (maxPhi < threshold)
                        {
                            var tempSigma = new DenseVector(R);
                            sigma.Column(maxIndex, tempSigma);

                            double minSigma = tempSigma.Minimum();
                            int minIndex = tempSigma.MinimumIndex();
                            sigma[minIndex, maxIndex] = k_sigma*minSigma;
                            Psi = CalculatePsi(X, c, sigma);
                            UpdateStructure(X, Psi, d, ref Q, ref O);
                            var psi = new DenseVector(Psi.ColumnCount);
                            Psi.Row(i, psi);

                            pT_n =
                                (DenseMatrix)
                                    CalculateGreatPsi((DenseMatrix) x.ToColumnMatrix(), (DenseMatrix) psi.ToRowMatrix())
                                        .Transpose();
                        }
                        else
                        {
                            KeepSpinning = false;
                        }
                    }
                }
            }

            out_C = c;
            out_O = O;
            out_Sigma = sigma;

            Console.WriteLine("Done.");
        }


        public void StructureRecurse(DenseMatrix X, DenseMatrix Psi, DenseVector d, int n, ref DenseMatrix Q,
            ref DenseMatrix O, ref DenseMatrix pT_n)
        {
            //O = O(t-1) O_enxt = O(t)
            //o should be a column vector ( in matrix form)
            var x = new DenseVector(X.RowCount);
            var psi = new DenseVector(Psi.ColumnCount);

            X.Column(n, x);
            Psi.Row(n, psi);

            DenseMatrix p_n = CalculateGreatPsi((DenseMatrix) x.ToColumnMatrix(), (DenseMatrix) psi.ToRowMatrix());

            pT_n = (DenseMatrix) p_n.Transpose();

            double ee = Math.Abs(d[n] - (pT_n.Multiply(O))[0, 0]);
            double temp = 1 + (pT_n.Multiply(Q)).Multiply(p_n)[0, 0];
            double ae = Math.Abs(ee/temp);

            if (ee >= ae)
            {
                var L = (DenseMatrix) Q.Multiply(p_n).Multiply(1/temp);
                Q = (DenseMatrix) ((DenseMatrix.Identity(Q.RowCount).Subtract(L.Multiply(pT_n))).Multiply(Q));
                O = (DenseMatrix) O.Add(L*ee);
            }
            else
            {
                Q = (DenseMatrix) DenseMatrix.Identity(Q.RowCount).Multiply(Q);
            }
        }


        public void UpdateStructure(DenseMatrix X, DenseMatrix Psi, DenseVector d, ref DenseMatrix Q, ref DenseMatrix O)
        {
            /*
            %Others Ways of getting Q=[P^T(t)*P(t)]^-1
            %**************************************************************************
            %opts.SYM = true;
            %Q = linsolve(GreatPsiBig*GreatPsiBig',eye(M),opts);
            %
            %Q = inv(GreatPsiBig*GreatPsiBig');
            %Q = pinv(GreatPsiBig*GreatPsiBig');
            %**************************************************************************
             * */
            //
            DenseMatrix GreatPsi = CalculateGreatPsi(X, Psi);
            var d_temp = (DenseMatrix) d.ToColumnMatrix();

            //M = U * (r + 1)
            //N = # of observations
            int M = GreatPsi.RowCount;
            DenseMatrix i = DenseMatrix.Identity(M);
            var tde = (DenseMatrix) GreatPsi.TransposeAndMultiply(GreatPsi);


            Q = (DenseMatrix) tde.LU().Solve(i);
            //Q = (DenseMatrix) ((GreatPsi.TransposeAndMultiply(GreatPsi)).Inverse());

            O = (DenseMatrix) (Q*GreatPsi).Multiply(d_temp);
        }


        /*
         * Layer 2: EBF Layer.
         * ------------------------
         * u_ij = exp{ - (x_i - c_ij)^2 / 2(sigma_ij^2) }
         * where:
         * u_ij is the ith membership function in the jth neuron
         * c_ij is the center of the ith membership in the jth neuron
         * sigma_ij is the width of the ith membership in the jth neuron
         * R is the number of input varaibles
         * u is the number of neurons
         * 
        */

        public void CalculatePhi(DenseVector x, DenseMatrix c, DenseMatrix sigma, out DenseVector Phi, out double SumPhi)
        {
            int R = c.RowCount; //r - the number of input variables
            int U = c.ColumnCount; //u - the number of neurons in the structure

            Phi = new DenseVector(U);
            SumPhi = 0;

            //iterate through neurons
            for (int j = 0; j < U; j++)
            {
                double S = 0;

                //iterate through input variables
                for (int i = 0; i < R; i++)
                {
                    S += Math.Pow((x[i] - c[i, j]), (2.0))/(2*(Math.Pow(sigma[i, j], 2)));
                }

                Phi[j] = Math.Exp(-S);
                SumPhi += Phi[j];
            }
        }


        /*
         * Layer 3: Normalized Layer.
         * The number of neurons in this layer is equal to that of layer 2.
         * Psi_j = phi_j / sum(phi from k = 1 to u), for j = 1 to u
         * 
        */

        public DenseMatrix CalculatePsi(DenseMatrix X, DenseMatrix c, DenseMatrix sigma)
        {
            int U = c.ColumnCount; //the number of neurons in the structure
            int R = X.RowCount; //the number of inputs per observation
            int N = X.ColumnCount; //the number of observations

            var Psi = new DenseMatrix(N, U);

            for (int i = 0; i < N; i++)
            {
                DenseVector Phi;
                double SumPhi;

                var x = new DenseVector(R);
                X.Column(i, x);

                CalculatePhi(x, c, sigma, out Phi, out SumPhi);

                //for each neuron
                for (int j = 0; j < U; j++)
                {
                    //Psi - In a row you go through the neurons and in a column you go through number of
                    //observations **** Psi(#obs,IndexNeuron) ****
                    Psi[i, j] = (Phi[j]/SumPhi);
                }
            }

            return Psi;
        }

        /*
         * Layer 4: weighted Layer.
         * 
        */

        public DenseMatrix CalculateGreatPsi(DenseMatrix X, DenseMatrix Psi)
        {
            int N = Psi.RowCount;
            int U = Psi.ColumnCount;
            int R = X.RowCount; //the number of inputs per observation

            var GreatPsi = new DenseMatrix(U*(R + 1), N);

            //foreach observation
            for (int i = 0; i < N; i++)
            {
                var x = new DenseVector(R);
                X.Column(i, x);

                var GreatPsiCol = new DenseVector(U*(R + 1));

                //foreach neuron
                for (int j = 0; j < U; j++)
                {
                    var temp = new DenseVector(x.Count + 1, 1);
                    temp.SetSubVector(1, x.Count, x);
                    GreatPsiCol.SetSubVector(j*(temp.Count), temp.Count, Psi[i, j]*temp);
                }

                GreatPsi.SetColumn(i, GreatPsiCol);
            }

            return GreatPsi;
        }
    }
}