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
        
        private PlayerView _player;
        private PlayerInputContext _playerInputContext;
        private CharacterEmotionContext _characterEmotionContext;
        private PlayerEmotionView _playerEmotionView;
        
        private CharacterVisual _playerVisual;

        public PlayerController(IGameplayInputEvents gameplayGameplayInputEvents, PlayerSettings playerSettings, EventBus eventBus, PlayerSpawner playerSpawner, ICedarLogger logger)
        {
            _gameplayInputEvents = gameplayGameplayInputEvents;
            _playerSettings = playerSettings;
            _eventBus = eventBus;
            _playerSpawner = playerSpawner;
            _logger = logger;
        }

        public void CreatePlayer(SpawnData data)
        {
            if (_player != null)
            {
                _logger.Error(SystemTag.Gameplay, "Player already exists.");
                return;
            }

            (_player, _playerEmotionView) = _playerSpawner.Spawn(data);
            _playerVisual = _player.Visual;
            
            // Input
            _playerInputContext = new PlayerInputContext
            {
                MoveInput = Vector2.zero,
                MoveSpeed = _playerSettings.MoveSpeed,
                JumpCooldown = _playerSettings.JumpCooldown,
                JumpForce = _playerSettings.JumpForce,
                JumpInput = false
            };
            _gameplayInputEvents.OnPlayerMoveChanged += SetPlayerMoveInput;
            _gameplayInputEvents.Jump += OnJumpInput;
            
            _player.Setup(_playerInputContext, ViewUpdateType.OnSetup | 
                                               ViewUpdateType.EveryFrame | 
                                               ViewUpdateType.EveryFixedUpdate);
            
            // Emotions
            _characterEmotionContext = new CharacterEmotionContext(_playerVisual, _logger);
            
            _playerEmotionView.Setup(_characterEmotionContext, ViewUpdateType.OnSetup |
                                                               ViewUpdateType.EveryFrame);

            _eventBus.Publish(new PlayerCreatedEvent(_player));
        }

        public void DestroyPlayer()
        {
            if (_player == null)
                return;
            
            Object.Destroy(_player.gameObject);
            _player = null;
            _playerInputContext = null;
            _characterEmotionContext = null;
            _gameplayInputEvents.OnPlayerMoveChanged -= SetPlayerMoveInput;
            _gameplayInputEvents.Jump -= OnJumpInput;
            _eventBus.Publish(new PlayerDestroyedEvent());
        }
        
        private void SetPlayerMoveInput(Vector2 inputValue)
        {
            if (_playerInputContext == null)
                return;

            _playerInputContext.MoveInput = inputValue;
        }
        
        private void OnJumpInput()
        {
            if (_playerInputContext == null)
                return;
            
            _playerInputContext.JumpInput = true;
        }
    }
}