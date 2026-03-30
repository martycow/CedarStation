using UnityEngine;

namespace Game.Gameplay
{
    public sealed class PlayerSpawner
    {
        private readonly PlayerSettings _playerSettings;
        
        public PlayerSpawner(PlayerSettings playerSettings)
        {
            _playerSettings = playerSettings;
        }
        
        public PlayerView Spawn(SpawnData spawnData)
        {
            var instance = Object.Instantiate(_playerSettings.PlayerPrefab, spawnData.Position, spawnData.Rotation);

            return instance;
        }
    }
}