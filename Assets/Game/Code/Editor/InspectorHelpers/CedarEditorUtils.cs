#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Cedar.Core;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    internal static class CedarEditorUtils
    {
        internal static readonly Color ColorSingleton = new(0.75f, 0.5f, 1f);
        internal static readonly Color ColorTransient = new(0.35f, 0.60f, 0.90f);
        internal static readonly Color ColorScoped    = new(0.90f, 0.70f, 0.25f);
        internal static readonly Color ColorLive      = new(0.35f, 1f,    0.45f);
        internal static readonly Color ColorOffline   = new(1f,    0.45f, 0.45f);
        internal static readonly Color ColorWarning   = new(1f,    0.75f, 0.20f);
        internal static readonly Color ColorDevice    = new(0.35f, 0.80f, 1f);
        internal static readonly Color ItemBg         = new(0.18f, 0.18f, 0.18f);
        internal static readonly Color SectionBg      = new(0.13f, 0.13f, 0.13f);
        internal static readonly Color SeparatorColor = new(0.32f, 0.32f, 0.32f, 0.6f);

        private static GUIStyle _sectionHeaderStyle;
        private static GUIStyle _subtitleStyle;

        private static GUIStyle SectionHeaderStyle => _sectionHeaderStyle ??= new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = new Color(0.92f, 0.92f, 0.92f) }
        };

        internal static GUIStyle SubtitleStyle => _subtitleStyle ??= new GUIStyle(EditorStyles.miniLabel)
        {
            fontSize = 11,
            normal = { textColor = new Color(0.58f, 0.58f, 0.58f) }
        };

        internal static bool DrawSectionHeader(string title, bool expanded, Color accentColor = default)
        {
            if (accentColor.a == 0f)
                accentColor = new Color(0.5f, 0.5f, 0.5f);

            var headerRect = GUILayoutUtility.GetRect(0f, 26f, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(headerRect, SectionBg);
            EditorGUI.DrawRect(new Rect(headerRect.x, headerRect.y, 3f, headerRect.height), accentColor);

            var arrow = expanded ? "▼" : "▶";
            GUI.Label(
                new Rect(headerRect.x + 12f, headerRect.y, headerRect.width - 20f, headerRect.height),
                $"{arrow}  {title}",
                SectionHeaderStyle
            );

            return GUI.Button(headerRect, GUIContent.none, GUIStyle.none) ? !expanded : expanded;
        }

        internal static void DrawStatusBar(Dictionary<Type, IDependency> dependencies)
        {
            var total    = dependencies.Count;
            var resolved = 0;

            foreach (var dep in dependencies.Values)
                if (dep.Lifetime == DependencyLifetime.Singleton && dep.SingletonInstance != null)
                    resolved++;

            var pending = total - resolved;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(6f);
            DrawBadge($"Total  {total}",    new Color(0.65f, 0.65f, 0.65f));
            GUILayout.Space(8f);
            DrawBadge($"Live  {resolved}",  ColorLive);
            GUILayout.Space(8f);
            DrawBadge($"Pending  {pending}", pending > 0 ? ColorOffline : new Color(0.42f, 0.42f, 0.42f));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        internal static void DrawFilterBar(ref string filter)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(6f);
            EditorGUILayout.LabelField("Filter", GUILayout.Width(40f));
            filter = EditorGUILayout.TextField(filter);
            if (GUILayout.Button("x", GUILayout.Width(24f)))
                filter = string.Empty;
            EditorGUILayout.EndHorizontal();
        }

        internal static void DrawDependencyList(
            string filter,
            Dictionary<Type, IDependency> dependencies,
            ref Vector2 scrollPos)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(400f));
            var lowerFilter = filter.ToLowerInvariant();

            foreach (var dep in dependencies.Values)
            {
                var contract = dep.ContractType.Name;
                var impl     = dep.ImplementationType.Name;

                if (!string.IsNullOrEmpty(lowerFilter) &&
                    !contract.ToLowerInvariant().Contains(lowerFilter) &&
                    !impl.ToLowerInvariant().Contains(lowerFilter))
                    continue;

                DrawDependencyItem(dep, contract, impl);
                DrawSeparator();
                EditorGUILayout.Space(2f);
            }

            EditorGUILayout.EndScrollView();
        }

        private static void DrawDependencyItem(IDependency dep, string contract, string impl)
        {
            var lifetimeColor = dep.Lifetime switch
            {
                DependencyLifetime.Singleton => ColorSingleton,
                DependencyLifetime.Transient => ColorTransient,
                _                            => ColorScoped
            };

            var isSingleton   = dep.Lifetime == DependencyLifetime.Singleton;
            var isResolved    = isSingleton && dep.SingletonInstance != null;
            var hasTwoLines   = contract != impl;
            var stripHeight   = hasTwoLines ? 40f : 24f;

            var itemRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(itemRect, ItemBg);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(8f);

                var stripRect = GUILayoutUtility.GetRect(3f, stripHeight, GUILayout.Width(3f));
                EditorGUI.DrawRect(stripRect, lifetimeColor);

                GUILayout.Space(8f);

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField(contract, EditorStyles.boldLabel);
                    if (hasTwoLines)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(14f);
                        EditorGUILayout.LabelField(impl, SubtitleStyle);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                if (isSingleton)
                {
                    DrawBadge(isResolved ? "● Live" : "○ Pending", isResolved ? ColorLive : ColorOffline);
                    GUILayout.Space(8f);
                }

                DrawBadge(dep.Lifetime.ToString(), lifetimeColor);
                GUILayout.Space(8f);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        internal static void DrawRowItem(string label, string badge, Color stripColor, Color badgeColor)
        {
            var itemRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(itemRect, ItemBg);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(8f);
                var stripRect = GUILayoutUtility.GetRect(3f, 24f, GUILayout.Width(3f));
                EditorGUI.DrawRect(stripRect, stripColor);
                GUILayout.Space(8f);
                EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                DrawBadge(badge, badgeColor);
                GUILayout.Space(8f);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        internal static void DrawBadge(string text, Color color, bool bold = true)
        {
            var style = new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize   = 11,
                fontStyle  = bold ? FontStyle.Bold : FontStyle.Normal,
                normal     = { textColor = color }
            };
            GUILayout.Label(text, style, GUILayout.ExpandWidth(false));
        }

        internal static void DrawSeparator()
        {
            var rect = EditorGUILayout.GetControlRect(false, 1f);
            EditorGUI.DrawRect(rect, SeparatorColor);
        }
    }
}
#endif
