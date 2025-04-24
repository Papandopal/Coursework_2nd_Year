using System.Collections;

namespace Agar.io_Alpfa
{
    public class ConcurrentList<T> : IEnumerable<T>
    {
        private readonly List<T> _list = new List<T>();
        private readonly object _lock = new object();

        public T this[int index]
        {
            get
            {
                lock (_lock) 
                {
                    return _list.ElementAt(index);
                }

            }
            set
            {
                lock (_lock)
                {
                    _list[index] = value;
                }
            }
        }

        // Добавление элемента в список
        public void Add(T item)
        {
            lock (_lock)
            {
                _list.Add(item);
            }
        }

        // Удаление элемента из списка
        public bool Remove(T item)
        {
            lock (_lock)
            {
                return _list.Remove(item);
            }
        }
        public void RemoveAt(int index)
        {
            lock (_lock)
            {
                _list.RemoveAt(index);
            }
        }
        public int IndexOf(T item)
        {
            lock (_lock)
            {
                return _list.IndexOf(item);
            }
        }
        // Получение количества элементов
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _list.Count;
                }
            }
        }

        // Очистка списка
        public void Clear()
        {
            lock (_lock)
            {
                _list.Clear();
            }
        }

        // Проверка, содержит ли список элемент
        public bool Contains(T item)
        {
            lock (_lock)
            {
                return _list.Contains(item);
            }
        }

        // Копирование элементов в массив
        public T[] ToArray()
        {
            lock (_lock)
            {
                return _list.ToArray();
            }
        }

        // Реализация IEnumerable<T> для перебора
        public IEnumerator<T> GetEnumerator()
        {
            List<T> snapshot;
            lock (_lock)
            {
                snapshot = new List<T>(_list); // Создаем копию для безопасного перебора
            }
            return snapshot.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
