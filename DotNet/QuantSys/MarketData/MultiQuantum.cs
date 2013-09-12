using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QuantSys.MarketData
{
    public class MultiQuantum : IEnumerable<List<Tick>>
    {
        public int Length { get { return _totalData.Keys.Count; } }
        private SortedList<DateTime, List<Tick>> _totalData;

        public List<Tick> this[int index] { get { return _totalData.Values[index]; } } 
        public IList<DateTime> Keys{get { return _totalData.Keys; }}
        public IList<List<Tick>> Values { get { return _totalData.Values; } } 
        protected MultiQuantum(SortedList<DateTime, List<Tick>> data)
        {
            _totalData = data;
        }

        /// <summary>
        /// Populates a MultiQuantum based on DateTimes that are available to all Quantums
        /// </summary>
        /// <param name="datalist"></param>
        /// <returns></returns>
        public static MultiQuantum OrganizeMultiQuantum(List<Quantum> datalist)
        {
            var totalData = new SortedList<DateTime, List<Tick>>();

            //Single Entry
            if (datalist.Count == 1)
            {
                totalData = new SortedList<DateTime, List<Tick>>();
                for (int i = 0; i < datalist[0].Data.Count; i++)
                {
                    totalData.Add(datalist[0].Data.Keys[i], new List<Tick>(){datalist[0].Data.Values[i]});
                }                
                return new MultiQuantum(totalData);
            }

            //Multiple Entries
            int num = datalist.Count;
           

            for (int i = 0; i < datalist[0].Data.Count; i++)
            {
                DateTime d = datalist[0].Data.Keys[i];

                bool allContains = true;
                for (int j = 1; j < num; j++)
                {
                    if (!datalist[j].Data.ContainsKey(d))
                    {
                        allContains = false;
                        break;
                    }
                }

                if (allContains)
                {
                    List<Tick> ticks = new List<Tick>();
                    foreach (Quantum q in datalist) ticks.Add(q.Data[d]);
                    totalData.Add(d, ticks);
                }
            }

            return new MultiQuantum(totalData);

        }

        public List<List<Tick>> RevertToList()
        {
            int count = this.First().Count;
            List<List<Tick>> list = new List<List<Tick>>();

            for (int i = 0; i < count; i++)
                list.Add(new List<Tick>());

            foreach(List<Tick> ticks in this)
            {
                for (int i = 0; i < count; i++)
                    list[i].Add(ticks[i]);
            }

            return list;

        } 
        public IEnumerator<List<Tick>> GetEnumerator()
        {
            return _totalData.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
