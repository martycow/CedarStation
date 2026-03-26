using UnityEngine;

namespace Game.General
{
    public abstract class MonoSingleton : MonoBehaviour
    {
        public static MonoSingleton Singleton { get; private set; }

        private void Awake()
        {
            Singleton = this;
            AwakeImpl();
        }

        protected abstract void AwakeImpl();
    }
}