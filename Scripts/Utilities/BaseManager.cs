using UnityEngine;

namespace _Main.Scripts.Utilities
{
    public abstract class BaseManager<T> : MonoBehaviour where T : Singleton<T>
    {
        protected T BaseComp;
        
        public virtual void Initialize()
        {
            BaseComp = Singleton<T>.Instance;
        }

        public virtual void Terminate() { }
    }
}