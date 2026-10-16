#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using Game.General;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    internal static class EventBusDrawer
    {
        private static readonly FieldInfo HandlersField =
            typeof(EventBus).GetField("_handlers", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo WrappersField =
            typeof(EventBus).GetField("_wrappers", BindingFlags.NonPublic | BindingFlags.Instance);

        internal static void Draw(EventBus eventBus)
        {
            if (eventBus == null)
            {
                EditorGUILayout.HelpBox("EventBus is not resolved.", MessageType.Warning);
                return;
            }

            var handlers = HandlersField?.GetValue(eventBus) as IDictionary;
            var wrappers = WrappersField?.GetValue(eventBus) as IDictionary;

            if (handlers == null || wrappers == null)
            {
                EditorGUILayout.HelpBox("Could not access EventBus internals via reflection.", MessageType.Error);
                return;
            }

            EditorGUILayout.Space(4f);

            // Summary row
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(6f);
                CedarEditorUtils.DrawBadge($"Event Types  {handlers.Count}", new Color(0.65f, 0.65f, 0.65f));
                GUILayout.Space(8f);
                CedarEditorUtils.DrawBadge(
                    $"Subscribers  {wrappers.Count}",
                    wrappers.Count > 0 ? CedarEditorUtils.ColorLive : new Color(0.42f, 0.42f, 0.42f)
                );
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6f);
            CedarEditorUtils.DrawSeparator();
            EditorGUILayout.Space(4f);

            if (handlers.Count == 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10f);
                EditorGUILayout.LabelField("No active subscriptions.", CedarEditorUtils.SubtitleStyle);
                EditorGUILayout.EndHorizontal();
                return;
            }

            foreach (DictionaryEntry entry in handlers)
            {
                var eventType      = (Type)entry.Key;
                var handler        = (Delegate)entry.Value;
                var subscriberCount = handler?.GetInvocationList().Length ?? 0;

                DrawEventRow(eventType.Name, subscriberCount);
                CedarEditorUtils.DrawSeparator();
                EditorGUILayout.Space(2f);
            }
        }

        private static void DrawEventRow(string eventName, int subscriberCount)
        {
            var badge      = $"↳ {subscriberCount} {(subscriberCount == 1 ? "sub" : "subs")}";
            var badgeColor = subscriberCount > 0 ? CedarEditorUtils.ColorWarning : new Color(0.42f, 0.42f, 0.42f);

            CedarEditorUtils.DrawRowItem(eventName, badge, CedarEditorUtils.ColorWarning, badgeColor);
        }
    }
}
#endif
