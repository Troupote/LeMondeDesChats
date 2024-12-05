using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomPropertyDrawer(typeof(Sprite))]
public class SpriteDrawer : PropertyDrawer
{
    private static GUIStyle s_TempStyle = new GUIStyle();
    private static float imageSize = 64;

    //private bool property.isExpanded = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        OldDisplay(position, property, label);
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float propertyHeight = base.GetPropertyHeight(property, label);

        if (property.isExpanded && property.objectReferenceValue != null)
        {
            //propertyHeight += 70f;
            propertyHeight += EditorGUIUtility.standardVerticalSpacing * 2 + imageSize;
        }

        return propertyHeight;
    }

    private void OldDisplay(Rect position, SerializedProperty property, GUIContent label)
    {
        //Rect fieldRect = new Rect(position.x,
        //    position.y,
        //    position.width - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing,
        //    EditorGUIUtility.singleLineHeight);
        //Rect checkRect = new Rect(position.x + position.width - EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
        //    position.y,
        //    EditorGUIUtility.singleLineHeight,
        //    EditorGUIUtility.singleLineHeight);

        ////create object field for the sprite
        //EditorGUI.PropertyField(fieldRect, property, label);
        //EditorGUI.indentLevel = 0;
        //property.isExpanded = EditorGUI.Toggle(checkRect, property.isExpanded);
        //EditorGUI.indentLevel = ident;

        position.height = EditorGUIUtility.singleLineHeight;
        PropertyFieldWithCheck(position, property, label, property.isExpanded, out bool result, "Toggle");
        property.isExpanded = result;

        //property.objectReferenceValue = EditorGUI.ObjectField(spriteRect, label, property.objectReferenceValue, typeof(Sprite), false);

        //if this is not a repain or the property is null exit now
        if (!property.isExpanded || Event.current.type != EventType.Repaint || property.objectReferenceValue == null || Selection.count > 1)
            return;
        
        //draw a sprite
        Sprite sp = property.objectReferenceValue as Sprite;
        DrawSprite(position, sp);
    }

    private void NewDisplay(Rect position, SerializedProperty property, GUIContent label)
    {
        Object _icon = property.objectReferenceValue;
        _icon = EditorGUI.ObjectField(position, label, _icon, typeof(Sprite), false);
        property.objectReferenceValue = _icon;
    }

    private void DrawSprite(Rect rect, Sprite sprite)
    {
        rect.x = rect.width - imageSize + rect.x;
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        rect.width = imageSize;
        rect.height = imageSize;
        s_TempStyle.normal.background = sprite.texture;
        s_TempStyle.Draw(rect, GUIContent.none, false, false, false, false);
    }

    private bool CheckSelection()
    {
        if (Selection.count == 1)
        {
            return false;
        }
        else
        {
            Sprite spriteRef = Selection.gameObjects[0].GetComponent<Image>().sprite;
            for (int i = 1; i < Selection.count; i++)
            {
                if (Selection.gameObjects[i].TryGetComponent<Image>(out Image image) && image.sprite != spriteRef)
                    return true;
            }
            return false;
        }
    }

    public static bool PropertyFieldWithCheck(Rect position, SerializedProperty property, GUIContent label, bool checkIn, out bool checkOut, GUIStyle checkStyle, bool includeChildren = false, int space = 2)
    {
        int buttonSize = 18;

        Rect fieldRect = new Rect(position);
        fieldRect.width -= buttonSize + space;

        bool returnValue = EditorGUI.PropertyField(fieldRect, property, label, includeChildren);

        Rect toggleRect = new Rect(position.position, Vector2.one * buttonSize);
        toggleRect.x = position.width + position.x - buttonSize;

        int indentation = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        checkOut = EditorGUI.Toggle(toggleRect, checkIn, checkStyle);
        EditorGUI.indentLevel = indentation;

        return returnValue;
    }

    public static bool PropertyFieldWithCheck(Rect position, SerializedProperty property, GUIContent label, bool checkIn, out bool checkOut, bool includeChildren = false, int space = 2)
    {
        return PropertyFieldWithCheck(position, property, label, checkIn, out checkOut, "Toggle", includeChildren, space);
    }
}