using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.Views
{
    public class ViewRepository : MonoBehaviour
    {
        private Dictionary<Type,UIBase> uiDictionary = new Dictionary<Type, UIBase>();
        private Dictionary<Type, GameObject> uiPrefabsDictionary;
        private Dictionary<Type, GameObject> UiPrefabsDictionary
        {
            get
            {
                if (uiPrefabsDictionary != null) return uiPrefabsDictionary;
                
                var prefabs = Resources.LoadAll<GameObject>(prefabsLocalPath)
                    .Where(prefab => prefab.name.EndsWith(".prefab"));
                uiPrefabsDictionary = new Dictionary<Type, GameObject>();
                
                foreach (var prefab in prefabs)
                    uiPrefabsDictionary.Add(prefab.name.GetType(), prefab);

                return uiPrefabsDictionary;
            }
        }
    

        private const string prefabsLocalPath = "Assets/Prefabs/Resources/Views";

        public void Show<T1, T2>(T2 data) where T2 : IUIData where T1: UIBase<T2>
        { 
            var a = GetView<T1>();
            a.SetData(data);
        }

        private T GetView<T>() where T : UIBase
        {
            if (uiDictionary.TryGetValue(typeof(T), out var value))
                return (T)value;
            var view = Instantiate(UiPrefabsDictionary[typeof(T)]);
            uiDictionary.Add(typeof(T), view.GetComponent<UIBase>());
            return (T)uiDictionary[typeof(T)];
        }
    }
}