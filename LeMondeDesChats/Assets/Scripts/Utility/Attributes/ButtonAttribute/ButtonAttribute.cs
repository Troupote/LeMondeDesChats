using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WMG
{
    public class ButtonAttribute : PropertyAttribute
    {
        public string Content;
        public string MethodName;

        public ButtonAttribute(string content, string methodName)
        {
            Content = content;
            MethodName = methodName;
        }
    }
}
