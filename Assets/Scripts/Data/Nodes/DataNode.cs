using Utils.Observer;

namespace Data.Nodes
{
    public abstract class DataNodeBase : ObserverBase<DataNodeBase> { }
    public class DataNodeTree : DataNodeBase
    {
        private ObserverDictionary<string, DataNodeBase> childs = new ObserverDictionary<string, DataNodeBase>();

        public void AddNode<T>(string key, T value) where T : DataNodeTree
        {
            childs.Add(key, value);
        }

        public void SetChild<T>(string key, T value)
        {
            if (!childs.ContainsKey(key))
            {
                childs.Add(key, new DataNode<T>(value));
            }
            else
            {
                var node = (DataNode<T>) childs[key];
                node.Property.Value = value;
            }
        }

        public ObserverProperty<T> GetChild<T>(string key)
        {
            var observerValue = (DataNode<T>)childs[key];
            return  observerValue.Property;
        }
    }

    public class DataNode<T> : DataNodeBase
    {
        public ObserverProperty<T> Property;
        public DataNode(T value)
        {
            this.Property = new ObserverProperty<T>(value);
        }
    }
}