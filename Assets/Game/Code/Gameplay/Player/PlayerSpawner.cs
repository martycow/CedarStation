using Cedar.Core;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class PlayerSpawner
    {
        private readonly PlayerSettings _playerSettings;
        private readonly ICedarLogger _logger;

        private PlayerView _player;
        private PlayerEmotionView _playerEmotionView;
        
        public PlayerSpawner(PlayerSettings playerSettings, ICedarLogger logger)
        {
            _playerSettings = playerSettings;
            _logger = logger;
        }
        
        public (PlayerView, PlayerEmotionView) Spawn(SpawnData spawnData)
        {
            if (_player != null && _playerEmotionView != null)
            {
                _logger.Error(SystemTag.Gameplay, "Player already exists. Cannot spawn another player.");
                return (null, null);
            }
            
            var instance = Object.Instantiate(_playerSettings.PlayerPrefab, spawnData.Position, spawnData.Rotation);
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