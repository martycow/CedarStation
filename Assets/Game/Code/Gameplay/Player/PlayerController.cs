using Cedar.Core;
using Game.General;
using Game.Input;

namespace Game.Gameplay
{
    public sealed class PlayerController : IInitializable
    {
        private readonly IGameplayInputEvents _gameplayInputEvents;
        private readonly EventBus _eventBus;
        private readonly ICedarLogger _logger;
        
        public PlayerController(IGameplayInputEvents gameplayGameplayInputEvents, EventBus eventBus, ICedarLogger logger)
        {
            _gameplayInputEvents = gameplayGameplayInputEvents;
            _eventBus = eventBus;
            _logger = logger;
        }
        
        public void Initialize()
        {
            _logger.Success(SystemTag.Gameplay, "Initialized.");
        }
    }
}