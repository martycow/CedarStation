#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Game.General;

namespace Editor
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var a = property.FindPropertyRelative("a");
            var b = property.FindPropertyRelative("b");
            var c = property.FindPropertyRelative("c");
            var d = property.FindPropertyRelative("d");

            var guid = ToGuid(a.uintValue, b.uintValue, c.uintValue, d.uintValue);

            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            var fieldRect = new Rect(labelRect.xMax, position.y, position.width - labelRect.width - 60, position.height);
            var buttonRect = new Rect(fieldRect.xMax + 4, position.y, 56, position.height);

            EditorGUI.LabelField(labelRect, label);
            EditorGUI.SelectableLabel(fieldRect, guid.ToString(), EditorStyles.textField);

            if (GUI.Button(buttonRect, "New"))
            {
                var bytes = Guid.NewGuid().ToByteArray();
                a.uintValue = BitConverter.ToUInt32(bytes, 0);
                b.uintValue = BitConverter.ToUInt32(bytes, 4);
                c.uintValue = BitConverter.ToUInt32(bytes, 8);
                d.uintValue = BitConverter.ToUInt32(bytes, 12);
            }

            EditorGUI.EndProperty();
        }

        private static Guid ToGuid(uint a, uint b, uint c, uint d)
        {
            var bytes = new byte[16];
            BitConverter.GetBytes(a).CopyTo(bytes, 0);
            BitConverter.GetBytes(b).CopyTo(bytes, 4);
            BitConverter.GetBytes(c).CopyTo(bytes, 8);
            BitConverter.GetBytes(d).CopyTo(bytes, 12);
            return new Guid(bytes);
        }
    }
}
#endif