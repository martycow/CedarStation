using System;
using CedarStation.Core.DI;
using CedarStation.Helpers;
using UnityEngine;

namespace CedarStation.Gameplay
{
    public sealed class GameScope : MonoBehaviour, IContainerScope
    {
        public static GameScope Instance { get; private set; }
        public Container Container { get; private set; }

        private bool _isInitialized;
        
        private void Awake()
        {
            Instance = this;
            Initialize(new CedarLogger());
        }

        private void OnDestroy()
        {
            Dispose();
        }
        
        public void Initialize(ICedarLogger logger)
        {
            if (_isInitialized)
                return;
            
            try
            {
                logger.Info(LogTag.Default, $"Running {Application.productName} v{Application.version}...");
                logger.Info(LogTag.Container, "Initializing container...");
                
                Container = CreateBuilder(logger).Build();
                
                var allInstancedMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
                foreach (var instance in allInstancedMonoBehaviours)
                    Container.Inject(instance);
                
                Container.Initialize();
                _isInitialized = true;
                logger.Success(LogTag.Container, "Container initialized!");
            }
            catch (Exception e)
            {
                logger.Fail(LogTag.Container, $"Container initialization failed: {e.Message} {e.StackTrace}.");
                throw;
            }
        }
        
        public void Dispose()
        {
            Container?.Dispose();
        }

        private static ContainerBuilder CreateBuilder(ICedarLogger logger)
        {
            var builder = new ContainerBuilder(logger);
            
            // Registering non-unity instances
            builder.RegisterInstance(logger);
            
            return builder;
        }
    }
}