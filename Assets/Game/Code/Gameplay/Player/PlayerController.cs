using Cedar.Core;
using Game.General;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Gameplay
{
    public sealed class PlayerController
    {
        private readonly IGameplayInputEvents _gameplayInputEvents;
        private readonly PlayerSettings _playerSettings;
        private readonly EventBus _eventBus;
        private readonly PlayerSpawner _playerSpawner;
        private readonly ICedarLogger _logger;
        
        private CharacterMover _characterMover;
        private PlayerMovementContext _playerMovementContext;
        private CharacterEmotionContext _characterEmotionContext;
        private CharacterEmotions _characterEmotions;
        
        private CharacterVisual _playerVisual;

        public PlayerController(IGameplayInputEvents gameplayGameplayInputEvents, PlayerSettings playerSettings, EventBus eventBus, PlayerSpawner playerSpawner, ICedarLogger logger)
        {
            _gameplayInputEvents = gameplayGameplayInputEvents;
            _playerSettings = playerSettings;
            _eventBus = eventBus;
            _playerSpawner = playerSpawner;
            _logger = logger;
        }

        public void SpawnPlayer(Vector3 spawnPosition, Quaternion spawnRotation)
        {
            _logger.Info(SystemTag.Player, "Spawning player");
            
            if (_characterMover != null)
            {
                _logger.Error(SystemTag.Gameplay, "Player already exists.");
                return;
            }

            // Spawning
            var components = _playerSpawner.Spawn(spawnPosition, spawnRotation);
            _characterMover = components.Movement;
            _playerVisual = components.Visual;
            _characterEmotions = components.Emotion;
            
            // Creating movement context 
            _playerMovementContext = new PlayerMovementContext(
                2.4f,
                _playerSettings.JumpCooldown,
                _playerSettings.JumpForce);
            
            _gameplayInputEvents.OnPlayerMoveChanged += OnPlayerMoveChanged;
            _gameplayInputEvents.Jump += _playerMovementContext.RequestJump;
            
            _characterMover.Setup(_playerMovementContext, ContextViewUpdateType.OnSetup | 
                                                          ContextViewUpdateType.EveryFrame | 
                                                          ContextViewUpdateType.EveryFixedUpdate);
            
            // Emotions
            _characterEmotionContext = new CharacterEmotionContext(_playerVisual, _logger);
            
            _characterEmotions.Setup(_characterEmotionContext, ContextViewUpdateType.OnSetup |
                                                               ContextViewUpdateType.EveryFrame);

            _eventBus.Publish(new PlayerCreatedEvent(_characterMover));
            
            _logger.Success(SystemTag.Player, "Player spawned successfully.");
        }

        public void DestroyPlayer()
        {
            if (_characterMover == null)
                return;
            
            _gameplayInputEvents.OnPlayerMoveChanged -= OnPlayerMoveChanged;
            _gameplayInputEvents.Jump -= _playerMovementContext.RequestJump;
            
            Object.Destroy(_characterMover.gameObject);
            
            _characterMover = null;
            _playerMovementContext = null;
            _characterEmotionContext = null;
            _eventBus.Publish(new PlayerDestroyedEvent());
        }
        
        private void OnPlayerMoveChanged(Vector2 moveInput)
        {
            var speed = _playerSettings.MoveSpeed * moveInput;
            
            _playerMovementContext.Speed.SetValue(speed);
        }
    }
}