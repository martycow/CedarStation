#if UNITY_EDITOR
using Game.General;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Gameplay
{
    /// <summary>
    /// Lets you press Play in any scene and still boot through Application.
    /// If the start scene is a level has LevelSceneRoot, the game starts directly in that level
    /// </summary>
    public static class DevLaunch
    {
        public const string EnabledPrefsKey = "Cedar.PlayFromScene.Enabled";

        private const string ScenePathKey = "Cedar.DevLaunch.ScenePath";
        private const string SpawnKey = "Cedar.DevLaunch.Spawn";

        public static bool Enabled
        {
            get => EditorPrefs.GetBool(EnabledPrefsKey, true);
            set => EditorPrefs.SetBool(EnabledPrefsKey, value);
        }

        public static void SetSpawnOverride(Vector3? spawn)
        {
            SessionState.SetString(SpawnKey, spawn.HasValue ? JsonUtility.ToJson(spawn.Value) : string.Empty);
        }

        public static bool TryRead(out string scenePath, out Vector3? spawn)
        {
            scenePath = SessionState.GetString(ScenePathKey, string.Empty);
            var spawnJson = SessionState.GetString(SpawnKey, string.Empty);
            spawn = string.IsNullOrEmpty(spawnJson) ? null : JsonUtility.FromJson<Vector3>(spawnJson);
            return !string.IsNullOrEmpty(scenePath);
        }

        public static void Clear()
        {
            SessionState.EraseString(ScenePathKey);
            SessionState.EraseString(SpawnKey);
        }

        // Runs after the start scene is loaded but before any Awake.
        // Works regardless of domain reloads/recompiles, unlike EditorSceneManager.playModeStartScene.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BootstrapFromStartScene()
        {
            SessionState.EraseString(ScenePathKey);

            if (!Enabled)
                return;

            var startScene = SceneManager.GetActiveScene();
            if (startScene.name == Const.Scope.ApplicationScope)
                return; // Normal boot

            var isLevel = false;
            foreach (var root in startScene.GetRootGameObjects())
            {
                if (root.GetComponent<LevelSceneRoot>() != null)
                    isLevel = true;

                // Don't let the start scene's objects wake up: it's replaced by Application right away
                root.SetActive(false);
            }

            if (isLevel)
                SessionState.SetString(ScenePathKey, startScene.path);

            SceneManager.LoadScene(Const.Scope.ApplicationScope, LoadSceneMode.Single);
        }
    }
}
#endif
