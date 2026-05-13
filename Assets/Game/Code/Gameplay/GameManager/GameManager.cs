using System;
using System.Threading.Tasks;
using Game.General;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Gameplay
{
    public sealed class GameManager : IDisposable
    {
        private readonly PlayerController _playerController;
        private readonly IInputManager _inputManager;
        private readonly LevelManager _levelManager;
        private readonly SaveManager _saveManager;
        private readonly LevelDataStorage _levelDataStorage;
        private readonly EventBus _eventBus;
        private readonly CedarLogger _logger;
        
        private GameContext _gameContext;

        public GameManager(
            PlayerController playerController,
            IInputManager inputManager,
            LevelManager levelManager,
            SaveManager saveManager,
            LevelDataStorage levelDataStorage,
            EventBus eventBus,
            CedarLogger logger)
        {
            _playerController = playerController;
            _inputManager = inputManager;
            _levelManager = levelManager;
            _saveManager = saveManager;
            _levelDataStorage = levelDataStorage;
            _eventBus = eventBus;
            _logger = logger;
        }
        
        public void Dispose()
        {
            _playerController.KillPlayer();
            _gameContext = null;
        }

        public async void StartNewGame(GameDifficulty difficulty)
        {
            try
            {
                if (_gameContext != null)
                {
                    _logger.Fail(LogTag.GameManager, "Game context already exists. Cannot start a new game.");
                    return;
                }
                
                var firstLevel = _levelDataStorage.GetFirstLevel();
                if (firstLevel == null)
                    return;
            
                // Selecting random spawn point on a level
                var randomSpawnZoneIndex = Random.Range(0, firstLevel.PlayerSpawnZones.Length);
                var randomSpawnZone = firstLevel.PlayerSpawnZones[randomSpawnZoneIndex];
            
                var spawnPosition = randomSpawnZone.GetRandomPosition();
                var spawnRotation = Quaternion.identity;
            
                // Creating new save slot
                var emptySlotData = _saveManager.CreateEmptySlot(difficulty, firstLevel.ID, spawnPosition, spawnRotation);
                
                _logger.Line();
                _logger.Info(LogTag.GameManager, $"Starting new game. Difficulty: {difficulty}.");
            
                var (isSuccess, gameContext) = await StartGameWithSaveSlot(emptySlotData);
                if (!isSuccess) 
                    return;
                
                _gameContext = gameContext;
                _eventBus.Publish(new GameStartedEvent(gameContext));
            }
            catch (Exception e)
            {
                _logger.Error(LogTag.GameManager, $"Exception: {e}");
            }
        }
        
        public async void LoadGame(Guid saveSlotID)
        {
            try
            {
                var slot = _saveManager.GetSlot(saveSlotID);
                if (slot == null)
                    return;
                
                var (isSuccess, gameContext) = await StartGameWithSaveSlot(slot);
                if (!isSuccess)
                    return;
                
                _gameContext = gameContext;
                _eventBus.Publish(new GameStartedEvent(gameContext));
            }
            catch (Exception e)
            {
                _logger.Error(LogTag.GameManager, $"Exception: {e}");
            }
        }

        private async Task<(bool, GameContext)> StartGameWithSaveSlot(SaveSlotData saveSlotData)
        {
            try
            {
                var levelComponents = await _levelManager.LoadLevel(saveSlotData.LevelID);
                if (!levelComponents.IsLoaded) 
                    return (false, null);
                
                _logger.Success(LogTag.GameManager, $"Level {levelComponents.Data.DisplayName} loaded successfully.");
                
                _inputManager.SetState(InputStateType.Gameplay);
                var playerComponents = _playerController.SpawnPlayer(saveSlotData.SpawnPosition, saveSlotData.SpawnRotation);
                
                var gameContext = new GameContext
                {
                    SaveSlot = saveSlotData,
                    Level = levelComponents,
                    Player = playerComponents
                };
                
                return (true, gameContext);
            }
            catch (Exception e)
            {
                _logger.Error(LogTag.GameManager, $"Exception: {e}");
                return (false, null);
            }
        }
    }
}