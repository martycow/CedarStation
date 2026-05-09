using Cedar.Core;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class PlayerSpawner
    {
        private readonly PlayerSettings _playerSettings;
        private readonly ICedarLogger _logger;

        private bool _spawned = false;
        private CharacterMover _characterMover;
        private CharacterVisual _playerVisual;
        private CharacterEmotions _characterEmotions;
        
        public PlayerSpawner(PlayerSettings playerSettings, ICedarLogger logger)
        {
            _playerSettings = playerSettings;
            _logger = logger;
        }
        
        public PlayerComponents Spawn(Vector3 spawnPos, Quaternion spawnRot)
        {
            if (_spawned || _playerVisual != null || _characterMover != null || _characterEmotions != null)
            {
                _logger.Error(SystemTag.Player, "Player already spawned.");
                return PlayerComponents.Empty;
            }
            
            _characterMover = Object.Instantiate(_playerSettings.CharacterMoverPrefab, spawnPos, spawnRot);
            _playerVisual = _characterMover.GetComponent<CharacterVisual>();
            _characterEmotions = _characterMover.GetComponent<CharacterEmotions>();
            _spawned = true;
            
            _logger.Info(SystemTag.Player, "Created player instance.");
            
            return new PlayerComponents(_playerSettings, _playerVisual, _characterMover, _characterEmotions);
        }

        public void Kill()
        {
            if (_characterMover == null)
            {
                _logger.Error(SystemTag.Player, "No player to kill.");
                return;
            }
            
            Object.Destroy(_characterMover.gameObject);
            
            _playerVisual = null;
            _characterMover = null;
            _characterEmotions = null;
            _spawned = false;
        }
    }
}