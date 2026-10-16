using Game.Core;
using Game.General;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class GameplayScope : MonoSingleton
    {
        [SerializeField]
        private PlayerSettings playerSettings;
        
        [SerializeField]
        private LevelDataStorage levelDataStorage;
        
        [SerializeField]
        private SaveSystemSettings saveSystemSettings;

        [SerializeField]
        private CinemachineBrain cinemachineBrain;

        public ICedarContainer Container { get; private set; }

        protected override void AwakeImpl()
        {
            name = $"Scope_{Const.Scope.GameplayScope}";
            
            // Looking for App-level container
            var appScope = FindAnyObjectByType<ApplicationScope>();
            if (appScope == null)
            {
                Debug.LogError("Application Scope not found in the scene.");
                return;
            }
            
            var logger = appScope.Container.Resolve<CedarLogger>();
            Container = CreateAndInitContainer(logger, appScope.Container);

            var gameManager = Container.Resolve<GameManager>();
            gameManager.StartNewGame(GameDifficulty.Normal);
        }

        private void OnDestroy()
        {
            Container?.Dispose();
        }

        private ICedarContainer CreateAndInitContainer(CedarLogger logger, ICedarContainer parent)
        {
            var builder = CreateBuilder(Const.Scope.GameplayScope, logger, parent);
            var container = builder.Build();
            
            InjectMonoBehaviours(container);
            InjectScriptableObjects(container);
            
            container.Initialize();
            return container;
        }

        private ICedarContainerBuilder CreateBuilder(string containerName, CedarLogger logger, ICedarContainer parent)
        {
            var builder = new CedarContainerBuilder(containerName, logger, parent);

            // Player management
            builder.RegisterInstance(playerSettings);
            builder.Register<PlayerSpawner>();
            builder.Register<PlayerController>();

            // Save management
            builder.RegisterInstance(saveSystemSettings);
            builder.Register<SaveManager>();
            
            // Level management
            builder.RegisterInstance(levelDataStorage);
            levelDataStorage.Initialize();
            builder.Register<LevelManager>();
            
            // Camera
            builder.RegisterInstance(cinemachineBrain);
            builder.Register<CameraController>();

            // Starter
            builder.Register<GameManager>();

            return builder;
        }
        
        private void InjectMonoBehaviours(ICedarContainer container)
        {
            // Injecting dependencies into MonoBehaviours
            var monoBehaviours = FindObjectsByType<MonoBehaviour>();
            foreach (var instance in monoBehaviours)
                container.Inject(instance);
        }

        private void InjectScriptableObjects(ICedarContainer container)
        {
            // Injecting dependencies into scriptable objects
            container.Inject(playerSettings);
            container.Inject(levelDataStorage);
            container.Inject(saveSystemSettings);
        }
    }
}