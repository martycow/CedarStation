using System;
using Cedar.Core;
using Game.General;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Gameplay
{
    internal sealed class GameManager : IInitializable, IDisposable
    {
        private readonly PlayerController _playerController;
        private readonly IInputManager _inputManager;
        private readonly LevelManager _levelManager;
        private readonly SaveManager _saveManager;
        private readonly LevelDataStorage _levelDataStorage;
        private readonly ICedarLogger _logger;

        public GameManager(
            PlayerController playerController,
            IInputManager inputManager,
            LevelManager levelManager,
            SaveManager saveManager,
            LevelDataStorage levelDataStorage,
            ICedarLogger logger)
        {
            _playerController = playerController;
            _inputManager = inputManager;
            _levelManager = levelManager;
            _saveManager = saveManager;
            _levelDataStorage = levelDataStorage;
            _logger = logger;
        }
        
        public void Initialize()
        {
            StartNewGame();
        }
        
        public void Dispose()
        {
            _playerController.DestroyPlayer();
        }

        private void StartNewGame()
        {
            _logger.Info(SystemTag.Gameplay, "Starting new game...");

            var defaultLevel = _levelDataStorage.DefaultLevel;
            if (defaultLevel == null)
            {
                _logger.Error(SystemTag.Gameplay, "No default level found in LevelDataStorage.");
                return;
            }
            
            var randomSpawnZoneIndex = Random.Range(0, defaultLevel.PlayerSpawnZones.Length);
            var randomSpawnZone = defaultLevel.PlayerSpawnZones[randomSpawnZoneIndex];
            
            var spawnPosition = randomSpawnZone.GetRandomPosition();
            var spawnRotation = Quaternion.identity;
            
            var emptySlotData = _saveManager.CreateEmptySlot(GameDifficulty.Normal, spawnPosition, spawnRotation);
            
            StartGameWithSaveSlot(emptySlotData);
        }

        private void StartGameWithSaveSlot(SaveSlotData saveSlotData)
        {
            var (level, levelData) = _levelManager.LoadLevel(saveSlotData.LevelID);
            
            _inputManager.SetState(InputStateType.Gameplay);
            _playerController.SpawnPlayer(saveSlotData.SpawnPosition, saveSlotData.SpawnRotation);
            
            _logger.Line();
            _logger.Success(SystemTag.Gameplay, "Game started.");
            _logger.Line();
        }
    }
}