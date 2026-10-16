using Unity.Cinemachine;
using UnityEngine.SceneManagement;

namespace Game.Gameplay
{
    public struct LevelComponents
    {
        public bool IsLoaded;
        public Scene Scene;
        public Level Level;
        public LevelSceneRoot SceneRoot;
        public LevelData Data;

        public LevelComponents(bool isLoaded, Scene scene, Level level, LevelSceneRoot sceneRoot, LevelData data)
        {
            IsLoaded = isLoaded;
            Scene = scene;
            Level = level;
            SceneRoot = sceneRoot;
            Data = data;
        }

        public static LevelComponents Fail() => new(false, default, null, null, null);

        public static LevelComponents Success(Scene scene, Level level, LevelSceneRoot sceneRoot, LevelData data)
            => new(true, scene, level, sceneRoot, data);
    }
}