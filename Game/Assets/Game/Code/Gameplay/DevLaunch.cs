#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Game.Gameplay
{
    public static class DevLaunch
    {
        private const string ScenePathKey = "Cedar.DevLaunch.ScenePath";
        private const string SpawnKey = "Cedar.DevLaunch.Spawn";

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
    }
}
#endif