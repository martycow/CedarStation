using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Transform))]
[CanEditMultipleObjects]
public class PositionInspector :  Editor
{
   public override void OnInspectorGUI()
   {
      //DrawDefaultInspector();
      
      var t = (Transform)target;
      
      DrawResettableField(
         () => EditorGUILayout.Vector3Field("Local Position", t.localPosition),
         Vector3.zero,
         (newValue) =>
         {
            Undo.RecordObject(t, "Reset Local Position");
            t.localPosition = newValue;
         });
      
      DrawResettableField(
         () => EditorGUILayout.Vector3Field("World Position", t.position),
         Vector3.zero,
         (newValue) =>
         {
            Undo.RecordObject(t, "Reset World Position");
            t.position = newValue;
         });
      
      DrawResettableField(
         () => EditorGUILayout.Vector3Field("Local Rotation", t.localRotation.eulerAngles),
         Vector3.zero,
         (newValue) =>
         {
            Undo.RecordObject(t, "Reset Local Rotation");
            t.localRotation = Quaternion.Euler(newValue);
         });

      DrawResettableField(
         () => EditorGUILayout.Vector3Field("Local Scale", t.localScale),
         Vector3.one,
         (newValue) =>
         {
            Undo.RecordObject(t, "Reset Local Scale");
            t.localScale = newValue;
         });
   }
   
   private void DrawResettableField<T>(Func<T> drawField, T resetValue, Action<T> onChange)
   {
      EditorGUILayout.BeginHorizontal();
      
      if (GUILayout.Button("↺", GUILayout.Width(24), GUILayout.Height(18))) 
         onChange(resetValue);
      
      EditorGUI.BeginChangeCheck();
      var newValue = drawField();
      if (EditorGUI.EndChangeCheck()) 
         onChange(newValue);
      
      EditorGUILayout.EndHorizontal();
   }
}
