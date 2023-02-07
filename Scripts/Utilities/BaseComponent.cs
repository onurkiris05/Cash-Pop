using UnityEngine;

namespace _Main.Scripts.Utilities
{
    public abstract class BaseComponent<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected T BaseComp;
        protected bool _hasInitialized = false;

        //public T BaseComponentt => BaseComp;

        internal virtual void Initialize()
        {
            if (_hasInitialized)
                return;
            
            BaseComp = GetComponent<T>();
            _hasInitialized = true;
        }

        internal virtual void ResetComponent(){}
    }
}