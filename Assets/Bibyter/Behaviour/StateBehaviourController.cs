using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
#endif

namespace Bibyter
{
    public sealed class Example1Behaviour : Behaviour
    {
        public string name = "example 1";
    }

    public sealed class Example2Behaviour : Behaviour
    {
        public string name = "example 2";
    }

    [System.Serializable]
    public sealed partial class StateBehaviourController
    {
        [SerializeField, SerializeReference] Behaviour[] _behaviours;
        [SerializeField] int[] _masks;

        int _currentState = -1;
        public int currentState
        {
            set
            {
                for (int i = 0; i < _behaviours.Length; i++)
                {
                    if (FlagsHelper.IsSet(_masks[i], FlagsHelper.GetFlag(value)))
                    {
                        _behaviours[i].enabled = true;
                    }
                    else
                    {
                        _behaviours[i].enabled = false;
                    }
                }

                _currentState = value;
            }

            get => _currentState;
        }


        public void Awake(IInjector injector)
        {
            for (int i = 0; i < _behaviours.Length; i++)
            {
                _behaviours[i].Awake(injector);
            }
        }

        public void Update()
        {
            for (int i = 0; i < _behaviours.Length; i++)
            {
                _behaviours[i].Run();
            }
        }

        public T GetBehaviour<T>() where T : Behaviour
        {
            for (int i = 0; i < _behaviours.Length; i++)
            {
                if (_behaviours[i] is T)
                    return _behaviours[i] as T;
            }

            return null;
        }


#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(StateBehaviourController))]
        public sealed class Drawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                Styles_Gui();
                Database_Gui();
                StateInfo_Gui(property);
                SetStartHeight(position.y);

                var stepY = EditorGUIUtility.singleLineHeight;
                var stepX = 20f;
                float y = position.y;
                float x = position.x;

                var controller = fieldInfo.GetValue(property.serializedObject.targetObject) as StateBehaviourController;
                var behaviours = property.FindPropertyRelative("_behaviours");
                var masks = property.FindPropertyRelative("_masks");

                if (masks.arraySize != behaviours.arraySize)
                {
                    masks.arraySize = behaviours.arraySize;
                }

                property.isExpanded = EditorGUI.Foldout(new Rect(x, y, position.width, stepY), property.isExpanded, property.displayName, true);
                x += stepX;

                if (property.isExpanded)
                {
                    y += stepY;
                    stateInfo_BehavioursFoldout = EditorGUI.Foldout(new Rect(x, y, position.width, stepY), stateInfo_BehavioursFoldout, "Behaviours", true, EditorStyles.foldoutHeader);

                    if (stateInfo_BehavioursFoldout)
                    {
                        x += stepX;

                        for (int i = 0; i < behaviours.arraySize; i++)
                        {
                            var behaviourType = controller._behaviours[i].GetType();
                            var behaviourProperty = behaviours.GetArrayElementAtIndex(i);
                            var maskProperty = masks.GetArrayElementAtIndex(i);

                            y += stepY;
                            var behaviourContent = new GUIContent($"{behaviourType.Name} ({i})");
                            behaviourContent = EditorGUI.BeginProperty(new Rect(x, y, position.width - 100f, stepY), behaviourContent, behaviourProperty);
                            behaviourProperty.isExpanded = EditorGUI.Foldout(new Rect(x, y, position.width - 100f, stepY), behaviourProperty.isExpanded, behaviourContent, true);
                            EditorGUI.EndProperty();

                            GUI.enabled = !property.isInstantiatedPrefab;
                            if (GUI.Button(new Rect(position.x + position.width - stepX, y, stepY, stepY), "x"))
                            {
                                DelBehaviour_Set(i);
                            }
                            GUI.enabled = true;


                            if (behaviourProperty.isExpanded)
                            {
                                foreach (SerializedProperty fieldProperty in behaviourProperty)
                                {
                                    if (fieldProperty.depth == 3)
                                    {
                                        EditorGUI.PropertyField(new Rect(x + stepX, y + stepY, position.width - (stepX * 3f), stepY), fieldProperty, true);
                                        y += EditorGUI.GetPropertyHeight(fieldProperty);
                                    }
                                }

                                y += stepY;
                                var maskContent = new GUIContent("Attached States");
                                maskContent = EditorGUI.BeginProperty(new Rect(x + stepX, y, position.width - (stepX * 3f), stepY), maskContent, maskProperty);
                                maskProperty.intValue = EditorGUI.MaskField(new Rect(x + stepX, y, position.width - (stepX * 3f), stepY), maskContent, maskProperty.intValue, StateInfo_GetNames());
                                EditorGUI.EndProperty();

                                DrawEndLine(ref x, ref y, stepY, stepX, position);
                            }
                        }

                        y += stepY * 2;
                        GUI.enabled = !property.isInstantiatedPrefab;
                        if (GUI.Button(new Rect(x, y, position.width - (stepX * 2f), stepY), "Add Behaviour"))
                        {
                            var dropdown = new BehaviourChoosePopup(new AdvancedDropdownState());
                            dropdown.Show(new Rect(Event.current.mousePosition.x - 150f, Event.current.mousePosition.y, 300f, stepY));
                            dropdown.onChoose += (type) => { AddBehaviour(type); };
                        }
                        GUI.enabled = true;

                        x -= stepX;

                        DrawEndLine(ref x, ref y, stepY, stepX, position);
                    }


                    y += stepY;
                    stateInfo_StatesFoldout = EditorGUI.Foldout(new Rect(x, y, position.width, stepY), stateInfo_StatesFoldout, "States", true, EditorStyles.foldoutHeader);
                    if (stateInfo_StatesFoldout)
                    {
                        var stateNames = StateInfo_GetNames();

                        x += stepX;

                        for (int i = 0; i < stateNames.Length; i++)
                        {
                            y += stepY;
                            StateInfo_SetFoldout(
                                EditorGUI.Foldout(new Rect(x, y, position.width, stepY), StateInfo_GetFoldout(i), stateNames[i], true,
                                    controller.currentState == i && Application.isPlaying ? _styles_ActiveState : EditorStyles.foldout
                                ), i
                            );


                            if (StateInfo_GetFoldout(i))
                            {
                                x += stepX;

                                for (int j = 0; j < behaviours.arraySize; j++)
                                {
                                    int mask = masks.GetArrayElementAtIndex(j).intValue;

                                    if (FlagsHelper.IsSet(mask, FlagsHelper.GetFlag(i)))
                                    {
                                        var behaviourType = controller._behaviours[j].GetType();

                                        y += stepY;
                                        EditorGUI.LabelField(new Rect(x, y, position.width, stepY), $"{behaviourType.Name} ({j})");

                                    }
                                }
                                x -= stepX;

                                DrawEndLine(ref x, ref y, stepY, stepX, position);
                            }
                        }

                        x -= stepX;

                        DrawEndLine(ref x, ref y, stepY, stepX, position);
                    }

                    GUI.enabled = false;
                    EditorGUI.PropertyField(new Rect(x, y + stepY, position.width, stepY), behaviours, new GUIContent("Debug Show Behaviours"), true);
                    y += EditorGUI.GetPropertyHeight(behaviours);
                    EditorGUI.PropertyField(new Rect(x, y + stepY, position.width, stepY), masks, new GUIContent("Debug Show Masks"), true);
                    y += EditorGUI.GetPropertyHeight(masks);
                    GUI.enabled = true;
                }

                SetEndHeight(y + stepY);
                AddBehaviourUpdate(behaviours, masks);
                Database_Gui2();
                DelBehaviour_Gui(behaviours, masks);
            }

            #region Styles
            GUIStyle _styles_ActiveState;
            GUIStyle _styles_maskOverride;

            void Styles_Gui()
            {
                if (_styles_ActiveState == null)
                {
                    _styles_ActiveState = new GUIStyle(EditorStyles.foldout);
                    _styles_ActiveState.normal.textColor = new Color(0.2f, 0.6f, 0.2f);
                }

                if (_styles_maskOverride == null)
                {
                    _styles_maskOverride = new GUIStyle(EditorStyles.popup);
                    _styles_maskOverride.fontStyle = FontStyle.Bold;
                }
            }
            #endregion

            #region Drawers
            void DrawEndLine(ref float x, ref float y, float stepY, float stepX, in Rect position)
            {
                y += stepY + 2f;
                EditorGUI.DrawRect(new Rect(x, y, position.width - x + 15f, 1f), new Color(0.3f, 0.3f, 0.3f));
                y -= (stepY - 2f);
            }
            #endregion

            #region Height
            float _heightA, _heightB;

            void SetStartHeight(float v)
            {
                _heightA = v;
            }

            void SetEndHeight(float v)
            {
                _heightB = v;
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return Mathf.Abs(_heightA - _heightB);
            }
            #endregion

            #region Add Behaviour
            System.Type _addBehaviourType;

            void AddBehaviour(System.Type type)
            {
                _addBehaviourType = type;
            }

            void AddBehaviourUpdate(SerializedProperty behaviours, SerializedProperty masks)
            {
                if (_addBehaviourType != null)
                {
                    behaviours.arraySize++;
                    var newProperty = behaviours.GetArrayElementAtIndex(behaviours.arraySize - 1);
                    newProperty.managedReferenceValue = System.Activator.CreateInstance(_addBehaviourType);

                    masks.arraySize = behaviours.arraySize;
                    masks.GetArrayElementAtIndex(masks.arraySize - 1).intValue = 0;

                    _addBehaviourType = null;
                }
            }
            #endregion

            #region Del Behaviour
            int _delBehaviour_Id = -1;

            void DelBehaviour_Set(int v)
            {
                _delBehaviour_Id = v;
            }

            void DelBehaviour_Gui(SerializedProperty behaviours, SerializedProperty masks)
            {
                if (_delBehaviour_Id != -1)
                {
                    if (_delBehaviour_Id >= 0 && _delBehaviour_Id < behaviours.arraySize)
                    {
                        behaviours.DeleteArrayElementAtIndex(_delBehaviour_Id);
                        masks.DeleteArrayElementAtIndex(_delBehaviour_Id);
                    }

                    _delBehaviour_Id = -1;
                }
            }
            #endregion

            #region Database
            SerializedObject _statesInfo_Object;
            SerializedProperty _statesInfo_StatesProperty;
            string[] _statesInfo_TypesName;

            void Database_Gui()
            {
                if (_statesInfo_Object == null)
                {
                    var asset = AssetDatabase.LoadAssetAtPath("Assets/Bibyter/Behaviour/Editor/Database.asset", typeof(ScriptableObject));

                    if (asset == null)
                    {
                        Debug.LogError("Asset not found");
                    }
                    else
                    {
                        _statesInfo_Object = new SerializedObject(asset);
                        _statesInfo_StatesProperty = _statesInfo_Object.FindProperty("_states");
                    }
                }
            }

            void Database_Gui2()
            {
                if (_statesInfo_Object != null && _statesInfo_Object.hasModifiedProperties)
                    _statesInfo_Object.ApplyModifiedProperties();
            }

            string[] Database_GetTypesName()
            {
                if (_statesInfo_TypesName != null)
                {
                    return _statesInfo_TypesName;
                }

                _statesInfo_TypesName = new string[_statesInfo_StatesProperty.arraySize];

                for (int i = 0; i < _statesInfo_StatesProperty.arraySize; i++)
                {
                    _statesInfo_TypesName[i] = _statesInfo_StatesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("typeName").stringValue;
                }
                return _statesInfo_TypesName;
            }

            SerializedProperty Database_GetStatesInfoProperty(int id)
            {
                return _statesInfo_StatesProperty.GetArrayElementAtIndex(id);
            }

            SerializedProperty Database_GetOrAddStatesInfoProperty(string name)
            {
                for (int i = 0; i < _statesInfo_StatesProperty.arraySize; i++)
                {
                    if (_statesInfo_StatesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("typeName").stringValue == name)
                    {
                        return _statesInfo_StatesProperty.GetArrayElementAtIndex(i);
                    }
                }

                _statesInfo_StatesProperty.arraySize++;
                var newStatesInfo = _statesInfo_StatesProperty.GetArrayElementAtIndex(_statesInfo_StatesProperty.arraySize - 1);
                newStatesInfo.FindPropertyRelative("typeName").stringValue = name;
                newStatesInfo.FindPropertyRelative("foldoutValues").arraySize = 32;

                newStatesInfo.FindPropertyRelative("stateNames").arraySize = 4;
                newStatesInfo.FindPropertyRelative("stateNames").GetArrayElementAtIndex(0).stringValue = "Name 1";
                newStatesInfo.FindPropertyRelative("stateNames").GetArrayElementAtIndex(1).stringValue = "Name 2";
                newStatesInfo.FindPropertyRelative("stateNames").GetArrayElementAtIndex(2).stringValue = "Name 3";
                newStatesInfo.FindPropertyRelative("stateNames").GetArrayElementAtIndex(3).stringValue = "Name 4";

                return newStatesInfo;
            }
            #endregion

            #region StateInfo
            SerializedProperty _foldoutProperty;
            SerializedProperty _stateInfo_StatesFoldout;
            SerializedProperty _stateInfo_BehavioursFoldout;
            SerializedProperty _stateInfo_StatesNameProperty;
            string[] _stateInfo_StateNames;
            bool _stateInfo_isInit;

            void StateInfo_Gui(SerializedProperty property)
            {
                if (!_stateInfo_isInit)
                {
                    var name = $"{property.serializedObject.targetObject.GetType().FullName}.{property.name}";
                    var stateProperty = Database_GetOrAddStatesInfoProperty(name);

                    _foldoutProperty = stateProperty.FindPropertyRelative("foldoutValues");
                    _stateInfo_StatesFoldout = stateProperty.FindPropertyRelative("statesFoldout");
                    _stateInfo_StatesNameProperty = stateProperty.FindPropertyRelative("stateNames");
                    _stateInfo_BehavioursFoldout = stateProperty.FindPropertyRelative("behavioursFoldout");
                    StateInfo_StatesNameUpdate();

                    _stateInfo_isInit = true;
                }
            }

            void StateInfo_StatesNameUpdate()
            {
                _stateInfo_StateNames = new string[_stateInfo_StatesNameProperty.arraySize];
                for (int i = 0; i < _stateInfo_StateNames.Length; i++)
                {
                    _stateInfo_StateNames[i] = _stateInfo_StatesNameProperty.GetArrayElementAtIndex(i).stringValue;
                }
            }

            string[] StateInfo_GetNames()
            {
                return _stateInfo_StateNames;
            }

            bool StateInfo_GetFoldout(int i)
            {
                if (_foldoutProperty == null)
                {
                    return false;
                }

                return _foldoutProperty.GetArrayElementAtIndex(i).boolValue;
            }

            void StateInfo_SetFoldout(bool v, int i)
            {
                if (_foldoutProperty != null)
                {
                    _foldoutProperty.GetArrayElementAtIndex(i).boolValue = v;
                }
            }

            bool stateInfo_StatesFoldout
            {
                set
                {
                    if (_stateInfo_StatesFoldout != null) _stateInfo_StatesFoldout.boolValue = value;
                }
                get
                {
                    if (_stateInfo_StatesFoldout != null) return _stateInfo_StatesFoldout.boolValue;
                    return false;
                }
            }

            bool stateInfo_BehavioursFoldout
            {
                set { if (_stateInfo_BehavioursFoldout != null) _stateInfo_BehavioursFoldout.boolValue = value; }
                get { if (_stateInfo_BehavioursFoldout != null) { return _stateInfo_BehavioursFoldout.boolValue; } return false; }
            }
            #endregion
        }

        public class BehaviourChoosePopup : AdvancedDropdown
        {
            public event System.Action<System.Type> onChoose;


            public BehaviourChoosePopup(AdvancedDropdownState state) : base(state)
            {
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                var root = new AdvancedDropdownItem("Weekdays");

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
                var sourceType = typeof(Bibyter.Behaviour);

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

        sealed class BehaviourReorderableList
        {
            UnityEditorInternal.ReorderableList list;

            BehaviourReorderableList(SerializedProperty behaviours, StateBehaviourController controller)
            {
            }



        }
#endif
    }

    public static class FlagsHelper
    {
        public static int GetFlag(int id)
        {
            return 1 << id;
        }

        public static bool IsSet(int flags, int flag)
        {
            return (flags & flag) != 0;
        }

        public static void Set(ref int mask, int flag)
        {
            mask = mask | flag;
        }

        public static void Unset(ref int mask, int flag)
        {
            mask = mask & (~flag);
        }
    }
}

