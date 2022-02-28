using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Events;
using Utils.Observer;

public interface IObserverDictionary : IDictionary
{
        
}

public class ObserverDictionary<TKey, TValue>
    : ReadOnlyObserver<IDictionary<TKey, TValue>>
        , IDictionary<TKey, TValue>
        , IReadOnlyDictionary<TKey, TValue>
        , IObserverDictionary

    where TValue : ObserverBase
    
{
    private Dictionary<TKey, TValue> data { get; }
    public UnityEvent<TKey, TValue> OnItemAdded { get; } = new UnityEvent<TKey, TValue>();
    public UnityEvent<TKey, TValue> OnItemChanged { get; } = new UnityEvent<TKey, TValue>();
    public UnityEvent<TKey, TValue> OnItemRemoved { get; } = new UnityEvent<TKey, TValue>();


    public new IDictionary<TKey, TValue> Value
    {
        get => data;
        set
        {
            Clear();
            foreach (var kv in value)
                Add(kv.Key, kv.Value);
        }
    }

    public int Count => data.Count;

    bool ICollection.IsSynchronized => ((IDictionary) data).IsSynchronized;
    object ICollection.SyncRoot => ((IDictionary) data).SyncRoot;
    bool IDictionary.IsFixedSize => ((IDictionary) data).IsFixedSize;
    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>) data).IsReadOnly;

    public ICollection<TKey> Keys => data.Keys;
    ICollection IDictionary.Keys => data.Keys;

    public Dictionary<TKey, TValue>.ValueCollection Values => data.Values;
    ICollection<TValue> IDictionary<TKey, TValue>.Values => data.Values;
    ICollection IDictionary.Values => data.Values;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => data.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => data.Values;

    private void Subscribe()
    {
        void SubscribeItem(TKey key, TValue value)
        {
            if (!(value is ObserverBase observerBase)) return;
            observerBase.OnChange.AddListener(item =>
            {
                OnChange.Invoke(this);
                OnItemChanged.Invoke(key, value);
            });
        }
            
        OnItemAdded.AddListener(SubscribeItem);
        OnItemRemoved.RemoveListener(SubscribeItem);
    }
    public TValue this[TKey key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => data.TryGetValue(key, out var value) ? value : default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            bool wasPresented;
            if ((wasPresented = data.TryGetValue(key, out var oldValue)) && oldValue.Equals(value))
                return;

            data[key] = value;
            if (wasPresented)
                OnItemChanged.Invoke(key, value);
            else
                OnItemAdded.Invoke(key, value);

            OnChange.Invoke(this);
        }
    }

    public TValue this[TKey key, System.Func<TValue> defaultValueCreator]
    {
        get
        {
            if (data.TryGetValue(key, out var value)) return value;
            
            value = defaultValueCreator.Invoke();

            data.Add(key, value);
            OnItemAdded.Invoke(key, value);
            OnChange.Invoke(this);

            return value;
        }
    }

    object IDictionary.this[object key]
    {
        get => key is TKey tKey ? data[tKey] : default;
        set
        {
            if (key is TKey tKey && value is TValue tValue)
                this[tKey] = tValue;
            else
                Debug.LogErrorFormat("Setting {0} to {1}[{2}]", value, GetType(), key);
        }
    }

    public ObserverDictionary()
    {
        data = new Dictionary<TKey, TValue>();
        Subscribe();
    }

    public ObserverDictionary(IDictionary<TKey, TValue> source)
    {
        data = new Dictionary<TKey, TValue>(source);
        Subscribe();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(TKey key, TValue value)
    {
        data.Add(key, value);
        OnItemAdded.Invoke(key, value);
        OnChange.Invoke(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        => Add(item.Key, item.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IDictionary.Add(object key, object value)
    {
        if (key is TKey tKey && value is TValue tValue)
            Add(tKey, tValue);
        else
            Debug.LogErrorFormat("Adding {0}+{1} to {2})", key, value, GetType());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(TKey key)
    {
        var isChanged = data.TryGetValue(key, out var oldValue) && data.Remove(key);
        if (isChanged)
        {
            OnItemRemoved.Invoke(key, oldValue);
            OnChange.Invoke(this);
        }

        return isChanged;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        var isChanged = ((ICollection<KeyValuePair<TKey, TValue>>) data).Remove(item);
        if (isChanged)
        {
            OnItemRemoved.Invoke(item.Key, item.Value);
            OnChange.Invoke(this);
        }

        return isChanged;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IDictionary.Remove(object key)
    {
        if (key is TKey tKey)
            Remove(tKey);
        else
            Debug.LogErrorFormat("Removing {0} from {1}", key, GetType());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        var oldPairs = new KeyValuePair<TKey, TValue>[data.Count];
        ((ICollection<KeyValuePair<TKey, TValue>>) data).CopyTo(oldPairs, 0);
        data.Clear();

        foreach (var oldPair in oldPairs)
        {
            OnItemRemoved.Invoke(oldPair.Key, oldPair.Value);
        }

        OnChange.Invoke(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) =>
        ((ICollection<KeyValuePair<TKey, TValue>>) data).Contains(item);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IDictionary.Contains(object key)
        => key is TKey tKey && data.ContainsKey(tKey);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(TKey key)
        => data.ContainsKey(key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(TKey key, out TValue value)
        => data.TryGetValue(key, out value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        => data.GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
        => data.GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IDictionaryEnumerator IDictionary.GetEnumerator()
        => data.GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        => ((ICollection<KeyValuePair<TKey, TValue>>) data).CopyTo(array, index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection.CopyTo(System.Array array, int index)
        => ((IDictionary) data).CopyTo(array, index);

    [OnDeserialized]
    private void Reinit(StreamingContext context)
    {
        Subscribe();
    }
}