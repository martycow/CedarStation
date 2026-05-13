using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Core;
using Game.General;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Game.Gameplay
{
    public struct LevelComponents
    {
        public bool IsSuccess;
        public Scene LoadedScene;
        public Level Level;
        public LevelData Data;
        
        public LevelComponents(bool isSuccess, Scene loadedScene, Level level, LevelData data)
        {
            IsSuccess = isSuccess;
            LoadedScene = loadedScene;
            Level = level;
            Data = data;
        }

        public static LevelComponents Fail() => new(false, default, null, null);
        public static LevelComponents Success(Scene loadedScene, Level level, LevelData data) => new(true, loadedScene, level, data);
    }
    
    public sealed class LevelManager
    {
        private readonly LevelDataStorage _levelDataStorage;
        private readonly CedarLogger _logger;
        private readonly Dictionary<Guid, LevelComponents> _loadedLevels = new();
        
        private Scene _currentScene;
        private Level _currentLevel;
        private LevelData _currentLevelData;

        public LevelManager(LevelDataStorage levelDataStorage, CedarLogger logger)
        {
            _levelDataStorage = levelDataStorage;
            _logger = logger;
        }

        public async Task<LevelComponents> LoadLevel(Guid levelID)
        {
            // Search level's settings
            if (!_levelDataStorage.LevelDataById.TryGetValue(levelID, out var levelData))
            {
                _logger.Error(LogTag.Level, $"Level with ID {levelID} not found.");
                return LevelComponents.Fail();
            }

            // Check if it's duplicate invoke
            if (_currentScene.isLoaded && _currentLevelData != null && _currentLevelData.ID == levelID)
            {
                _logger.Warn(LogTag.Level, $"Level with ID {levelID} is already loaded.");
                return new LevelComponents(true, _currentScene, _currentLevel, _currentLevelData);
            }

            // Hiding current level if we have one 
            if (_currentLevelData != null)
                HideLevel(_currentLevelData.ID);
            
            _currentLevelData = null;
            _currentLevel = null;
            
            // Loading level
            if (_loadedLevels.TryGetValue(levelID, out var loadResult))
            {
                _logger.Info(LogTag.Level, $"Level with ID {levelID} is already loaded.");
                
                loadResult.Level.gameObject.SetActive(true);
                _currentScene = loadResult.LoadedScene;
                _currentLevel = loadResult.Level;
                _currentLevelData = loadResult.Data;
            }
            else
            {
                _logger.Info(LogTag.Level, $"Loading scene {levelData.SceneName}...");
                var scene = await Utilities.Scenes.LoadAsync(levelData.SceneName);
                _logger.Info(LogTag.Level, "Scene loaded. Searching for Level object...");
                
                Level levelRoot = null;
                
                foreach (var gameObject in scene.GetRootGameObjects())
                {
                    levelRoot = gameObject.GetComponent<Level>();
                    if (levelRoot != null)
                        break;
                }

                if (levelRoot == null)
                {
                    levelRoot = new GameObject("LevelRoot").AddComponent<Level>();
                    _logger.Info(LogTag.Level, $"Level object not found in scene {levelData.SceneName}. Created new one.");
                }
                
                // Binding data to level object
                var data = new LevelData(
                    levelData.ID, 
                    levelData.TechName, 
                    levelData.SubType);
                
                levelRoot.Setup(data, ContextViewUpdateType.OnSetup | ContextViewUpdateType.EveryFrame);

                _currentScene = scene;
                _currentLevel = levelRoot;
                _currentLevelData = levelData;
                _loadedLevels[levelID] = LevelComponents.Success(scene, levelRoot, levelData);
            }
            
            return new LevelComponents(true, _currentScene, _currentLevel, _currentLevelData);
        }

        private void HideLevel(Guid levelID)
        {
            if (_loadedLevels.TryGetValue(levelID, out var loaded))
            {
                _logger.Info(LogTag.Level, $"Hiding level with ID {levelID}.");
                loaded.Level.gameObject.SetActive(false);
            }
            
            //SceneManager.UnloadSceneAsync(_currentScene);
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