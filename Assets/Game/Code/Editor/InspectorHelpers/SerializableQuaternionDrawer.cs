#if UNITY_EDITOR
using Game.General;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(SerializableQuaternion))]
    public class SerializableQuaternionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var x = property.FindPropertyRelative("x");
            var y = property.FindPropertyRelative("y");
            var z = property.FindPropertyRelative("z");
            var w = property.FindPropertyRelative("w");
            
            //Display quaternion on user-friendly format
            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            var fieldRect = new Rect(labelRect.xMax, position.y, position.width - labelRect.width, position.height);
            EditorGUI.LabelField(labelRect, label);
            EditorGUI.SelectableLabel(fieldRect, $"({x.floatValue}, {y.floatValue}, {z.floatValue}, {w.floatValue})", EditorStyles.textField);
            
        }
    }
}
#endif