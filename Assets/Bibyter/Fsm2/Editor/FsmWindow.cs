using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Bibyter.Fsm2
{
    public class FsmWindow : EditorWindow
    {
        NodeWindow _nodeWindow;
        State _stateAsset;
        bool _isInit;

        [OnOpenAsset(1)]
        public static bool OnOnenAssetSubscriber(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);

            if (obj is State)
            {
                var window = EditorWindow.GetWindow<FsmWindow>();
                window.titleContent = new GUIContent("Fsm Window");
                window.Show();
                window.Init(obj as State);
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            _isInit = false;

            Undo.undoRedoPerformed += UndoRedoPerformed;

            _nodeWindow = new NodeWindow();
            _nodeWindow.onSelectedNode += OnNodeSelected;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            _nodeWindow.onSelectedNode -= OnNodeSelected;
        }

        public void Init(State stateAsset)
        {
            if (stateAsset == null)
            {
                Close();
                return;
            }

            _nodeWindow.Init();

            NodeFabric_Init();
            StateHierarchy_Init(stateAsset);
            _stateAsset = stateAsset;

            NextFrameAction_Init();
        }

        void OnGUI()
        {
            if (_stateAsset == null)
            {
                Close();
                return;
            }

            // for recompile errors fix on open the window
            if (!_isInit)
            {
                Init(_stateAsset);
                _isInit = true;
            }

            NextFrameAction_Run();

            _nodeWindow.Draw(Event.current, position);

            GUI.matrix = Matrix4x4.identity;
            var e = Event.current;

            if (StateHierarchy_CanBack())
            {
                if (GUI.Button(new Rect(2, 20, 70, EditorGUIUtility.singleLineHeight), "Back", EditorStyles.miniButton))
                {
                    StateHierarchy_Back();
                    e.Use();
                }
            }

            if (e.type == EventType.MouseDown)
            {
                if (e.button == 0 && e.clickCount == 2)
                {
                    var localMousePosition = _nodeWindow.GetLocalMousePosition();
                    var clickNode = _nodeWindow.GetNode(localMousePosition);

                    if (clickNode != null)
                    {
                        StateHierarchy_Open((clickNode as StateNode).state);
                    }
                }

                if (e.button == 1)
                {
                    var localMousePosition = _nodeWindow.GetLocalMousePosition();
                    var clickNode = _nodeWindow.GetNode(localMousePosition);

                    if (clickNode == null)
                    {
                        NextFrameAction_Add(() =>
                        {
                            var menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Create Node"), false, () => { CreateState(localMousePosition); });
                            menu.ShowAsContext();
                        });

                        GUI.changed = true;
                    }
                    else
                    {
                        NextFrameAction_Add(() =>
                        {
                            var menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Delete Node"), false, () => { DeleteState(clickNode); });

                            if (!(clickNode as StateNode).IsEntry())
                            {
                                menu.AddItem(new GUIContent("Set Entry"), false, () => { SetEntryState(clickNode); });
                            }

                            menu.ShowAsContext();
                        });

                        GUI.changed = true;
                    }
                }
            }

            

            GUI.Label(new Rect(2, 2, position.width, EditorGUIUtility.singleLineHeight), StateHierarchy_StackToString());
        }

        void OnNodeSelected(Node node)
        {
            if (node != null)
            {
                var stateNode = node as StateNode;
                Selection.activeObject = stateNode.state;
            }
            else
            {
                if (Selection.activeObject is State)
                {
                    Selection.activeObject = null;
                }
            }
        }

        void UndoRedoPerformed()
        {
            Repaint();
        }

        #region Hierarchy

        Stack<State> _stateHierarchy;

        void StateHierarchy_Init(State rootState)
        {
            _stateHierarchy = new Stack<State>();
            StateHierarchy_Open(rootState);
        }

        void StateHierarchy_Open(State state)
        {
            _stateHierarchy.Push(state);

            _nodeWindow.NodesClear();

            for (int i = 0; i < state.GetChildStatesCount(); i++)
            {
                _nodeWindow.AddNode(NodeFabric_Create(state.GetChildState(i), StateHierarchy_GetCurrent()));
            }
        }

        bool StateHierarchy_CanBack()
        {
            return _stateHierarchy.Count >= 2;
        }

        void StateHierarchy_Back()
        {
            if (StateHierarchy_CanBack())
            {
                _stateHierarchy.Pop();

                var currentState = _stateHierarchy.Peek();

                _nodeWindow.NodesClear();

                for (int i = 0; i < currentState.GetChildStatesCount(); i++)
                {
                    _nodeWindow.AddNode(NodeFabric_Create(currentState.GetChildState(i), StateHierarchy_GetCurrent()));
                }
            }
        }

        State StateHierarchy_GetCurrent()
        {
            return _stateHierarchy.Peek();
        }

        string StateHierarchy_StackToString()
        {
            var builder = new System.Text.StringBuilder(256);
            foreach (var state in _stateHierarchy)
            {
                builder.Insert(0, '/');
                builder.Insert(0, state.name);
            }

            return builder.ToString();
        }
        #endregion


        void CreateState(Vector2 position)
        {
            var newState = ScriptableObject.CreateInstance<State>();
            newState.name = "New State";
            newState.position = position;
            newState.hideFlags = HideFlags.HideInHierarchy;

            Selection.activeObject = newState;

            AssetDatabase.AddObjectToAsset(newState, _stateAsset);
            EditorUtility.SetDirty(_stateAsset);
            AssetDatabase.SaveAssetIfDirty(_stateAsset);

            StateHierarchy_GetCurrent().AddChildState(newState);
            _nodeWindow.AddNode(NodeFabric_Create(newState, StateHierarchy_GetCurrent()));
        }

        void DeleteState(Node node)
        {
            var stateClickNode = node as StateNode;
            var deletedState = stateClickNode.state;

            StateHierarchy_GetCurrent().DelChildState(deletedState);
            AssetDatabase.RemoveObjectFromAsset(deletedState);

            _nodeWindow.DeleteNode(node);
            Object.DestroyImmediate(deletedState);

            EditorUtility.SetDirty(_stateAsset);
            AssetDatabase.SaveAssetIfDirty(_stateAsset);
        }

        void SetEntryState(Node node)
        {
            var parentState = StateHierarchy_GetCurrent();
            parentState.ChildStatesFlip((node as StateNode).state, parentState.GetChildState(0));

            EditorUtility.SetDirty(_stateAsset);
            AssetDatabase.SaveAssetIfDirty(_stateAsset);
        }

        #region Node

        GUIStyle _nodeFabric_Idle;
        GUIStyle _nodeFabric_Selected;
        GUIStyle _nodeFabric_EntryIdle;
        GUIStyle _nodeFabric_EntrySelected;

        void NodeFabric_Init()
        {
            _nodeFabric_Idle = NodeFabric_CreateNodeStyle("builtin skins/lightskin/images/node0.png");
            _nodeFabric_Selected = NodeFabric_CreateNodeStyle("builtin skins/lightskin/images/node0 on.png");
            _nodeFabric_EntryIdle = NodeFabric_CreateNodeStyle("builtin skins/lightskin/images/node3.png");
            _nodeFabric_EntrySelected = NodeFabric_CreateNodeStyle("builtin skins/lightskin/images/node3 on.png");
        }

        StateNode NodeFabric_Create(State state, State parent)
        {
            var n = new StateNode(parent, state);
            n.SetStyles(_nodeFabric_Idle, _nodeFabric_Selected, _nodeFabric_EntryIdle, _nodeFabric_EntrySelected);
            return n;
        }

        GUIStyle NodeFabric_CreateNodeStyle(string texturePath)
        {
            var style = new GUIStyle();
            style.normal.background = EditorGUIUtility.Load(texturePath) as Texture2D;
            style.border = new RectOffset(12, 12, 12, 12);
            style.padding = new RectOffset(14, 14, 14, 14);
            style.fontSize = 13;
            style.alignment = TextAnchor.UpperCenter;
            return style;
        }

        #endregion

        #region
        List<System.Action> _nextFrameActions;

        void NextFrameAction_Init()
        {
            _nextFrameActions = new List<System.Action>();
        }

        void NextFrameAction_Add(System.Action action)
        {
            _nextFrameActions.Add(action);
        }

        void NextFrameAction_Run()
        {
            if (Event.current.type != EventType.Repaint) return;

            for (int i = 0; i < _nextFrameActions.Count; i++)
            {
                var action = _nextFrameActions[i];
                action?.Invoke();
            }

            _nextFrameActions.Clear();
        }
        #endregion
    }

    public sealed class StateNode : Node
    {
        State _parent;
        State _state;

        bool _isSelected;

        GUIStyle _idleStyle;
        GUIStyle _selectedStyle;
        GUIStyle _entryIdleStyle;
        GUIStyle _entrySelectedStyle;


        public State state => _state;



        public StateNode(State parent, State state) : base()
        {
            _parent = parent;
            _state = state;
        }

        public void SetStyles(GUIStyle idleStyle, GUIStyle selectedStyle, GUIStyle entryIdleStyle, GUIStyle entrySelectedStyle)
        {
            _idleStyle = idleStyle;
            _selectedStyle = selectedStyle;
            _entryIdleStyle = entryIdleStyle;
            _entrySelectedStyle = entrySelectedStyle;
        }

        public override void Awake()
        {
            _isSelected = false;
        }

        public override void Draw()
        {
            var style = _idleStyle;

            if (IsEntry())
            {
                style = _isSelected ? _entrySelectedStyle : _entryIdleStyle;
            }
            else
            {
                style = _isSelected ? _selectedStyle : _idleStyle;
            }

            GUI.Box(new Rect(Vector2.zero, new Vector2(214f, 54f)), _state.name, style);
        }

        public override void OnSelected()
        {
            _isSelected = true;
        }

        public override void OnUnselected()
        {
            _isSelected = false;
        }


        public override void OnDragBegin()
        {
        }

        public override void OnDragEnd(Vector2 from, Vector2 to)
        {
            _state.position = from;
            Undo.RecordObject(_state, "State Drag");
            _state.position = to;
        }

        public override Vector2 position
        {
            get => _state.position;
            set => _state.position = value;
        }

        public bool IsEntry()
        {
            return _parent.GetChildStatesCount() > 0 && _parent.GetChildState(0) == _state;
        }
    }

}
