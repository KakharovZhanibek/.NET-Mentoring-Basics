using System;
using System.Collections;
using System.Collections.Generic;
using Tasks.DoNotChange;

namespace Tasks
{
    public class DoublyLinkedList<T> : IDoublyLinkedList<T>
    {
        private class Node
        {
            public T Value;
            public Node Prev;
            public Node Next;
            public Node(T value) { Value = value; }
        }

        private Node _head;
        private Node _tail;
        private int _length;

        public int Length => _length;

        public void Add(T e)
        {
            var node = new Node(e);
            if (_head == null)
            {
                _head = _tail = node;
            }
            else
            {
                node.Prev = _tail;
                _tail.Next = node;
                _tail = node;
            }
            _length++;
        }

        public void AddAt(int index, T e)
        {
            if (index < 0 || index > _length)
                throw new IndexOutOfRangeException();

            var node = new Node(e);
            if (_head == null)
            {
                _head = _tail = node;
            }
            else if (index == 0)
            {
                node.Next = _head;
                _head.Prev = node;
                _head = node;
            }
            else if (index == _length)
            {
                node.Prev = _tail;
                _tail.Next = node;
                _tail = node;
            }
            else
            {
                var current = _head;
                for (int i = 0; i < index; i++) current = current.Next;
                var prev = current.Prev;
                prev.Next = node;
                node.Prev = prev;
                node.Next = current;
                current.Prev = node;
            }
            _length++;
        }

        public T ElementAt(int index)
        {
            if (index < 0 || index >= _length)
                throw new IndexOutOfRangeException();

            var current = _head;
            for (int i = 0; i < index; i++) current = current.Next;
            return current.Value;
        }

        public void Remove(T item)
        {
            var current = _head;
            while (current != null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Value, item))
                {
                    RemoveNode(current);
                    return;
                }
                current = current.Next;
            }
        }

        public T RemoveAt(int index)
        {
            if (index < 0 || index >= _length)
                throw new IndexOutOfRangeException();

            var current = _head;
            for (int i = 0; i < index; i++) current = current.Next;
            RemoveNode(current);
            return current.Value;
        }

        private void RemoveNode(Node node)
        {
            if (node.Prev != null) node.Prev.Next = node.Next;
            else _head = node.Next;

            if (node.Next != null) node.Next.Prev = node.Prev;
            else _tail = node.Prev;

            _length--;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new DoublyLinkedListEnumerator(_head);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class DoublyLinkedListEnumerator : IEnumerator<T>
        {
            private readonly Node _start;
            private Node _current;
            private bool _started;

            public DoublyLinkedListEnumerator(Node head)
            {
                _start = head;
            }

            public T Current => _current.Value;
            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (!_started)
                {
                    _started = true;
                    _current = _start;
                }
                else
                {
                    _current = _current?.Next;
                }
                return _current != null;
            }

            public void Reset()
            {
                _started = false;
                _current = null;
            }

            public void Dispose() { }
        }
    }
}
