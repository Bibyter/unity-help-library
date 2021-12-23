using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Bibyter.Fsm2
{
    [CustomEditor(typeof(State))]
    public sealed class StateEditor : UnityEditor.Editor
    {
        System.Type _behaviourAddType;
        int _behaviourDeleteId = -1;

        public override void OnInspectorGUI()
        {
            var state = target as State;

            if (state.parentState == null)
            {
                // dont draw root node
                return;
            }

            serializedObject.Update();

            var behaviours = serializedObject.FindProperty("_behaviours");

            if (_behaviourAddType != null)
            {
                behaviours.arraySize++;
                var newProperty = behaviours.GetArrayElementAtIndex(behaviours.arraySize - 1);
                newProperty.managedReferenceValue = System.Activator.CreateInstance(_behaviourAddType);

                _behaviourAddType = null;
            }

            DrawName(state);

            for (int i = 0; i < behaviours.arraySize; i++)
            {
                var rect = EditorGUILayout.GetControlRect(false, 0f);

                if (GUI.Button(new Rect(rect.position.x + rect.width - EditorGUIUtility.singleLineHeight - 1f, rect.position.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight), "x"))
                {
                    _behaviourDeleteId = i;
                }

                var element = behaviours.GetArrayElementAtIndex(i);
                var name = element.type;
                name = name.Substring(17, name.Length - 18);
                EditorGUILayout.PropertyField(element, new GUIContent(name), true);

                rect = EditorGUILayout.GetControlRect(false, 1);
                EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f));
            }

            if (_behaviourDeleteId != -1)
            {
                behaviours.DeleteArrayElementAtIndex(_behaviourDeleteId);
                _behaviourDeleteId = -1;
            }

            EditorGUILayout.Space(25f);
            if (GUILayout.Button("Add Behaviour", GUILayout.Height(25f)))
            {
                var dropdown = new BehaviourChoosePopup(new AdvancedDropdownState());
                dropdown.Show(new Rect(Event.current.mousePosition.x - 150f, Event.current.mousePosition.y + 50f, 300f, EditorGUIUtility.singleLineHeight));
                dropdown.onChoose += (type) => 
                {
                    _behaviourAddType = type;
                };
            }

            serializedObject.ApplyModifiedProperties();
        }

        void DrawName(State state)
        {
            EditorGUI.BeginChangeCheck();
            var name = EditorGUILayout.DelayedTextField("Name", state.name);
            if (EditorGUI.EndChangeCheck())
            {
                state.name = name;
                EditorUtility.SetDirty(state);
                AssetDatabase.SaveAssetIfDirty(state);
            }

            EditorGUILayout.Space();
        }
    }

    public class BehaviourChoosePopup : AdvancedDropdown
    {
        public event System.Action<System.Type> onChoose;


        public BehaviourChoosePopup(AdvancedDropdownState state) : base(state)
        {
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Behaviours");

            for (int i = 0; i < BehaviourTypeList.Count; i++)
            {
                var type = BehaviourTypeList.Get(i);

                var item = new AdvancedTypePopupItem(type, type.Name);

                root.AddChild(item);
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            onChoose?.Invoke((item as AdvancedTypePopupItem).Type);
        }

        class AdvancedTypePopupItem : AdvancedDropdownItem
        {
            public System.Type Type { get; }

            public AdvancedTypePopupItem(System.Type type, string name) : base(name)
            {
                Type = type;
            }

        }
    }

    static class BehaviourTypeList
    {
        static List<System.Type> _list = new List<System.Type>();

        static BehaviourTypeList()
        {
            var sourceType = typeof(Bibyter.Fsm2.StateBehaviour);

            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (sourceType.IsAssignableFrom(type) && sourceType != type && !type.IsInterface)
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