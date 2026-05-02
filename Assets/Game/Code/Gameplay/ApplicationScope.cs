using Cedar.Core;
using Game.General;
using Game.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Gameplay
{
    /// <summary>
    /// Top-level scope for the entire application. Runs core systems
    /// </summary>
    public sealed class ApplicationScope : MonoSingleton, IContainerScope
    {
        [SerializeField] private LoggerSettings loggerSettings;

        public ICedarContainer Container { get; private set; }

        private InputActions _inputActions;

        protected override void AwakeImpl()
        {
            name = $"Scope_{Const.Main.ApplicationScene}";
            
            // Creating root logger
            var logger = new CedarLogger(loggerSettings);
            logger.Line();
            logger.Info(SystemTag.Application, $"Starting {Application.productName} v{Application.version}...");
            logger.Line();
            
            // Creating App-level container (no parent)
            Container = CreateAndInitContainer(logger, parent: null);

            // By default, no control
            var inputManager = Container.Resolve<IInputManager>();
            inputManager.SetState(InputStateType.NoControl);
            
            // For now - load Game scene and stuff
            Utilities.Scenes.Load(Const.Main.GameplayScene);
            
            logger.Line();
            logger.Success(SystemTag.Application, $"{Application.productName} started.");
            logger.Line();
        }

        private void OnDestroy()
        {
            Dispose();
        }
        
        public void Dispose()
        {
            _inputActions?.Dispose();
            Container?.Dispose();
        }
        
        public ICedarContainer CreateAndInitContainer(ICedarLogger logger,  ICedarContainer parent)
        {
            _inputActions = new InputActions();

            var builder = CreateBuilder(Const.Main.ApplicationScene, logger, parent);
            var container = builder.Build();

            // Injecting dependencies into MonoBehaviours
            var monoBehaviours = FindObjectsByType<MonoBehaviour>();
            foreach (var instance in monoBehaviours)
                container.Inject(instance);
            
            container.Initialize();
            return container;
        }

        public ICedarContainerBuilder CreateBuilder(string containerName, ICedarLogger logger, ICedarContainer parent = null)
        {
            var builder = new CedarContainerBuilder(containerName, logger, parent);

            builder.RegisterInstance(logger);
            builder.Register<EventBus>();

            // Input System
            {
                builder.RegisterInstance(_inputActions);
                
                // Gameplay controls
                builder.Register<GameplayInputState>();
                builder.Register<IGameplayInputEvents, GameplayInputState>();

                // Menu controls
                builder.Register<MenuInputState>();
                builder.Register<IMenuInputEvents, MenuInputState>();

                // No control mode
                builder.Register<NoControlState>();

                builder.Register<IInputManager, InputManager>();
            }
            
            return builder;
        }
    }
}
    
    