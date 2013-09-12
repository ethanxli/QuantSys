using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QuantSys.Indicators.Abstraction;
using QuantSys.Indicators.Oscillators;
using QuantSys.MarketData;
using QuantSys.TradeEngine;
using QuantSys.TradeEngine.Functions;

namespace QuantSys.Analytics
{
    public class IndicatorMatrix
    {
        private const int ticks = 1000;

        private List<Quantum> mData;
        private List<Type> indList;
        private string symbol;

        private const string S1 = "m5";
        private const string S2 = "m15";
        private const string M1 = "H1";
        private const string M2 = "H4";
        private const string L1 = "D1";
        private const string L2 = "W1";

        private string[] sList = new string[] {S1, S2, M1, M2, L1, L2};

        public IndicatorMatrix(string symbol, List<Type> iList = null)
        {
            mData = new List<Quantum>();
            this.symbol = symbol;
            indList = iList ?? new List<Type>()
                {
                    typeof(MACD), 
                    typeof(RSI), 
                    typeof(WilliamsR),
                    typeof(AC),
                    typeof(AO),
                    typeof(UltimateOscillator)
                }; 
        }

        public void LoadData()
        {
            FXSession session = new FXSession();
            session.InitializeSession();
            
            foreach (string s in sList)
            {
                HistoricPriceGrabber h = new HistoricPriceGrabber(session);
                h.GetLongHistoricPrices(symbol, s, ticks);
                while(!h.Complete) Thread.Sleep(100);   
                mData.Add(h.Data);
            }
        }

        public void Execute()
        {
            for(int i = 0; i < mData.Count; i++)
            {
                Quantum q = mData[i];

                List<object> iList = new List<object>();

                foreach (Type t in indList)
                {
                    iList.Add(Activator.CreateInstance(t));
                }

                foreach (KeyValuePair<DateTime,Tick> tick in q.Data)
                {
                    foreach (var indicator in iList)
                    {
                        ((AbstractIndicator)indicator).HandleNextTick(tick.Value);
                    }
                }

                Console.WriteLine(sList[i] + "-------------");
                foreach (var indicator in iList)
                {
                    Console.WriteLine(((AbstractIndicator)indicator).ToString() + ":" + ((AbstractIndicator)indicator)[0]);
                }

            }
        }


    }
}
