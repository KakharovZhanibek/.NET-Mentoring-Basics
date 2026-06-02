using System;
using Tasks.DoNotChange;

namespace Tasks
{
    public class HybridFlowProcessor<T> : IHybridFlowProcessor<T>
    {
        private readonly DoublyLinkedList<T> _storage = new DoublyLinkedList<T>();

        // Push/Pop = FILO (stack) - add and remove from the end
        public void Push(T item)
        {
            _storage.Add(item);
        }

        public T Pop()
        {
            if (_storage.Length == 0)
                throw new InvalidOperationException("Processor is empty.");
            return _storage.RemoveAt(_storage.Length - 1);
        }

        // Enqueue/Dequeue = FIFO (queue) - add to end, remove from beginning
        public void Enqueue(T item)
        {
            _storage.Add(item);
        }

        public T Dequeue()
        {
            if (_storage.Length == 0)
                throw new InvalidOperationException("Processor is empty.");
            return _storage.RemoveAt(0);
        }
    }
}
