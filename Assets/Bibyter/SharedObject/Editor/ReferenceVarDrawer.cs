using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SharedObjectNs.Editor
{
    [CustomPropertyDrawer(typeof(SharedObject.ReferenceVar))]
    public sealed class ReferenceVarDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);

            EditorGUI.BeginProperty(position, label, property);

            var nameProperty = property.FindPropertyRelative("name");
            var varProperty = property.FindPropertyRelative("var");

            if (string.IsNullOrEmpty(nameProperty.stringValue))
            {
                nameProperty.stringValue = "default-name";
            }

            var p = position;
            p.height = EditorGUIUtility.singleLineHeight;
            p.width = EditorGUIUtility.labelWidth;
            nameProperty.stringValue = EditorGUI2.RemaneField(p, Event.current, nameProperty.stringValue, controlID);

            p = position;
            p.height = EditorGUIUtility.singleLineHeight;
            p.x += EditorGUIUtility.labelWidth;
            EditorGUI.PropertyField(p, varProperty, GUIContent.none, false);

            EditorGUI.EndProperty();
        }
    }
}
