using CedarStation.Core.DI;
using UnityEngine;
using Logger = CedarStation.Helpers.Logger;
using LogType = CedarStation.Helpers.LogType;

namespace CedarStation.Gameplay
{
    public sealed class Bootstrap : MonoBehaviour, IContainerHandler
    {
        public Container Container { get; private set; }

        public static Logger Logger => logger ?? new Logger();
        private static Logger logger;

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Dispose();
        }
        
        public void Initialize()
        {
            Logger.Info($"Running {Application.productName} v{Application.version}...");
            Logger.Info("Initializing container...", LogType.Container);
            
            var builder = new ContainerBuilder();

            // ...
            
            Container = builder.Build(Logger);
            var allInstancedMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var instance in allInstancedMonoBehaviours)
                Container.Inject(instance);
            
            logger.Info("Container initialized successfully.", LogType.Container);
        }
        
        public void Dispose()
        {
            Container?.Dispose();
        }
    }
}