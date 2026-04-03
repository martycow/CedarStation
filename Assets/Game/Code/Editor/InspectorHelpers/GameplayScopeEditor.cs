#if UNITY_EDITOR
using Cedar.Core;
using Game.Gameplay;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GameplayScope))]
    public sealed class GameplayScopeEditor : UnityEditor.Editor
    {
        private string  _filter        = string.Empty;
        private Vector2 _scrollPos;
        private bool    _showContainer = true;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var scope = (GameplayScope)target;

            EditorGUILayout.Space(10f);
            CedarEditorUtils.DrawSeparator();
            EditorGUILayout.Space(6f);

            if (!Application.isPlaying || scope.Container == null)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to inspect runtime systems.", MessageType.Info);
                return;
            }
            
            var appScope = FindAnyObjectByType<ApplicationScope>();
            if (appScope?.Container != null)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(6f);
                    CedarEditorUtils.DrawBadge("Parent:", new Color(0.58f, 0.58f, 0.58f), false);
                    GUILayout.Space(4f);
                    CedarEditorUtils.DrawBadge("ApplicationScope", CedarEditorUtils.ColorSingleton);
                    GUILayout.Space(8f);
                    CedarEditorUtils.DrawBadge(
                        $"({appScope.Container.RegisteredDependencies.Count} deps inherited)",
                        new Color(0.48f, 0.48f, 0.48f),
                        false
                    );
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(6f);
            }
            
            _showContainer = CedarEditorUtils.DrawSectionHeader(
                "Cedar Container  ·  Gameplay", _showContainer, CedarEditorUtils.ColorTransient);

            if (_showContainer)
            {
                EditorGUILayout.Space(4f);
                CedarEditorUtils.DrawStatusBar(scope.Container.RegisteredDependencies);
                EditorGUILayout.Space(4f);
                CedarEditorUtils.DrawFilterBar(ref _filter);
                EditorGUILayout.Space(4f);
                CedarEditorUtils.DrawDependencyList(_filter, scope.Container.RegisteredDependencies, ref _scrollPos);
            }

            EditorGUILayout.Space(4f);

            if (Application.isPlaying)
                Repaint();
        }
    }
}
#endif
