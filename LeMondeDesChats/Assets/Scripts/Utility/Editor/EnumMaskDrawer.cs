using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnumMask), true)]
public class EnumMaskDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var enumType = property.FindPropertyRelative("type");
        var mask = property.FindPropertyRelative("mask");
        mask.intValue = EditorGUI.MaskField(position, label, mask.intValue, enumType.enumDisplayNames);
    }
}
