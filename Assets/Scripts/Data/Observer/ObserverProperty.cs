using System.Linq;
using UnityEngine.Events;

namespace Utils.Observer
{
    
    public abstract class ObserverBase
    {
        internal UnityEvent<ObserverBase> OnChange { get; } = new UnityEvent<ObserverBase>();

        protected ObserverBase()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            if (this is IObserverList || this is IObserverDictionary)
                return;

            var properties = GetType().GetProperties()
                .Where(info => info.PropertyType.IsSubclassOf(typeof(ObserverBase)) && info.PropertyType != GetType());
            foreach (var property in properties)
            {
                var setMethod = property.GetMethod;
                if(!property.CanWrite)
                    continue;
                var propertyValue = (ObserverBase) property.GetValue(this);
                propertyValue?.OnChange.AddListener(OnChange.Invoke);
            }

            var fields = GetType().GetFields()
                .Where(info => info.FieldType.IsSubclassOf(typeof(ObserverBase)) && info.FieldType != GetType());
            foreach (var field in fields)
            {
                var fieldValue = (ObserverBase) field.GetValue(this);
                fieldValue.OnChange.AddListener(OnChange.Invoke);
            }
        }
    }

    public abstract class ObserverBase<T> : ObserverBase
    {
        public UnityEvent<T> OnChanged { get; } = new UnityEvent<T>();
        
        protected T value;
        
        protected ObserverBase() : base()
        {
            OnChange.AddListener((item) => OnChanged.Invoke(value));
        }
        protected ObserverBase(T value) : this() { }

    }
    public class ReadOnlyObserver<T> : ObserverBase<T>
    {
        public T Value => value;
        
        protected ReadOnlyObserver() : base() { }
        protected ReadOnlyObserver(T value) : base() { }

        public override string ToString() => Value.ToString();

        public static explicit operator T(ReadOnlyObserver<T> observer) => observer.value;
    }

    public abstract class Observer<T> : ObserverBase<T> where T : Observer<T>
    {
        public Observer() : base()
        {
            value = (T)this;
        } 
    }

    public sealed class ObserverProperty<T> : ReadOnlyObserver<T>
    {
        public ObserverProperty() : base() { }
        public ObserverProperty(T value) : base(value) { }

        public new T Value
        {
            get => value;
            set
            {
                this.value = value;
                OnChange.Invoke(this);
            }
        }
        public static explicit operator T(ObserverProperty<T> observer) => observer.value;
    }
}