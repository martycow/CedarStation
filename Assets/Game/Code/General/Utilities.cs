using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.General
{
    public static class Utilities
    {
        public static class Vectors
        {
            
        }

        public static class Scenes
        {
            public static Scene Load(
                string sceneName,
                LoadSceneMode loadMode = LoadSceneMode.Additive, 
                LocalPhysicsMode physicsMode = LocalPhysicsMode.Physics3D)
            {
                var gameScene = SceneManager.GetSceneByName(sceneName);
                if (gameScene.isLoaded)
                    return gameScene;
            
                var parameters = new LoadSceneParameters(loadMode, physicsMode);
                SceneManager.LoadScene(sceneName, parameters);
                
                gameScene = SceneManager.GetSceneByName(sceneName);
                return gameScene;
            }
            
            public static async Task<Scene> LoadAsync(
                string sceneName, 
                LoadSceneMode loadMode = LoadSceneMode.Additive, 
                LocalPhysicsMode physicsMode = LocalPhysicsMode.Physics3D)
            {
                var gameScene = SceneManager.GetSceneByName(sceneName);
                if (gameScene.isLoaded)
                    return gameScene;
            
                var parameters = new LoadSceneParameters(loadMode, physicsMode);
                var task = SceneManager.LoadSceneAsync(sceneName, parameters);
                
                while (task != null && !task.isDone)
                    await Task.Yield();
                
                gameScene = SceneManager.GetSceneByName(sceneName);
                return gameScene;
            }
        }

        public static class Colors
        {
            public static Color Darken(Color c, float factor = 0.75f)
            {
                return new Color(c.r * factor, c.g * factor, c.b * factor);
            }

            public static Color FromHex(string hex, Color fallbackColor = default)
            {
                return ColorUtility.TryParseHtmlString(hex, out var color) ? color : fallbackColor;
            }
        }

        public static class DebugTools
        {
            public static void DrawArrow(Vector3 from, Vector3 to, Color color)
            {
                Debug.DrawLine(from, to, color);
                var direction = (to - from).normalized;
                var right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
                var left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -150, 0) * Vector3.forward;
                Debug.DrawLine(to, to + right * 0.25f, color);
                Debug.DrawLine(to, to + left * 0.25f, color);
            }
        }
    }
}