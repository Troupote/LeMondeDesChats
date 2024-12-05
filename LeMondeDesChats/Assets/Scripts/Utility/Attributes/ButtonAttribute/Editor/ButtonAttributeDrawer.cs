using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace WMG
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonAttributeDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            // First get the attribute since it contains the range for the slider
            ButtonAttribute button = attribute as ButtonAttribute;

            EditorGUI.PropertyField(position, property, label);

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (GUI.Button(position, new GUIContent(button.Content)))
            {
                if (string.IsNullOrEmpty(button.MethodName))
                {
                    Debug.Log("No method name.");
                    return;
                }

                (property.serializedObject.targetObject as MonoBehaviour).Invoke(button.MethodName, 0);
                Debug.Log($"Invoke {button.MethodName}()");
                InternalEditorUtility.RepaintAllViews();
            }

            //// Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
            //if (property.propertyType == SerializedPropertyType.Float)
            //    EditorGUI.Slider(position, property, range.min, range.max, label);
            //else if (property.propertyType == SerializedPropertyType.Integer)
            //    EditorGUI.IntSlider(position, property, Convert.ToInt32(range.min), Convert.ToInt32(range.max), label);
            //else
            //    EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
        }
    }
}
