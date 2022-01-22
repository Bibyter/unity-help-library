using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SharedObjectNs.BaseVariables
{
    [System.Serializable, SharedVariable]
    public sealed class StringVariable
    {
        public string value;
    }

    [System.Serializable, SharedVariable]
    public sealed class IntVariable
    {
        public int value;
    }

    [System.Serializable, SharedVariable]
    public sealed class FloatVariable
    {
        public float value;
    }
}

namespace SharedObjectNs.BaseVariables
{
    #region
    [System.Serializable]
    public struct StringLocalVariable
    {
        [SerializeField] string _name;
        [SerializeField] string _localValue;
        StringVariable _variable;

        public void Awake(IInjector injector)
        {
            _variable = !string.IsNullOrEmpty(_name) ? injector.GetInternalLink<StringVariable>() : null;
        }

        public string value
        {
            get { return _variable != null ? _variable.value : _localValue; }
            set { if (_variable != null) { _variable.value = value; } else { _localValue = value; } }
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(StringLocalVariable))]
    public sealed class StringLocalVariableDrawer : Editor.LocalVariableDrawer { }
#endif
    #endregion

    #region
    [System.Serializable]
    public struct IntLocalVariable
    {
        [SerializeField] string _name;
        [SerializeField] int _localValue;
        IntVariable _variable;

        public void Awake(IInjector injector)
        {
            _variable = !string.IsNullOrEmpty(_name) ? injector.GetInternalLink<IntVariable>() : null;
        }

        public int value
        {
            get { return _variable != null ? _variable.value : _localValue; }
            set { if (_variable != null) { _variable.value = value; } else { _localValue = value; } }
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(IntLocalVariable))]
    public sealed class IntLocalVariableDrawer : Editor.LocalVariableDrawer { }
#endif
    #endregion

    #region
    [System.Serializable]
    public struct FloatLocalVariable
    {
        [SerializeField] string _name;
        [SerializeField] float _localValue;
        FloatVariable _variable;

        public void Awake(IInjector injector)
        {
            _variable = !string.IsNullOrEmpty(_name) ? injector.GetInternalLink<FloatVariable>() : null;
        }

        public float value
        {
            get { return _variable != null ? _variable.value : _localValue; }
            set { if (_variable != null) { _variable.value = value; } else { _localValue = value; } }
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FloatLocalVariable))]
    public sealed class FloatLocalVariableDrawer : Editor.LocalVariableDrawer { }
#endif
    #endregion
}

#if UNITY_EDITOR
namespace SharedObjectNs.BaseVariables.Editor
{
    public class LocalVariableDrawer : PropertyDrawer
    {
        static string[] _popupOptions = new string[] { "Local Value", "Shared Value Name" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var y = position.y;
            var x = position.x;

            var nameProperty = property.FindPropertyRelative("_name");
            var valueProperty = property.FindPropertyRelative("_localValue");

            property.isExpanded = EditorGUI.Foldout(new Rect(x, y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight), property.isExpanded, property.displayName, true);


            if (property.isExpanded)
            {

                int popupId;

                if (string.IsNullOrEmpty(nameProperty.stringValue))
                {
                    popupId = 0;
                }
                else
                {
                    popupId = 1;
                }

                EditorGUI.BeginChangeCheck();

                x += 16f;
                y += EditorGUIUtility.singleLineHeight;
                popupId = EditorGUI.Popup(new Rect(x, y, EditorGUIUtility.labelWidth - 16f, EditorGUIUtility.singleLineHeight), popupId, _popupOptions);

                if (EditorGUI.EndChangeCheck())
                {
                    if (popupId == 0)
                    {
                        nameProperty.stringValue = string.Empty;
                    }
                    else if (popupId == 1)
                    {
                        nameProperty.stringValue = "default-name";
                    }
                }


                if (popupId == 0)
                {
                    EditorGUI.PropertyField(new Rect(x + (EditorGUIUtility.labelWidth - 16f), y, (EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth) - 23f, EditorGUIUtility.singleLineHeight), valueProperty, GUIContent.none);
                }
                else
                {
                    nameProperty.stringValue = EditorGUI.DelayedTextField(new Rect(x + (EditorGUIUtility.labelWidth - 16f), y, (EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth) - 23f, EditorGUIUtility.singleLineHeight), nameProperty.stringValue);
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight * 2f;
            }

            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
