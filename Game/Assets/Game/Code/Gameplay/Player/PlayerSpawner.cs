using Game.Core;
using Game.General;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class PlayerSpawner
    {
        public PlayerComponents Player => new(
            _spawned, 
            _playerSettings, 
            _playerVisual, 
            _characterMover, 
            _characterEmotions);
        
        private readonly PlayerSettings _playerSettings;
        private readonly CedarLogger _logger;

        private bool _spawned;
        private CharacterMover _characterMover;
        private CharacterVisual _playerVisual;
        private CharacterEmotions _characterEmotions;
        
        public PlayerSpawner(PlayerSettings playerSettings, CedarLogger logger)
        {
            _playerSettings = playerSettings;
            _logger = logger;
        }
        
        public PlayerComponents Spawn(Vector3 spawnPos, Quaternion spawnRot)
        {
            if (_spawned || _playerVisual != null || _characterMover != null || _characterEmotions != null)
            {
                _logger.Error(LogTag.Player, "Player already spawned.");
                return PlayerComponents.Empty;
            }
            
            _characterMover = Object.Instantiate(_playerSettings.CharacterMoverPrefab, spawnPos, spawnRot);
            _playerVisual = _characterMover.GetComponent<CharacterVisual>();
            _characterEmotions = _characterMover.GetComponent<CharacterEmotions>();
            _spawned = true;
            
            _logger.Info(LogTag.Player, "Created player instance.");
            
            return new PlayerComponents(true, _playerSettings, _playerVisual, _characterMover, _characterEmotions);
        }

        public void Kill()
        {
            if (_characterMover == null)
            {
                _logger.Error(LogTag.Player, "No player to kill.");
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