using Cedar.Core;
using Game.General;
using Game.Input;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class ApplicationScope : MonoSingleton, IContainerScope
    {
        [SerializeField] private LoggerSettings loggerSettings;

        public ICedarContainer RootContainer { get; private set; }

        private InputActions _inputActions;

        protected override void AwakeImpl()
        {
            Initialize(new CedarLogger(loggerSettings));
            
#if UNITY_EDITOR
            LoadAdditiveGame();
#endif
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void Initialize(ICedarLogger logger)
        {
            _inputActions = new InputActions();
            
            logger.Info(SystemTag.Application, $"Starting {Application.productName} v{Application.version}...");
            RootContainer = CreateBuilder(logger).Build();

            // Injecting dependencies into MonoBehaviours
            var monoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var instance in monoBehaviours)
                RootContainer.Inject(instance);
            
            RootContainer.Initialize();
            logger.Success(SystemTag.Application, $"{Application.productName} started.");
        }

        public void Dispose()
        {
            _inputActions?.Dispose();
            RootContainer?.Dispose();
        }

        private CedarContainerBuilder CreateBuilder(ICedarLogger logger)
        {
            var builder = new CedarContainerBuilder(logger);

            builder.RegisterInstance(logger);
            builder.RegisterInstance(_inputActions);
            builder.Register<EventBus>();

            // Gameplay controls
            builder.Register<GameplayInputState>();
            builder.Register<IGameplayInputEvents, GameplayInputState>();

            // Menu controls
            builder.Register<MenuInputState>();
            builder.Register<IMenuInputEvents, MenuInputState>();

            // No control mode
            builder.Register<NoControlState>();

            builder.Register<IInputManager, InputManager>();
            
            return builder;
        }

#if UNITY_EDITOR
        private void LoadAdditiveGame()
        {
            var gameScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(Const.Main.GameScene);
            if (!gameScene.isLoaded)
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Const.Main.GameScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
#endif
    }
}
    
    