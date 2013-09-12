using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace QuantSys.Analytics
{
    using MathNet.Numerics.LinearAlgebra;

    public class PolynomialRegression
    {
        Vector x_data, y_data, coef;
        int order;

        public PolynomialRegression(Vector x_data, Vector y_data, int order)
        {
            if (x_data.Length != y_data.Length)
            {
                throw new IndexOutOfRangeException();
            }
            this.x_data = x_data;
            this.y_data = y_data;
            this.order = order;
            int N = x_data.Length;
            Matrix A = new Matrix(N, order + 1);
            for (int i = 0; i < N; i++)
            {
                A.SetRowVector(VandermondeRow(x_data[i]), i);
            }

            // Least Squares of |y=A(x)*c| 
            //  tr(A)*y = tr(A)*A*c
            //  inv(tr(A)*A)*tr(A)*y = c
            Matrix At = Matrix.Transpose(A);
            Matrix y2 = new Matrix(y_data, N);
            coef = (At * A).Solve(At * y2).GetColumnVector(0);
        }

        Vector VandermondeRow(double x)
        {
            double[] row = new double[order + 1];
            for (int i = 0; i <= order; i++)
            {
                row[i] = Math.Pow(x, i);
            }
            return new Vector(row);
        }

        public double Fit(double x)
        {
            return Vector.ScalarProduct(VandermondeRow(x), coef);
        }

        public int Order { get { return order; } }
        public Vector Coefficients { get { return coef; } }
        public Vector XData { get { return x_data; } }
        public Vector YData { get { return y_data; } }
    }
}
