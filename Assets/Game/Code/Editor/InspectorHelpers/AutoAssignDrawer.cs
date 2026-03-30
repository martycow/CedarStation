using System;
using System.Reflection;
using Game.General;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(AutoAssignAttribute))]
    public class AutoAssignDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 24f;
        private const float Spacing = 4f;
 
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
 
            var attr = (AutoAssignAttribute)attribute;
            var isFilled = property.objectReferenceValue != null;
 
            // Split rect: [label + field] [button]
            var fieldRect = new Rect(position.x, position.y, position.width - ButtonWidth - Spacing, position.height);
            var buttonRect = new Rect(position.xMax - ButtonWidth, position.y, ButtonWidth, position.height);
 
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(fieldRect, property, label);
 
            var icon = isFilled
                ? EditorGUIUtility.IconContent("d_Refresh", "|Re-search for component")
                : EditorGUIUtility.IconContent("d_Search Icon", "|Find component and assign");
 
            var prevColor = GUI.color;
            GUI.color = isFilled ? new Color(0.6f, 1f, 0.6f) : new Color(1f, 0.85f, 0.4f);
 
            if (GUI.Button(buttonRect, icon, EditorStyles.iconButton))
                TryAssign(property, attr);
 
            GUI.color = prevColor;
            EditorGUI.EndProperty();
        }
 
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property, label, true);
 
        private static void TryAssign(SerializedProperty property, AutoAssignAttribute attr)
        {
            var targetObject = property.serializedObject.targetObject as Component;
            if (targetObject == null)
            {
                Debug.LogWarning("[AutoAssign] Target is not a Component.");
                return;
            }
 
            var fieldType = GetFieldType(property);
            if (fieldType == null)
            {
                Debug.LogWarning($"[AutoAssign] Could not determine field type for '{property.name}'.");
                return;
            }
 
            var found = Search(targetObject, fieldType, attr.Scope);
            if (found == null)
            {
                Debug.LogWarning(
                    $"[AutoAssign] No component of type <b>{fieldType.Name}</b> found " +
                    $"on '{targetObject.gameObject.name}' (scope: {attr.Scope}).",
                    targetObject.gameObject);
                return;
            }
 
            Undo.RecordObject(property.serializedObject.targetObject, $"AutoAssign {fieldType.Name}");
            property.objectReferenceValue = found;
            property.serializedObject.ApplyModifiedProperties();
        }
 
        private static UnityEngine.Object Search(Component root, Type type, SearchScope scope)
        {
            return scope switch
            {
                SearchScope.Self => root.GetComponent(type),
                SearchScope.SelfThenChildren => root.GetComponent(type) ??
                                                root.GetComponentInChildren(type, includeInactive: true),
                SearchScope.SelfThenParents => root.GetComponent(type) ??
                                               root.GetComponentInParent(type, includeInactive: true),
                SearchScope.SelfThenChildrenThenParents => root.GetComponent(type) ??
                                                           root.GetComponentInChildren(type, includeInactive: true) ??
                                                           root.GetComponentInParent(type, includeInactive: true),
                _ => null
            };
        }
        
        private static Type GetFieldType(SerializedProperty property)
        {
            // Walk the field info chain (handles nested/array paths).
            var targetType = property.serializedObject.targetObject.GetType();
            var path = property.propertyPath.Replace(".Array.data[", "[");
            var parts = path.Split('.');

            var currentType = targetType;
 
            foreach (var part in parts)
            {
                FieldInfo fi;
                if (part.Contains("["))
                {
                    var fieldName = part.Substring(0, part.IndexOf('['));
                    fi = GetField(currentType, fieldName);
                    if (fi == null) return null;
                    currentType = fi.FieldType.IsArray
                        ? fi.FieldType.GetElementType()
                        : fi.FieldType.GetGenericArguments()[0];
                }
                else
                {
                    fi = GetField(currentType, part);
                    if (fi == null) return null;
                    currentType = fi.FieldType;
                }
            }
 
            return currentType;
        }
 
        private static FieldInfo GetField(Type type, string name)
        {
            const BindingFlags flags = BindingFlags.Instance |
                                       BindingFlags.Public |
                                       BindingFlags.NonPublic;
 
            while (type != null && type != typeof(object))
            {
                var fi = type.GetField(name, flags);
                if (fi != null) return fi;
                type = type.BaseType;
            }
            return null;
        }
    }
}