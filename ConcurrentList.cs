using System.Collections;

namespace Agar.io_Alpfa
{
    public class ConcurrentList<T> : IEnumerable<T>
    {
        private readonly List<T> _list = new List<T>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public void Add(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                _list.Add(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool Remove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _list.Remove(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void RemoveAt(int index)
        {
            _lock.EnterWriteLock();
            try
            {
                _list.RemoveAt(index);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public int IndexOf(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return _list.IndexOf(item);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public T this[int index]
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _list[index];
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _list[index] = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        public int Count
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _list.Count;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _list.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            List<T> snapshot;
            _lock.EnterReadLock();
            try
            {
                snapshot = new List<T>(_list);
            }
            finally
            {
                _lock.ExitReadLock();
            }
            return snapshot.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
