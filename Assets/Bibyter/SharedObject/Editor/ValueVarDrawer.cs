using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SharedObjectNs.Editor
{
    [CustomPropertyDrawer(typeof(SharedObject.ValueVar))]
    public class ValueVarDrawer : PropertyDrawer
    {
        Dropdown _dropdown;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);

            var nameProperty = property.FindPropertyRelative("name");
            var varProperty = property.FindPropertyRelative("var");

            EditorGUI.BeginProperty(position, label, property);

            if (_dropdown == null)
            {
                _dropdown = new Dropdown(new AdvancedDropdownState());
            }


            if (string.IsNullOrEmpty(nameProperty.stringValue))
            {
                nameProperty.stringValue = "default-name";
            }

            var p = position;
            p.height = EditorGUIUtility.singleLineHeight;
            p.width = EditorGUIUtility.labelWidth;
            nameProperty.stringValue = EditorGUI2.RemaneField(p, Event.current, nameProperty.stringValue, controlID);

            if (EditorGUI.DropdownButton(GetDropdownButtonPosition(position), GetTypeName(varProperty), FocusType.Keyboard))
            {
                _dropdown.property = varProperty;
                _dropdown.Show(GetDropdownButtonPosition(position));
            }

            EditorGUI.PropertyField(position, varProperty, GUIContent.none, true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var varProperty = property.FindPropertyRelative("var");
            return EditorGUI.GetPropertyHeight(varProperty, true);// - (EditorGUIUtility.singleLineHeight * 2f);
        }

        void OnDropdownChoose(System.Type type)
        {

        }

        GUIContent GetTypeName(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
            {
                return new GUIContent("null");
            }

            var text = property.type;
            text = property.type.Substring(17, text.Length - 18);
            return new GUIContent(text);
        }

        Rect GetDropdownButtonPosition(in Rect position)
        {
            Rect rect = new Rect(position);
            rect.width -= EditorGUIUtility.labelWidth;
            rect.x += EditorGUIUtility.labelWidth;
            rect.height = EditorGUIUtility.singleLineHeight;
            return rect;
        }

        public sealed class Dropdown : AdvancedDropdown
        {
            public SerializedProperty property { set; get; }

            public Dropdown(AdvancedDropdownState state) : base(state)
            { }

            protected override AdvancedDropdownItem BuildRoot()
            {
                var root = new AdvancedDropdownItem("Select Type");

                for (int i = 0; i < SharedVariableTypes.Count; i++)
                {
                    root.AddChild(new Item(SharedVariableTypes.Get(i).Name) { type = SharedVariableTypes.Get(i) });
                }

                return root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                var type = (item as Item).type;

                try
                {
                    property.managedReferenceValue = System.Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"FieldDrawer(Error Add Managed Reference: type={type.FullName} {ex})");
                }
                finally
                {
                }
            }

            class Item : AdvancedDropdownItem
            {
                public System.Type type;

                public Item(string name) : base(name)
                {
                }
            }
        }
    }

    static class EditorGUI2
    {
        static int _editControlId = 0;


        public static string RemaneField(Rect position, Event e, string name, int keyControlId)
        {
            bool isRenameState = keyControlId == _editControlId;

            if (isRenameState)
            {
                EditorGUI.BeginChangeCheck();

                var r = EditorGUI.DelayedTextField(position, name);

                if (EditorGUI.EndChangeCheck())
                {
                    name = r;
                    _editControlId = 0;
                }
                else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
                {
                    _editControlId = 0;
                    e.Use();
                }
            }
            else
            {
                EditorGUI.LabelField(position, name);
            }

            if (e.type == EventType.MouseDown)
            {
                if (position.Contains(e.mousePosition))
                {
                    if (e.clickCount == 2)
                    {
                        _editControlId = keyControlId;
                    }
                    else
                    {
                        _editControlId = 0;
                    }

                    e.Use();
                }
                else
                {
                    if (isRenameState)
                    {
                        _editControlId = 0;
                        e.Use();
                    }
                }
            }

            return name;
        }
    }

    static class SharedVariableTypes
    {
        static List<System.Type> _list = new List<System.Type>();

        static SharedVariableTypes()
        {
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (System.Attribute.IsDefined(type, typeof(SharedVariableAttribute)) && !type.IsInterface && !type.IsAbstract)
                    {
                        _list.Add(type);
                    }
                }
            }
        }

        public static int Count => _list.Count;

        public static System.Type Get(int i)
        {
            return _list[i];
        }
    }
}