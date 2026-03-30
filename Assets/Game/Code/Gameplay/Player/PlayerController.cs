using System;
using Cedar.Core;
using Game.General;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class PlayerController : IInitializable, IDisposable
    {
        private readonly IGameplayInputEvents _gameplayInputEvents;
        private readonly PlayerSettings _playerSettings;
        private readonly EventBus _eventBus;
        private readonly PlayerSpawner _playerSpawner;
        private readonly ICedarLogger _logger;
        
        private PlayerView _player;
        private PlayerContext _playerContext;

        public PlayerController(IGameplayInputEvents gameplayGameplayInputEvents, PlayerSettings playerSettings, EventBus eventBus, PlayerSpawner playerSpawner, ICedarLogger logger)
        {
            _gameplayInputEvents = gameplayGameplayInputEvents;
            _playerSettings = playerSettings;
            _eventBus = eventBus;
            _playerSpawner = playerSpawner;
            _logger = logger;
        }
        
        public void Initialize()
        {
            var data = new SpawnData
            {
                Position = Vector3.zero,
                Rotation = Quaternion.identity
            };
            
            InitPlayer(data);
        }
        
        public void Dispose()
        {
            _gameplayInputEvents.MovePerformed -= OnMovePerformed;
            _gameplayInputEvents.MoveCanceled -= MoveCanceled;
        }

        public void InitPlayer(SpawnData data)
        {
            if (_player != null)
                return;
            
            _player = _playerSpawner.Spawn(data);
            _playerContext = new PlayerContext
            {
                MoveInput = Vector2.zero,
                MoveSpeed = _playerSettings.MoveSpeed
            };
            
            _player.Setup(_playerContext, ViewUpdateType.OnSetup | ViewUpdateType.EveryFrame);
            
            _gameplayInputEvents.MovePerformed += OnMovePerformed;
            _gameplayInputEvents.MoveCanceled += MoveCanceled;
        }
        
        private void OnMovePerformed(Vector2 inputValue)
        {
            if (_playerContext == null)
                return;

            _playerContext.MoveInput = inputValue;
        }
        
        private void MoveCanceled(Vector2 inputValue)
        {
            if (_playerContext == null)
                return;

            _playerContext.MoveInput = inputValue;
        }
    }
}