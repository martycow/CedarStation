using Cedar.Core;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class PlayerSpawner
    {
        private readonly PlayerSettings _playerSettings;
        private readonly ICedarLogger _logger;

        private Player _player;
        private PlayerEmotionView _playerEmotionView;
        
        public PlayerSpawner(PlayerSettings playerSettings, ICedarLogger logger)
        {
            _playerSettings = playerSettings;
            _logger = logger;
        }
        
        public (Player, PlayerEmotionView) Spawn(Vector3 spawnPos, Quaternion spawnRot)
        {
            if (_player != null && _playerEmotionView != null)
            {
                _logger.Error(SystemTag.Gameplay, "Player already exists. Cannot spawn another player.");
                return (null, null);
            }
            
            var instance = Object.Instantiate(_playerSettings.PlayerPrefab, spawnPos, spawnRot);
            var playerEmotionView = instance.GetComponent<PlayerEmotionView>();

            return (instance, playerEmotionView);
        }

        public void Kill()
        {
            if (_player == null)
            {
                _logger.Error(SystemTag.Gameplay, "No player to kill.");
                return;
            }
            
            Object.Destroy(_player.gameObject);
            
            _player = null;
            _playerEmotionView = null;
        }
    }
}