using Utils.Observer;

namespace Units
{
    public class UnitData : Observer<UnitData>
    {
        public ObserverProperty<float> CurrentHP { get; } = new ObserverProperty<float>(0);
        public ObserverProperty<float> CurrentMP { get; } = new ObserverProperty<float>(0);
        private ObserverDictionary<string, ObserverBase> Attributes { get; } = new ObserverDictionary<string, ObserverBase>();

        public UnitData()
        {
            Attributes.Add("HP", new ObserverProperty<int>(0));
            Attributes.Add("MP", new ObserverProperty<int>(0));
            Attributes.Add("AttackDamage", new ObserverProperty<float>(0));
            Attributes.Add("AttackSpeed", new ObserverProperty<float>(0));
            Attributes.Add("HPRegen", new ObserverProperty<float>(0));
            Attributes.Add("MPRegen", new ObserverProperty<float>(0));
        }

        public ObserverProperty<int> MaxHP => GetAttribute<int>("HP");
        public ObserverProperty<int> MaxMP => GetAttribute<int>("MP");
        public ObserverProperty<float> AttackDamage => GetAttribute<float>("AttackDamage");
        public ObserverProperty<float> AttackSpeed => GetAttribute<float>("AttackSpeed");
        public ObserverProperty<float> HPRegen => GetAttribute<float>("HPRegen");
        public ObserverProperty<float> MPRegen => GetAttribute<float>("MPRegen");

        private ObserverProperty<T> GetAttribute<T>(string name) where T : struct
        {
            return Attributes[name] as ObserverProperty<T>;
        }
    }
}