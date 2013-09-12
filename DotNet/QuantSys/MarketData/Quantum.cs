using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using QuantSys.TradeEngine;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface.Functions;
using QuantSys.Util;

namespace QuantSys.MarketData
{

    public class Quantum : IEnumerable<Tick>
    {
        private readonly SortedList<DateTime, Tick> _data;
        public Quantum(SortedList<DateTime, Tick> data)
        {
            _data = data;
        }

        public Quantum()
        {
            _data = new SortedList<DateTime, Tick>();
        }

        public Tick[] ToArray()
        {
            return _data.Values.ToArray();
        }
        public SortedList<DateTime, Tick> Data
        {
            get { return _data; }
        }

        public Symbol Symbol { get; set; }
        public Timeframe Period { get; set; }

        public Tick this[int index]
        {
            get { return _data.Values[index]; }
        }

        public void CombineWith(Quantum other)
        {
            foreach (KeyValuePair<DateTime, Tick> kvp in other.Data)
            {
                if(!_data.ContainsKey(kvp.Key))
                    _data.Add(kvp.Key, kvp.Value);
            }
        }

        public IEnumerator<Tick> GetEnumerator()
        {
            return Data.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        /// <summary>
        /// Parses an excel file into a Quantum object.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="symbol"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static Quantum ExcelToQuantum(string filename, string symbol, int startIndex = 0)
        {
            object[,] denseMatrix;
            ExcelUtil.Open(@filename, out denseMatrix);
            var s = new Symbol(symbol);

            var mData = new SortedList<DateTime, Tick>();

            for (int i = denseMatrix.GetLength(0) - startIndex; i > 1; i--)
            {
                try
                {
                    DateTime dateTime = (DateTime)denseMatrix[i, 1];

                    var t = new Tick(
                        0,
                        (double)denseMatrix[i, 6],
                        (double)denseMatrix[i, 7],
                        (double)denseMatrix[i, 8],
                        (double)denseMatrix[i, 9],
                        0,
                        (double)denseMatrix[i, 2],
                        (double)denseMatrix[i, 3],
                        (double)denseMatrix[i, 4],
                        (double)denseMatrix[i, 5],
                        (double)denseMatrix[i, 10],
                        dateTime
                        );
                    t.Symbol = s;
                    mData.Add(dateTime, t);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return (new Quantum(mData) { Symbol = s });
        }

        public static Quantum QuantumFromLiveData(string symbol, string timeframe, int ticks)
        {
            FXSession session = new FXSession();
            session.InitializeSession();
            HistoricPriceEngine h = new HistoricPriceEngine(session);
            h.GetLongHistoricPrices(symbol, timeframe, ticks);
            while(!h.Complete) Thread.Sleep(100);

            return (h.Data);
        }


    }
}