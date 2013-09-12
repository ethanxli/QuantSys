using System.Collections;
using System.Collections.Generic;

namespace QuantSys.DataStructures
{
    public class MovingQueue<T> : IEnumerable<T>
    {
        public Queue<T> QueueData;
        public int Count {get { return QueueData.Count; }}
        public int Capacity {get { return _capacity; }}
        public Queue<T> Data { get { return QueueData; } } 

        private readonly int _capacity;

        public MovingQueue(int capacity)
        {
            QueueData = new Queue<T>(capacity);
            _capacity = capacity;
        }

        public void Enqueue(T item)
        {

            if (QueueData.Count >= _capacity)
            {
                QueueData.Dequeue();
            }

            QueueData.Enqueue(item);
        }

        public T[] ToArray()
        {
            return QueueData.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return QueueData.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
