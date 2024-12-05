using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ExposeComponentAttribute))]
public class ExposeComponentAttributeDrawer : PropertyDrawer
{
    private Editor editor = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Draw label
        EditorGUI.PropertyField(position, property, label, true);
        
        if (property.objectReferenceValue != null
            && (property.objectReferenceValue.GetType() == typeof(GameObject)
            || property.objectReferenceValue.GetType().IsSubclassOf(typeof(Component))))
        {
            DrawInternalParameters(position, property);
        }
        else
        {
            Debug.LogWarning($"Can't expose a component on a field of type : {property.type}. Field must be a GameObject or a Component");
            return;
        }
    }

    private void DrawInternalParameters(Rect position, SerializedProperty property)
    {
        var exposeComponent = attribute as ExposeComponentAttribute;
        Component component;
        if (property.type == "GameObject")
        {
            GameObject gameObject = property.objectReferenceValue as GameObject;
            if (!gameObject.TryGetComponent(exposeComponent.Type, out component))
                return;
        }
        else
        {
            component = property.objectReferenceValue as Component;
        }

        // Draw foldout arrow
        if (property.objectReferenceValue != null)
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);

        // Draw foldout properties
        if (property.isExpanded)
        {
            // Make child fields be indented
            EditorGUI.indentLevel++;

            // Draw object properties
            if (!editor)
                Editor.CreateCachedEditor(component, null, ref editor);
            editor.OnInspectorGUI();

            // Set indent back to what it is
            EditorGUI.indentLevel--;
        }
    }
}
