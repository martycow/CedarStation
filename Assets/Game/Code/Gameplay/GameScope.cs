using Cedar.Core;
using Game.General;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class GameScope : MonoSingleton, IContainerScope
    {
        public ICedarContainer RootContainer { get; private set; }
        
        protected override void AwakeImpl()
        {
            var appScope = FindAnyObjectByType<ApplicationScope>();
            var logger = appScope.RootContainer.Resolve<ICedarLogger>();
            
            Initialize(logger, appScope.RootContainer);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            RootContainer?.Dispose();
        }

        private void Initialize(ICedarLogger logger, ICedarContainer parent)
        {
            var builder = new CedarContainerBuilder(logger, parent);

            // Player management
            builder.Register<PlayerController>();
            
            RootContainer = builder.Build();
            
            // Injecting dependencies into MonoBehaviours
            var monoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var instance in monoBehaviours)
                RootContainer.Inject(instance);
            
            RootContainer.Initialize();
        }
    }
}