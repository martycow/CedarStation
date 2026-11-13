using Game.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    [InitializeOnLoad]
    public static class PlayModeBootstrapper
    {
        private const string ApplicationScenePath = "Assets/Game/Scenes/Application.unity";
        private const string DefaultStartupKey = "Cedar.PlayFromScene.DefaultStartup";
        private const string MenuPath = "Cedar Station/Always start from Application";
        
        static PlayModeBootstrapper()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.delayCall += Apply;
        }

        private static bool DefaultStartup
        {
            get => EditorPrefs.GetBool(DefaultStartupKey, true);
            set => EditorPrefs.SetBool(DefaultStartupKey, value);
        }

        [MenuItem(MenuPath)]
        private static void Toggle()
        {
            DefaultStartup = !DefaultStartup;
            Apply();
        }

        [MenuItem(MenuPath, true)]
        private static bool ToggleValidation()
        {
            Menu.SetChecked(MenuPath, DefaultStartup);
            return true;
        }
        
        private static void Apply()
        {
            EditorSceneManager.playModeStartScene = DefaultStartup
                ? AssetDatabase.LoadAssetAtPath<SceneAsset>(ApplicationScenePath)
                : null;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode)
                return;

            Apply();
            DevLaunch.Clear();

            if (!DefaultStartup)
                return;

            var scene = SceneManager.GetActiveScene();
            if (scene.path == ApplicationScenePath || !scene.path.Contains("/Levels/"))
                return;

            // The level is loaded from disk in Play Mode, so unsaved edits wouldn't be there
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorApplication.isPlaying = false;
                return;
            }

            DevLaunch.Write(scene.path, GetSceneViewSpawn());
        }

        private static Vector3? GetSceneViewSpawn()
        {
            return null;
        }
    }
}