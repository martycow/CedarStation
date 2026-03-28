using System;
using Cedar.Core;
using Game.General;
using Game.Input;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class GameScope : MonoSingleton, IContainerScope
    {
        [SerializeField] private LoggerSettings loggerSettings;
        
        public ICedarContainer CedarContainer { get; private set; }

        private bool _isInitialized;
        private InputActions _inputActions;

        protected override void AwakeImpl()
        {
            Initialize(new CedarLogger(loggerSettings));
        }

        private void OnDestroy()
        {
            Dispose();
        }
        
        public void Initialize(ICedarLogger logger)
        {
            if (_isInitialized)
                return;
            
            _inputActions = new InputActions();

            logger.Info(SystemTag.Application, $"Starting {Application.productName} v{Application.version}...");
            
            CedarContainer = CreateBuilder(logger).Build();
                
            // Injecting dependencies into MonoBehaviours
            var allInstancedMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var instance in allInstancedMonoBehaviours)
                CedarContainer.Inject(instance);
                
            // Triggering initialization
            CedarContainer.Initialize();
            _isInitialized = true;
            
            logger.Success(SystemTag.Application, $"{Application.productName} started.");
        }
        
        public void Dispose()
        {
            _inputActions?.Dispose();
            CedarContainer?.Dispose();
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
            
            // Player management
            builder.Register<PlayerController>();
            
            return builder;
        }
    }
}