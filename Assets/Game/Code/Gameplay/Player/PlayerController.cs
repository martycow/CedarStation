using Game.Core;
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
        private readonly CedarLogger _logger;
        
        private CharacterMover _characterMover;
        private PlayerMovementContext _playerMovementContext;
        private CharacterEmotionContext _characterEmotionContext;
        private CharacterEmotions _characterEmotions;
        
        private CharacterVisual _playerVisual;

        public PlayerController(IGameplayInputEvents gameplayGameplayInputEvents, PlayerSettings playerSettings, EventBus eventBus, PlayerSpawner playerSpawner, CedarLogger logger)
        {
            _gameplayInputEvents = gameplayGameplayInputEvents;
            _playerSettings = playerSettings;
            _eventBus = eventBus;
            _playerSpawner = playerSpawner;
            _logger = logger;
        }

        public PlayerComponents SpawnPlayer(Vector3 spawnPosition, Quaternion spawnRotation)
        {
            _logger.Info(LogTag.Player, "Spawning player...");
            
            if (_playerSpawner.Player.IsSpawned)
            {
                _logger.Error(LogTag.Gameplay, "Player already exists.");
                return PlayerComponents.Empty;
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

            _eventBus.Publish(new PlayerSpawnedEvent(_characterMover));
            
            _logger.Success(LogTag.Player, "Player spawned successfully.");
            
            return new PlayerComponents(true, _playerSettings, _playerVisual, _characterMover, _characterEmotions);
        }

        public void KillPlayer()
        {
            if (!_playerSpawner.Player.IsSpawned)
                return;
            
            _gameplayInputEvents.OnPlayerMoveChanged -= OnPlayerMoveChanged;
            _gameplayInputEvents.Jump -= _playerMovementContext.RequestJump;
            
            _playerSpawner.Kill();
            
            _characterMover = null;
            _playerMovementContext = null;
            _characterEmotionContext = null;
            _eventBus.Publish(new PlayerKilledEvent());
        }
        
        private void OnPlayerMoveChanged(Vector2 moveInput)
        {
            var motion = _playerSettings.MoveSpeed * moveInput;
            
            _playerMovementContext.Motion.SetValue(motion);
        }
    }
}