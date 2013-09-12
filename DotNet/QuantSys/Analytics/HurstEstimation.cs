using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;

namespace QuantSys.Analytics
{
    /// <summary>
    /// The Hurst exponent is referred to as the "index of dependence," or 
    /// "index of long-range dependence." It quantifies the relative tendency 
    /// of a time series either to regress strongly to the mean or to cluster 
    /// in a direction. A value H in the range 0.5 < H < 1 indicates a time series 
    /// with long-term positive autocorrelation, meaning both that a high value 
    /// in the series will probably be followed by another high value and that 
    /// the values a long time into the future will also tend to be high. A value 
    /// in the range 0 < H < 0.5 indicates a time series with long-term switching 
    /// between high and low values in adjacent pairs, meaning that a single high 
    /// value will probably be followed by a low value and that the value after 
    /// that will tend to be high, with this tendency to switch between high and low 
    /// values lasting a long time into the future. A value of H=0.5 can indicate a 
    /// completely uncorrelated series, but in fact it is the value applicable to 
    /// series for which the autocorrelations at small time lags can be positive 
    /// or negative but where the absolute values of the autocorrelations decay 
    /// exponentially quickly to zero. 
    /// </summary>
    public class HurstEstimation
    {
        public static double CalculateHurstEstimate(double[] data, int minLength = 8)
        {
            List<double> n = new List<double>();
            List<double> avgRange = new List<double>();
            for (int i = 1; i <= data.Length; i *= 2)
            {
                double range = CalculateAverageRange(data, data.Length/i);
                if (!range.Equals(double.NaN))
                {
                    n.Add(i);
                    avgRange.Add(range);
                }
            }

            LinearRegression LR = new LinearRegression();
            LR.Model(n.Select(i=> Math.Log(i)).ToArray(),avgRange.Select(i=> Math.Log(i)).ToArray());
            return LR.X2;
        }

        private static double CalculateAverageRange(double[] data, int seriesLength)
        {
            if(data.Length % seriesLength != 0)
                throw new InvalidDataException("Data length must be divisible by series length.");

            double[] ranges = new double[data.Length/seriesLength];
            for (int i = 0; i < ranges.Length; i++)
                ranges[i] = CalculateRescaledRange(data.Skip(i*seriesLength).Take(seriesLength).ToArray());

            double averageRange = ranges.Mean();

            return averageRange;
        }


        private static double CalculateRescaledRange(double[] data)
        {
            double mean = data.Mean();
            double[] meanAdjustedSeries = data.Select(i => i - mean).ToArray();
            double[] cumulativeDeviateSeries = new double[data.Length];
            for(int i = 1; i < data.Length; i++)
                for (int j = 0; j < i; j++) cumulativeDeviateSeries[i] += meanAdjustedSeries[j];

            double range = cumulativeDeviateSeries.Max() - cumulativeDeviateSeries.Min();
            double stdev = data.PopulationStandardDeviation();

            double rescaledRange = range/stdev;
            return rescaledRange;
        }
    }
}
