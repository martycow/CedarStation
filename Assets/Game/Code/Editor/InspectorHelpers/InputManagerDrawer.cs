#if UNITY_EDITOR
using Cedar.Core;
using Game.General;
using Game.Input;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    internal static class InputManagerDrawer
    {
        internal static void Draw(InputManager inputManager)
        {
            if (inputManager == null)
            {
                EditorGUILayout.HelpBox("InputManager is not resolved.", MessageType.Warning);
                return;
            }

            EditorGUILayout.Space(4f);

            // Current State & Device
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(6f);
                CedarEditorUtils.DrawBadge("State:", new Color(0.65f, 0.65f, 0.65f), false);
                GUILayout.Space(4f);
                CedarEditorUtils.DrawBadge(inputManager.CurrentStateType.ToString(), CedarEditorUtils.ColorLive);
                GUILayout.Space(16f);
                CedarEditorUtils.DrawBadge("Device:", new Color(0.65f, 0.65f, 0.65f), false);
                GUILayout.Space(4f);
                CedarEditorUtils.DrawBadge(inputManager.CurrentDevice.ToString(), CedarEditorUtils.ColorDevice);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6f);
            CedarEditorUtils.DrawSeparator();
            EditorGUILayout.Space(4f);

            // States list
            foreach (var (stateType, _) in inputManager.States)
            {
                var isActive = inputManager.CurrentStateType == stateType;
                DrawStateRow(stateType, isActive);
                CedarEditorUtils.DrawSeparator();
                EditorGUILayout.Space(2f);
            }
        }

        private static void DrawStateRow(InputStateType stateType, bool isActive)
        {
            var stripColor = isActive ? CedarEditorUtils.ColorLive : new Color(0.38f, 0.38f, 0.38f);
            var badge      = isActive ? "● Active" : "○ Inactive";

            CedarEditorUtils.DrawRowItem(stateType.ToString(), badge, stripColor, stripColor);
        }
    }
}
#endif
