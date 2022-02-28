using UnityEngine;

namespace UI
{
    public abstract class UIBase : MonoBehaviour { }

    public abstract class UIBase<T> : UIBase where T : IUIData
    {
        public abstract void SetData(T data);
    }
}