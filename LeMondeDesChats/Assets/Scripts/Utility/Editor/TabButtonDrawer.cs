using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace WMG.Utilities
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TabButton))]
    public class TabButtonDrawer : SelectableEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tabGroup"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SelectedImage"));
            serializedObject.ApplyModifiedProperties();
        }
    } 
}
