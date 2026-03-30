#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Cedar.Core;
using Game.Gameplay;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ApplicationScope))]
    public sealed class ApplicationScopeEditor : UnityEditor.Editor
    {
        private static readonly Color ColorSingleton = new(0.75f, 0.5f, 1f, 1f);
        private static readonly Color ColorTransient = new(0.35f, 0.60f, 0.90f, 1f);
        private static readonly Color ColorScoped = new(0.90f, 0.70f, 0.25f, 1f);
        private static readonly Color ColorLive = new(0.35f, 1f, 0.45f, 1f);
        private static readonly Color ColorOffline = new(1f, 0.45f, 0.45f, 1f);
        private static readonly Color ItemBgColor = new(0.18f, 0.18f, 0.18f, 1f);

        private static GUIStyle SubtitleStyle => new(EditorStyles.miniLabel)
        {
            fontSize = 12,
            normal =
            {
                textColor = new Color(0.75f, 0.75f, 0.75f, 1f)
            }
        };
        
        private string _filter = string.Empty;
        private Vector2 _scrollPos;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var scope = (ApplicationScope)target;

            // Title
            EditorGUILayout.Space(10);
            DrawSeparator();
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Cedar Container", EditorStyles.boldLabel);
            
            // Info Message
            if (!Application.isPlaying || scope.RootContainer == null)
            {
                EditorGUILayout.HelpBox("Container is not initialized. Enter Play Mode to inspect dependencies.", MessageType.Info);
                return;
            }

            // Status Bar
            DrawStatusBar(scope.RootContainer.RegisteredDependencies);
            
            EditorGUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            
            // Filter Bar
            DrawFilterBar(ref _filter);
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(4);
            
            // Dependency List
            DrawDependencyList(_filter, scope.RootContainer.RegisteredDependencies, ref _scrollPos);
            
            if (Application.isPlaying)
                Repaint();
        }

        private static void DrawStatusBar(Dictionary<Type, IDependency> dependencies)
        {
            var total = dependencies.Count;
            var resolved = 0;

            foreach (var dep in dependencies.Values)
            {
                if (dep.Lifetime == DependencyLifetime.Singleton && dep.SingletonInstance != null)
                    resolved++;
            }

            EditorGUILayout.BeginHorizontal();

            DrawBadge($"Total: {total}", new Color(0.6f, 0.6f, 0.6f, 1f));
            DrawBadge($"Resolved: {resolved}", ColorLive);
            DrawBadge($"Pending: {total - resolved}", ColorOffline);

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawFilterBar(ref string filterString)
        {
            EditorGUILayout.LabelField("Filter", GUILayout.Width(40));
            filterString = EditorGUILayout.TextField(filterString);
            
            if (GUILayout.Button("x", GUILayout.Width(24)))
                filterString = string.Empty;
        }
        
        private static void DrawDependencyList(string filterString, Dictionary<Type, IDependency> dependencies, ref Vector2 scrollPos)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(600));
            var filter = filterString.ToLowerInvariant();

            foreach (var dep in dependencies.Values)
            {
                var contractName = dep.ContractType.Name;
                var implementationName = dep.ImplementationType.Name;

                if (!string.IsNullOrEmpty(filter) &&
                    !contractName.ToLowerInvariant().Contains(filter) &&
                    !implementationName.ToLowerInvariant().Contains(filter))
                    continue;

                DrawDependencyListItem(dep, contractName, implementationName);
                DrawSeparator();
                EditorGUILayout.Space(2);
            }
            EditorGUILayout.EndScrollView();
        }

        private static void DrawDependencyListItem(IDependency dependency, string contractName, string implementationName)
        {
            var lifetimeColor = dependency.Lifetime switch
            {
                DependencyLifetime.Singleton => ColorSingleton,
                DependencyLifetime.Transient => ColorTransient,
                _ => ColorScoped
            };

            var isSingleton = dependency.Lifetime == DependencyLifetime.Singleton;
            var isResolved = isSingleton && dependency.SingletonInstance != null;
            
            var itemRect = EditorGUILayout.BeginVertical();
            itemRect.height = 40;
            {
                EditorGUI.DrawRect(itemRect, ItemBgColor);
                
                // Drawing Content
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(10);

                    // Drawing lifetime strip
                    var stripRect = GUILayoutUtility.GetRect(4, 40, GUILayout.Width(4));
                    EditorGUI.DrawRect(stripRect, lifetimeColor);
                    
                    GUILayout.Space(10);

                    // Drawing dependency names
                    EditorGUILayout.BeginVertical();
                    {
                        if (contractName == implementationName)
                        {
                            EditorGUILayout.LabelField(contractName, EditorStyles.boldLabel);
                        }
                        else
                        {
                            EditorGUILayout.LabelField(contractName, EditorStyles.boldLabel);

                            // Drawing implementation name as subtitle
                            EditorGUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(15);
                                EditorGUILayout.LabelField(implementationName, SubtitleStyle);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.EndVertical();
                    
                    // Just empty space
                    GUILayout.FlexibleSpace();

                    // Drawing resolve status label
                    if (isSingleton)
                    {
                        var statusText = isResolved ? "● Live" : "○ Offline";
                        var statusColor = isResolved ? ColorLive : ColorOffline;
                        DrawBadge(statusText, statusColor);
                    }
                    
                    GUILayout.Space(10);

                    // Drawing lifetime label
                    DrawBadge(dependency.Lifetime.ToString(), lifetimeColor);
                    
                    GUILayout.Space(10);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawBadge(string text, Color color)
        {
            var style = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = color },
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };
            GUILayout.Label(text, style, GUILayout.ExpandWidth(false));
        }

        private static void DrawSeparator()
        {
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.35f, 0.35f, 0.35f, 0.5f));
        }
    }
}
#endif