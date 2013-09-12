using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;

namespace QuantSys.Analytics
{
    public static class StatisticsExtension
    {
        #region normalization

        public static double[] Normalize0to1(this double[] data)
        {
            var returnData = new double[data.Length];
            var d = new DenseVector(data);
            var result = new DenseVector(d.Count);
            d.CopyTo(result);
            result = (DenseVector) (result - d.Min())/(d.Max() - d.Min());
            return result.ToArray();
        }

        public static DenseVector Normalize0to1(this DenseVector data)
        {
            var d = new DenseVector(data);
            var result = new DenseVector(d.Count);
            d.CopyTo(result);
            return (DenseVector) (result - d.Min())/(d.Max() - d.Min());
        }

        public static double[] NormalizeNeg1to1(this double[] data)
        {
            var d = new DenseVector(data);
            var result = new DenseVector(d.Count);
            d.CopyTo(result);
            result = (DenseVector) (result - ((d.Max() + d.Min())/2))/((d.Max() - d.Min())/2);
            return result.ToArray();
        }

        public static double[] NormalizeZScore(this double[] data)
        {
            var d = new DenseVector(data);
            var result = new DenseVector(d.Count);
            d.CopyTo(result);
            result = (DenseVector) ((result - d.Mean())/(d.StandardDeviation()));
            return result.ToArray();
        }

        public static double[] NormalizeZScore(this double[] data, double[] longData)
        {
            var d = new DenseVector(data);
            var result = new DenseVector(d.Count);
            d.CopyTo(result);

            result = (DenseVector) ((result - longData.Mean())/(longData.StandardDeviation()));
            return result.ToArray();
        }

        #endregion

        ///////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////
        public static double Min(this double[] data)
        {
            return (new DenseVector(data)).Minimum();
        }

        public static double Max(this double[] data)
        {
            return (new DenseVector(data)).Maximum();
        }

        public static double Average(this double[] data)
        {
            return (new DenseVector(data)).Average();
        }

        public static double Kurtosis(this double[] data)
        {
            var d = new DenseVector(data);
            var num = new DenseVector(d.Count);
            var denom = new DenseVector(d.Count);
            d.CopyTo(num);
            d.CopyTo(denom);

            for (int i = 0; i < num.Count; i++)
            {
                num[i] = Math.Pow(d[i] - d.Mean(), 4);
                denom[i] = Math.Pow(d[i] - d.Mean(), 2);
            }

            return (num.Sum()*num.Count)/(Math.Pow(denom.Sum(), 2));
        }

        public static double Covariance(DenseVector x, DenseVector y)
        {
            var cov = new DenseVector(x.Count);
            double xMean = x.Mean();
            double yMean = y.Mean();

            if (xMean.Equals(double.NaN) || yMean.Equals(double.NaN))
                return double.NaN;

            for (int i = 0; i < x.Count; i++)
            {
                cov[i] = (x[i] - xMean)*(y[i] - yMean);
            }

            return ((double) 1/(x.Count - 1))*cov.Sum();
        }

        public static double Correlation(double[] a, double[] b)
        {
            return MathNet.Numerics.Statistics.Correlation.Pearson(a, b);
        }


        public static DenseMatrix CorrelationToCovariance(DenseMatrix corr, DenseVector stdev)
        {
            var cov = new DenseMatrix(corr.RowCount, corr.ColumnCount);

            for (int i = 0; i < corr.RowCount; i++)
            {
                for (int j = 0; j < corr.ColumnCount; j++)
                {
                    cov[i, j] = corr[i, j]*(stdev[i]*stdev[j]);
                }
            }

            return cov;
        }


        public static double[] AggregateWindow(double[] data, Func<double[], double> f, int window, bool staggered,
            bool padded)
        {
            var retData = new List<double>();

            if (padded)
            {
                for (int i = 1; i < window; i++) retData.Add(double.NaN);
            }

            for (int i = window - 1; i < data.Length; i += (staggered) ? window : 1)
            {
                var tempdata = new double[window];
                for (int j = 0; j < window; j++)
                {
                    tempdata[j] = data[i - (window - j - 1)];
                }
                retData.Add(f(tempdata));
            }

            return retData.ToArray();
        }


        public static double[] AggregateWindow(double[] data1, double[] data2, Func<double[], double[], double> f,
            int window, bool staggered, bool padded)
        {
            var retData = new List<double>();

            if (padded)
            {
                for (int i = 1; i < window; i++) retData.Add(double.NaN);
            }

            for (int i = window - 1; i < data1.Length; i += (staggered) ? window : 1)
            {
                var tempdata1 = new double[window];
                var tempdata2 = new double[window];

                for (int j = 0; j < window; j++)
                {
                    tempdata1[j] = data1[i - (window - j - 1)];
                    tempdata2[j] = data2[i - (window - j - 1)];
                }
                retData.Add(f(tempdata1, tempdata2));
            }

            return retData.ToArray();
        }


        public static void ApplyFunction(DenseVector d, Func<double, double> func)
        {
            for (int i = 0; i < d.Count; i++)
            {
                d[i] = func(d[i]);
            }
        }

        #region Performance

        public static double Variance(this double[] data)
        {
            return (new DenseVector(data)).PopulationVariance();
        }

        public static double StandardDeviation(this double[] data)
        {
            return (new DenseVector(data)).PopulationStandardDeviation();
        }

        public static double SharpeRatio(this double[] data, double rf = 0)
        {
            return (data.AverageRawReturn())/data.StandardDeviation();
        }

       
        public static double MaximumDrawdown(this double[] data, bool asPercentage)
        {
            double maxDrawdown = 0;
            double maxPercentDrawdown = 0;

            for (int i = 0; i < data.Length; i++)
            {
                for (int j = i + 1; j < data.Length; j++)
                {
                    if (data[i] - data[j] > maxDrawdown)
                    {
                        maxDrawdown = data[i] - data[j];
                        maxPercentDrawdown = (maxDrawdown/data[i]);
                    }
                }
            }

            return (asPercentage) ? maxPercentDrawdown : maxDrawdown;
        }


        public static double SortinoRatio(this double[] data, double rf = 0)
        {
            return 0;
        }

        public static double[] RawRateOfReturn(this double[] data)
        {
            var result = new double[data.Length - 1];
            for (int i = 1; i < data.Length; i++)
            {
                result[i - 1] = (data[i] - data[i - 1])/data[i];
            }

            return result;
        }

        public static double[] LogRateOfReturn(this double[] data)
        {
            var result = new double[data.Length - 1];
            for (int i = 1; i < data.Length; i++)
            {
                double ret = (data[i] - data[i - 1])/data[i];
                result[i - 1] = Math.Log(1 + ret);
            }

            return result;
        }

        public static double AverageRawReturn(this DenseVector d)
        {
            double[] result = RawRateOfReturn(d.ToArray());
            var e = new DenseVector(result);
            return e.Mean();
        }

        public static double AverageRawReturn(this double[] d)
        {
            return (new DenseVector(d)).AverageRawReturn();
        }

        public static double GeometricReturn(this DenseVector d)
        {
            double[] result = RawRateOfReturn(d.ToArray());
            double gReturn = 1.0;
            for (int i = 0; i < result.Length; i++)
            {
                gReturn *= (1 + result[i]);
            }

            return Math.Pow(gReturn, 1/result.Length);
        }

        public static double GeometricReturn(this double[] d)
        {
            return (new DenseVector(d)).GeometricReturn();
        }

        #endregion

        ////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////
    }
}