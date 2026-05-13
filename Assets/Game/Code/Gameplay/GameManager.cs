using System;
using Game.Core;
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
        private readonly CedarLogger _logger;
        
        private GameContext _gameContext;

        public GameManager(
            PlayerController playerController,
            IInputManager inputManager,
            LevelManager levelManager,
            SaveManager saveManager,
            LevelDataStorage levelDataStorage,
            CedarLogger logger)
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
            _playerController.KillPlayer();
        }

        private void StartNewGame()
        {
            if (_gameContext != null)
            {
                _logger.Fail(LogTag.Gameplay, "Game context already exists. Cannot start a new game.");
                return;
            }
            
            _logger.Info(LogTag.Gameplay, "Starting new game...");
            
            var defaultLevel = _levelDataStorage.DefaultLevel;
            if (defaultLevel == null)
            {
                _logger.Error(LogTag.Gameplay, "No default level found in LevelDataStorage.");
                return;
            }
            
            var randomSpawnZoneIndex = Random.Range(0, defaultLevel.PlayerSpawnZones.Length);
            var randomSpawnZone = defaultLevel.PlayerSpawnZones[randomSpawnZoneIndex];
            
            var spawnPosition = randomSpawnZone.GetRandomPosition();
            var spawnRotation = Quaternion.identity;
            
            var emptySlotData = _saveManager.CreateEmptySlot(GameDifficulty.Normal, spawnPosition, spawnRotation);
            
            StartGameWithSaveSlot(emptySlotData);
        }

        private async void StartGameWithSaveSlot(SaveSlotData saveSlotData)
        {
            try
            {
                var levelComponents = await _levelManager.LoadLevel(saveSlotData.LevelID);
                
                if (levelComponents.IsSuccess)
                {
                    _logger.Success(LogTag.Gameplay, $"Level {levelComponents.Data.DisplayName} loaded successfully.");
                
                    _inputManager.SetState(InputStateType.Gameplay);
                    var playerComponents = _playerController.SpawnPlayer(saveSlotData.SpawnPosition, saveSlotData.SpawnRotation);
                    
                    _logger.Info(LogTag.Gameplay, "Initializing game context...");
                    _gameContext = new GameContext
                    {
                        SaveSlot = saveSlotData,
                        Level = levelComponents,
                        Player = playerComponents
                    };

                    _logger.Success(LogTag.Gameplay, "Game started.");
                }
            }
            catch (Exception e)
            {
                _logger.Error(LogTag.Gameplay, $"Exception: {e}");
            }
        }
    }
}