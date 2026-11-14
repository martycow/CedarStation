#if UNITY_EDITOR
using Game.General;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Gameplay
{
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

        public static void Write(string scenePath, Vector3? spawn)
        {
            SessionState.SetString(ScenePathKey, scenePath);
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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BootstrapFromStartScene()
        {
            var startScene = SceneManager.GetActiveScene();
            if (startScene.name == Const.Scope.ApplicationScope)
                return; // Normal boot

            foreach (var root in startScene.GetRootGameObjects())
                root.SetActive(false);

            SceneManager.LoadScene(Const.Scope.ApplicationScope, LoadSceneMode.Single);
        }
    }
}
#endif
