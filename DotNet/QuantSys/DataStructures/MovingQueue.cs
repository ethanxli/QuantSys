using System.Collections;
using System.Collections.Generic;

namespace QuantSys.DataStructures
{
    public class MovingQueue<T> : IEnumerable<T>
    {
        public int Count {get { return _queueData.Count; }}
        public int Capacity {get { return _capacity; }}
        public Queue<T> Data { get { return _queueData; } }


        private Queue<T> _queueData;
        private readonly int _capacity;

        public MovingQueue(int capacity)
        {
            _queueData = new Queue<T>(capacity);
            _capacity = capacity;
        }

        public void Enqueue(T item)
        {

            if (_queueData.Count >= _capacity)
            {
                _queueData.Dequeue();
            }

            _queueData.Enqueue(item);
        }

        public T[] ToArray()
        {
            return _queueData.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queueData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
