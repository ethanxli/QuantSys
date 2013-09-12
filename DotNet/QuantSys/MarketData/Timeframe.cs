using System;

namespace QuantSys.MarketData
{
    public class Timeframe
    {
        public enum Period
        {
            T,
            m1,
            m5,
            m15, 
            m30, 
            H1,
            H2,
            H3,
            H4,
            H8,
            D1,
            W1,
            M1
        };

        public static int TimeframeToMinutes(string timespan)
        {
            switch (timespan)
            {
                case "m1":
                    return 1;
                case "m5":
                    return 5;
                case "m15":
                    return 15;
                case "m30":
                    return 30;
                case "H1":
                    return 60;
                case "H2":
                    return 120;
                case "H3":
                    return 180;
                case "H4":
                    return 240;
                case "H6":
                    return 360;
                case "H12":
                    return 720;
                case "D1":
                    return 60 * 24;
                case "W1":
                    return 60 * 24 * 7;
                case "M1":
                    return 60 * 24 * 30;
            }

            return 60;
        }


        public static TimeSpan StringToTimeSpan(string timespan)
        {
            TimeSpan t = new TimeSpan();

            switch (timespan)
            {
                case "m1":
                    {
                        t = new TimeSpan(0, 1, 0);
                        break;
                    }
                case "m5":
                    {
                        t = new TimeSpan(0, 5, 0);
                        break;
                    }
                case "m15":
                    {
                        t = new TimeSpan(0, 15, 0);
                        break;
                    }
                case "m30":
                    {
                        t = new TimeSpan(0, 30, 0);
                        break;
                    }
                case "H1":
                    {
                        t = new TimeSpan(1, 0, 0);
                        break;
                    }
                case "H2":
                    {
                        t = new TimeSpan(2, 0, 0);
                        break;
                    }
                case "H3":
                    {
                        t = new TimeSpan(3, 0, 0);
                        break;
                    }
                case "H4":
                    {
                        t = new TimeSpan(4, 0, 0);
                        break;
                    }
                case "H6":
                    {
                        t = new TimeSpan(6, 0, 0);
                        break;
                    }
                case "H12":
                    {
                        t = new TimeSpan(12, 0, 0);
                        break;
                    }
                case "D1":
                    {
                        t = new TimeSpan(1, 0, 0, 0);
                        break;
                    }
                case "W1":
                    {
                        t = new TimeSpan(5, 1, 0);
                        break;
                    }
                case "M1":
                    {
                        t = new TimeSpan(25, 1, 0);
                        break;
                    }
            }

            return t;
        }
        public Period TimePeriod { get; set; }

        public Timeframe()
        {
            
        }
    }
}
