#if UNITY_EDITOR
using Cedar.Core;
using Game.General;
using Game.Gameplay;
using Game.Input;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ApplicationScope))]
    public sealed class ApplicationScopeEditor : UnityEditor.Editor
    {
        private string  _filter          = string.Empty;
        private Vector2 _scrollPos;
        private bool    _showContainer   = true;
        private bool    _showInput       = true;
        private bool    _showEventBus    = true;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var scope = (ApplicationScope)target;

            EditorGUILayout.Space(10f);
            CedarEditorUtils.DrawSeparator();
            EditorGUILayout.Space(6f);

            if (!Application.isPlaying || scope.Container == null)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to inspect runtime systems.", MessageType.Info);
                return;
            }

            // ── Cedar Container ───────────────────────────────────────────────
            _showContainer = CedarEditorUtils.DrawSectionHeader(
                "Cedar Container  ·  Application", _showContainer, CedarEditorUtils.ColorSingleton);

            if (_showContainer)
            {
                EditorGUILayout.Space(4f);
                CedarEditorUtils.DrawStatusBar(scope.Container.RegisteredDependencies);
                EditorGUILayout.Space(4f);
                CedarEditorUtils.DrawFilterBar(ref _filter);
                EditorGUILayout.Space(4f);
                CedarEditorUtils.DrawDependencyList(_filter, scope.Container.RegisteredDependencies, ref _scrollPos);
            }

            EditorGUILayout.Space(6f);

            // ── Input Manager ─────────────────────────────────────────────────
            _showInput = CedarEditorUtils.DrawSectionHeader(
                "Input Manager", _showInput, CedarEditorUtils.ColorLive);

            if (_showInput)
            {
                var inputManager = scope.Container.Resolve<IInputManager>() as InputManager;
                InputManagerDrawer.Draw(inputManager);
            }

            EditorGUILayout.Space(6f);

            // ── Event Bus ─────────────────────────────────────────────────────
            _showEventBus = CedarEditorUtils.DrawSectionHeader(
                "Event Bus", _showEventBus, CedarEditorUtils.ColorWarning);

            if (_showEventBus)
            {
                var eventBus = scope.Container.Resolve<EventBus>();
                EventBusDrawer.Draw(eventBus);
            }

            EditorGUILayout.Space(4f);

            if (Application.isPlaying)
                Repaint();
        }
    }
}
#endif
