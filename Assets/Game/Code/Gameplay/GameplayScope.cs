using Cedar.Core;
using Game.General;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class GameplayScope : MonoSingleton, IContainerScope
    {
        [SerializeField] private PlayerSettings playerSettings;
        
        public ICedarContainer Container { get; private set; }

        protected override void AwakeImpl()
        {
            name = $"Scope_{Const.Main.GameplayScene}";
            
            // Looking for App-level container
            var appScope = FindAnyObjectByType<ApplicationScope>();
            if (appScope == null)
            {
                Debug.LogError("Application Scope not found in the scene.");
                return;
            }

            // Creating container for Gameplay purposes
            var logger = appScope.Container.Resolve<ICedarLogger>();
            Container = CreateAndInitContainer(logger, appScope.Container);
            
            // For now - start game
            var starter = Container.Resolve<GameStarter>();
            starter.Start();
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            Container?.Dispose();
        }
        
        public ICedarContainer CreateAndInitContainer(ICedarLogger logger, ICedarContainer parent)
        {
            var builder = CreateBuilder(Const.Main.GameplayScene, logger, parent);
            var container = builder.Build();
            
            // Injecting dependencies into MonoBehaviours
            var monoBehaviours = FindObjectsByType<MonoBehaviour>();
            foreach (var instance in monoBehaviours)
                container.Inject(instance);
            
            container.Initialize();
            return container;
        }

        public ICedarContainerBuilder CreateBuilder(string containerName, ICedarLogger logger, ICedarContainer parent)
        {
            var builder = new CedarContainerBuilder(containerName, logger, parent);

            // Player management
            builder.RegisterInstance(playerSettings);
            builder.Register<PlayerSpawner>();
            builder.Register<PlayerController>();

            // Starter
            builder.Register<GameStarter>();

            return builder;
        }
    }
}