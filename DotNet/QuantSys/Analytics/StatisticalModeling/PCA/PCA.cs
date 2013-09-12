using MathNet.Numerics.LinearAlgebra;

namespace QuantSys.Analytics.StatisticalModeling.PCA
{
    class PCA
    {
        private Matrix sourceMatrix;
        private Vector singularValues;

        private Vector eigenValues;
        private Matrix eigenVectors;
        private Matrix resultMatrix;

        /*
        /// <summary>Computes the Principal Component Analysis algorithm.</summary>
        public void Compute()
        {
            int rows = sourceMatrix.RowCount;
            int cols = sourceMatrix.ColumnCount;

            // Create a new matrix to work upon
            Matrix matrix = Matrix.Create(new double[rows,cols]);


            // Prepare the data, storing it in the new matrix.
            if (this.analysisMethod == AnalysisMethod.Correlation)
            {
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        // subtract mean and divide by standard deviation (convert to Z Scores)
                        matrix[i, j] = (sourceMatrix[i, j] - sourceMatrix.GetColumnVector(j).Mean())/
                                       sourceMatrix.GetColumnVector(j).StandardDeviation();
            }
            else
            {
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        // Just center the data around the mean. Will have no effect if the
                        //  data is already centered (the mean will be zero).
                        matrix[i, j] = (sourceMatrix[i, j] - sourceMatrix..GetColumnVector(j).Mean());
            }



            // Perform the Singular Value Decomposition (SVD) of the matrix
            SingularValueDecomposition singularDecomposition = new SingularValueDecomposition(matrix);
            singularValues = singularDecomposition.SingularValues;


            // Eigen values are the square of the singular values
            for (int i = 0; i < singularValues.Length; i++)
            {
                eigenValues[i] = singularValues[i] * singularValues[i];
            }


            //  The principal components of 'Source' are the eigenvectors of Cov(Source). Thus if we
            //  calculate the SVD of 'matrix' (which is Source standardized), the columns of matrix V
            //  (right side of SVD) will be the principal components of Source.                        


            // The right singular vectors contains the principal components of the data matrix
            this.eigenVectors = singularDecomposition.RightSingularVectors;

            // The left singular vectors contains the scores of the principal components
            this.resultMatrix = singularDecomposition.LeftSingularVectors;


            // Calculate proportions
            double sum = 0;
            for (int i = 0; i < eigenValues.Length; i++)
                sum += eigenValues[i];
            sum = (1.0 / sum);

            for (int i = 0; i < eigenValues.Length; i++)
                componentProportions[i] = eigenValues[i] * sum;


            // Calculate cumulative proportions
            this.componentCumulative[0] = this.componentProportions[0];
            for (int i = 1; i < this.componentCumulative.Length; i++)
            {
                this.componentCumulative[i] = this.componentCumulative[i - 1] + this.componentProportions[i];
            }


            // Creates the object-oriented structure to hold the principal components
            PrincipalComponent[] components = new PrincipalComponent[singularValues.Length];
            for (int i = 0; i < components.Length; i++)
            {
                components[i] = new PrincipalComponent(this, i);
            }
            this.componentCollection = new PrincipalComponentCollection(components);
        }
         * 
         * * 
         */
    }
         
}
