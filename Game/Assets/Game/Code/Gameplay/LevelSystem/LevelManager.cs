using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.General;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Game.Gameplay
{
    public sealed class LevelManager
    {
        private readonly LevelDataStorage _levelDataStorage;
        private readonly CedarLogger _logger;
        private readonly Dictionary<Guid, LevelComponents> _loadedLevels = new();

        private Scene _currentScene;
        private Level _currentLevel;
        private LevelSceneRoot _currentSceneRoot;
        private LevelData _currentLevelData;

        public LevelManager(LevelDataStorage levelDataStorage, CedarLogger logger)
        {
            _levelDataStorage = levelDataStorage;
            _logger = logger;
        }

        public async Task<LevelComponents> LoadLevel(Guid levelID)
        {
            if (!_levelDataStorage.LevelDataById.TryGetValue(levelID, out var levelData))
            {
                _logger.Error(LogTag.Level, $"Level with ID {levelID} not found.");
                return LevelComponents.Fail();
            }

            if (_currentScene.isLoaded && _currentLevelData != null && _currentLevelData.ID == levelID)
            {
                _logger.Warn(LogTag.Level, $"Level with ID {levelID} is already loaded.");
                return new LevelComponents(true, _currentScene, _currentLevel, _currentSceneRoot, _currentLevelData);
            }

            if (_currentLevelData != null)
                HideLevel(_currentLevelData.ID);

            _currentLevelData = null;
            _currentLevel = null;
            _currentSceneRoot = null;

            if (_loadedLevels.TryGetValue(levelID, out var cached))
            {
                _logger.Info(LogTag.Level, $"Restoring cached level {levelID}.");

                cached.SceneRoot.gameObject.SetActive(true);
                _currentScene = cached.Scene;
                _currentLevel = cached.Level;
                _currentSceneRoot = cached.SceneRoot;
                _currentLevelData = cached.Data;
            }
            else
            {
                _logger.Info(LogTag.Level, $"Loading scene {levelData.SceneName}...");
                var scene = await Utilities.Scenes.LoadAsync(levelData.SceneName);
                _logger.Info(LogTag.Level, "Scene loaded. Searching for LevelSceneRoot...");

                LevelSceneRoot sceneRoot = null;
                foreach (var go in scene.GetRootGameObjects())
                {
                    sceneRoot = go.GetComponent<LevelSceneRoot>();
                    if (sceneRoot != null)
                        break;
                }

                if (sceneRoot == null)
                {
                    _logger.Error(LogTag.Level, $"LevelSceneRoot not found in scene {levelData.SceneName}. Add the component to a root object.");
                    return LevelComponents.Fail();
                }

                var levelGo = new GameObject("Level");
                SceneManager.MoveGameObjectToScene(levelGo, scene);
                var level = levelGo.AddComponent<Level>();

                var data = new LevelData(levelData.ID, levelData.TechName, levelData.SubType);
                level.Setup(data, ContextViewUpdateType.OnSetup | ContextViewUpdateType.EveryFrame);

                _currentScene = scene;
                _currentLevel = level;
                _currentSceneRoot = sceneRoot;
                _currentLevelData = levelData;
                _loadedLevels[levelID] = LevelComponents.Success(scene, level, sceneRoot, levelData);
            }

            return new LevelComponents(true, _currentScene, _currentLevel, _currentSceneRoot, _currentLevelData);
        }

        private void HideLevel(Guid levelID)
        {
            if (_loadedLevels.TryGetValue(levelID, out var loaded))
            {
                _logger.Info(LogTag.Level, $"Hiding level {levelID}.");
                loaded.SceneRoot.gameObject.SetActive(false);
            }
        }

        private void Clear()
        {
            if (_currentLevel == null || _currentLevelData == null)
                return;

            Object.Destroy(_currentLevel.gameObject);
            _currentLevel = null;
            _currentSceneRoot = null;
            _currentLevelData = null;
            _loadedLevels.Clear();
        }
    }
}
