using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Tag))]
public class TagPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var tag = property.FindPropertyRelative("stringTag");
        tag.stringValue = EditorGUI.TagField(position, label, tag.stringValue);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;// + EditorGUIUtility.standardVerticalSpacing;
    }
}
