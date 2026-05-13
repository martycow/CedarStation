using Game.Core;
using Game.General;
using Game.Input;
using UnityEngine;

namespace Game.Gameplay
{
    /// <summary>
    /// Top-level scope for the entire application. Runs core systems
    /// </summary>
    public sealed class ApplicationScope : MonoSingleton
    {
        [SerializeField] 
        private LoggerSettings loggerSettings;

        public ICedarContainer Container { get; private set; }

        private InputActions _inputActions;

        protected override void AwakeImpl()
        {
            name = $"Scope_{Const.Scope.ApplicationScope}";
            
            // Creating root logger
            var logger = new CedarLogger(loggerSettings);
            logger.Line();
            logger.Info(LogTag.Application, $"Starting {Application.productName} v{Application.version}...");
            logger.Line();
            
            // Creating App-level container (no parent)
            Container = CreateAndInitContainer(logger, parent: null);

            // By default, no control
            var inputManager = Container.Resolve<IInputManager>();
            inputManager.SetState(InputStateType.NoControl);
            
            // For now - load Game scene and stuff
            Utilities.Scenes.Load(Const.Scope.GameplayScope);
            
            logger.Success(LogTag.Application, $"{Application.productName} started.");
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

        private ICedarContainer CreateAndInitContainer(CedarLogger logger, ICedarContainer parent)
        {
            _inputActions = new InputActions();

            var builder = CreateBuilder(Const.Scope.ApplicationScope, logger, parent);
            var container = builder.Build();

            // Injecting dependencies into MonoBehaviours
            var monoBehaviours = FindObjectsByType<MonoBehaviour>();
            foreach (var instance in monoBehaviours)
                container.Inject(instance);
            
            container.Initialize();
            return container;
        }

        private ICedarContainerBuilder CreateBuilder(string containerName, CedarLogger logger, ICedarContainer parent = null)
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
    
    