using System;
using System.Collections.Generic;
using Cedar.Core;
using Game.General;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Gameplay
{
    public sealed class LevelManager
    {
        private readonly PlayerSpawner _playerSpawner;
        private readonly LevelDataStorage _levelDataStorage;
        private readonly ICedarLogger _logger;
        private readonly Dictionary<Guid, (Level level, LevelData levelData)> _loadedLevels = new();
        private readonly GameObject _levelsRoot;
        
        private Level _currentLevel;
        private LevelData _currentLevelData;

        public LevelManager(PlayerSpawner playerSpawner, LevelDataStorage levelDataStorage, ICedarLogger logger)
        {
            _playerSpawner = playerSpawner;
            _levelDataStorage = levelDataStorage;
            _logger = logger;
            
            _levelsRoot = new GameObject("Levels");
        }

        public (Level level, LevelData levelData) LoadLevel(Guid levelID)
        {
            // Search level's settings
            if (!_levelDataStorage.LevelDataById.TryGetValue(levelID, out var levelData))
            {
                _logger.Error(SystemTag.Level, $"Level with ID {levelID} not found.");
                return (null, null);
            }

            // Check if it's duplicate invoke
            if (_currentLevelData != null && _currentLevelData.ID == levelID)
            {
                _logger.Warn(SystemTag.Level, $"Level with ID {levelID} is already loaded.");
                return (_currentLevel, _currentLevelData);
            }

            // Hiding current level if we have one 
            if (_currentLevelData != null)
                HideLevel(_currentLevelData.ID);
            _currentLevelData = null;
            _currentLevel = null;
            
            // Loading level
            if (_loadedLevels.TryGetValue(levelID, out var loadedLevel))
            {
                _logger.Info(SystemTag.Level, $"Level with ID {levelID} is already loaded.");
                
                loadedLevel.level.gameObject.SetActive(true);
                _currentLevel = loadedLevel.level;
                _currentLevelData = loadedLevel.levelData;
            }
            else
            {
                _logger.Info(SystemTag.Level, $"Creating new level instance for Level ID: {levelID}.");
                
                var prefab = Resources.Load<Level>($"Prefabs/Levels/{levelData.PrefabName}");
                if (prefab == null)
                {
                    _logger.Error(SystemTag.Level, $"Level prefab with name {levelData.PrefabName} not found in Resources/Prefabs/Levels.");
                    return (null, null);
                }

                var levelInstance = Object.Instantiate(prefab, _levelsRoot.transform);
                
                var data = new LevelData(
                    levelData.ID, 
                    levelData.TechName, 
                    levelData.SubType, 
                    _logger);
                
                levelInstance.Setup(data, ViewUpdateType.OnSetup | ViewUpdateType.EveryFrame);
                
                _currentLevel = levelInstance;
                _currentLevelData = levelData;
                _loadedLevels[levelID] = (levelInstance, levelData);
            }
            
            return (_currentLevel, _currentLevelData);
        }

        private void HideLevel(Guid levelID)
        {
            if (_loadedLevels.TryGetValue(levelID, out var loaded))
            {
                _logger.Info(SystemTag.Level, $"Hiding level with ID {levelID}.");
                loaded.level.gameObject.SetActive(false);
            }
        }

        private void Clear()
        {
            if (_currentLevel == null || _currentLevelData == null)
                return;
            
            Object.Destroy(_currentLevel.gameObject);
            _currentLevel = null;
            _currentLevelData = null;
            _loadedLevels.Clear();
        }
    }
}