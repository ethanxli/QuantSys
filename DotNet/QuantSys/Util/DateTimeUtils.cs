using System;

namespace QuantSys.Util
{
    public class DateTimeUtils
    {
        public static TimeSpan TimeFrameToTimeSpan(string timeframe)
        {
            switch (timeframe)
            {
                case "m1":
                    return new TimeSpan(0, 1, 0);
                case "m5":
                    return new TimeSpan(0, 5, 0);
                case "m15":
                    return new TimeSpan(0, 15, 0);
                case "m30":
                    return new TimeSpan(0, 30, 0);
                case "H1":
                    return new TimeSpan(1, 0, 0);
                default:
                    return new TimeSpan(1, 0, 0);
            }
        }

        public static double[] ReverseArray(double[] array)
        {
            var reverse = new double[array.Length];
            for (int i = array.Length - 1; i >= 0; i--) reverse[array.Length - 1 - i] = array[i];
            return reverse;
        }
    }
}