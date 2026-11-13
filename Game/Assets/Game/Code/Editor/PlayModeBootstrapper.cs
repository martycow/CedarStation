using Game.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor
{
    /// <summary>
    /// Editor side of "Play from any scene". The actual boot redirect happens at runtime in <see cref="DevLaunch"/>.
    /// </summary>
    [InitializeOnLoad]
    public static class PlayModeBootstrapper
    {
        private const string MenuPath = "Cedar Station/Play From Current Scene";

        static PlayModeBootstrapper()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.delayCall += () => EditorSceneManager.playModeStartScene = null;
        }

        [MenuItem(MenuPath)]
        private static void Toggle()
        {
            DevLaunch.Enabled = !DevLaunch.Enabled;
        }

        [MenuItem(MenuPath, true)]
        private static bool ToggleValidation()
        {
            Menu.SetChecked(MenuPath, DevLaunch.Enabled);
            return true;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode)
                return;

            DevLaunch.Clear();

            if (!DevLaunch.Enabled)
                return;

            // The level is reloaded from disk in Play Mode, so unsaved edits wouldn't be there
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorApplication.isPlaying = false;
        }
    }
}
