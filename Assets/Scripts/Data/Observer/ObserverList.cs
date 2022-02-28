using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace Utils.Observer
{
    public interface IObserverList : IList
    {
        
    }
    public class ObserverList<TValue>
        : ReadOnlyObserver<IList<TValue>>
            , IList<TValue>
            , IReadOnlyList<TValue>
            , IObserverList

        where TValue : ObserverBase
    {
        private List<TValue> data { get; }
        public UnityEvent<TValue> OnItemAdded { get; } = new UnityEvent<TValue>();
        public UnityEvent<TValue> OnItemRemoved { get; } = new UnityEvent<TValue>();
        public UnityEvent<TValue> OnItemChanged { get; } = new UnityEvent<TValue>();

        public int Count => data.Count;
        public new List<TValue> Value
        {
            get => data;
            set
            {
                Clear();
                foreach (var item in value)
                    Add(item);
            }
        }

        bool ICollection.IsSynchronized => ((ICollection) data).IsSynchronized;
        object ICollection.SyncRoot => ((ICollection) data).SyncRoot;
        bool IList.IsFixedSize => ((IList) data).IsFixedSize;

        public bool IsReadOnly => ((ICollection<TValue>) data).IsReadOnly;

        public TValue this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data[index];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                data[index] = value;
                OnChanged.Invoke(this);
            }
        }

        object IList.this[int index]
        {
            get => data[index];
            set
            {
                if (value is TValue tValue)
                    this[index] = tValue;
                else
                    Debug.LogErrorFormat("Setting {0} to {1}[{2}]", value, GetType(), index);
            }
        }

        private void Subscribe()
        {
            void SubscribeItem(ObserverBase observerBase)
            {
                observerBase.OnChange.AddListener(item =>
                {
                    OnChange.Invoke(this);
                    OnItemChanged.Invoke((TValue)observerBase);
                });
            }
            
            OnItemAdded.AddListener(SubscribeItem);
            OnItemRemoved.RemoveListener(SubscribeItem);
        }

        public ObserverList()
        {
            data = new List<TValue>();
            Subscribe();
        }

        public ObserverList(IEnumerable<TValue> source)
        {
            data = new List<TValue>(source);
            OnChange.AddListener((item) => this.OnChanged.Invoke(data));
            Subscribe();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TValue item)
        {
            data.Add(item);
            OnItemAdded.Invoke(item);
            OnChange.Invoke(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IEnumerable<TValue> items)
        {
            int oldCount = data.Count;
            data.AddRange(items);
            for (int i = oldCount; i < data.Count; ++i)
            {
                OnItemAdded.Invoke(data[i]);
            }

            OnChange.Invoke(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, TValue item)
        {
            data.Insert(index, item);
            OnItemAdded.Invoke(item);
            OnChange.Invoke(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IList.Insert(int index, object value)
        {
            if (value is TValue tValue)
                Insert(index, tValue);
            else
                Debug.LogErrorFormat("Inserting {0} to {1}[{2}]", value, GetType(), index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(TValue item)
        {
            var oldIndex = data.IndexOf(item);
            if (oldIndex < 0)
                return false;

            data.RemoveAt(oldIndex);
            OnItemRemoved.Invoke(item);
            OnChange.Invoke(this);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IList.Remove(object value)
        {
            if (value is TValue tValue)
                Remove(tValue);
            else
                Debug.LogErrorFormat("Removing {0} from {1}", value, GetType());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            var oldValue = data[index];
            data.RemoveAt(index);
            OnItemRemoved.Invoke(oldValue);

            OnChange.Invoke(this);
        }

        int IList.Add(object value)
        {
            if (value is TValue tValue)
            {
                Add(tValue);
                return data.Count - 1;
            }

            Debug.LogErrorFormat("Adding {0} to {1}", value, GetType());
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            var oldValues = new TValue[data.Count];
            data.CopyTo(oldValues);
            data.Clear();
            foreach (var oldValue in oldValues)
            {
                OnItemRemoved.Invoke(oldValue);
            }

            OnChange.Invoke(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(TValue item) => data.IndexOf(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int IList.IndexOf(object value) => value is TValue tValue ? IndexOf(tValue) : -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TValue item) => data.Contains(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IList.Contains(object value) => value is TValue tValue && Contains(tValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(TValue[] array, int arrayIndex) => data.CopyTo(array, arrayIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ICollection.CopyTo(System.Array array, int index)
            => ((IList) data).CopyTo(array, index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<TValue> GetEnumerator() => data.GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => data.GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort()
        {
            data.Sort();
            OnChange.Invoke(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort(Comparer<TValue> comparer)
        {
            data.Sort(comparer);
            OnChange.Invoke(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort(System.Comparison<TValue> comparison)
        {
            data.Sort(comparison);
            OnChange.Invoke(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort(int index, int count, Comparer<TValue> comparer)
        {
            data.Sort(index, count, comparer);
            OnChange.Invoke(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(TValue item)
            => data.BinarySearch(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(TValue item, Comparer<TValue> comparer)
            => data.BinarySearch(item, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(int index, int count, TValue item, Comparer<TValue> comparer)
            => data.BinarySearch(index, count, item, comparer);
    }
}