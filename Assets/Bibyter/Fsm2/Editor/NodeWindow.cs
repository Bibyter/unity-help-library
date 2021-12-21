using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Bibyter.Fsm2
{
    public sealed class NodeWindow
    {
        MyMatrix _matrix;
        public MyMatrix spaceMatrix
        {
            get { return _matrix; }
            set { _matrix = value; }
        }

        DragSpaceBehaviour _dragSpaceBehaviour;

        public void Init()
        {
            NodesInit();
            _dragSpaceBehaviour = new DragSpaceBehaviour(this);
            _matrix = MyMatrix.Create();
        }

        public void Draw(Event e, Rect position)
        {
            NodesOnEvent(e);
            _dragSpaceBehaviour.OnEvent(e);

            if (e.type == EventType.Repaint)
            {
                DrawBackground(_matrix, position);
                NodesDraw();
            }
        }

        #region
        private void DrawBackground(MyMatrix matrix, Rect position)
        {
            EditorGUI.DrawRect(new Rect(0f, 0f, position.width, position.height), new Color(0.364f, 0.364f, 0.364f, 1f));
            DrawGrid(10f, Mathf.Lerp(0.0f, 0.1f, (matrix.scale - 0.1f)), Color.black, position, matrix);
            DrawGrid(100f, Mathf.Lerp(0.13f, 0.2f, (matrix.scale - 0.1f)), Color.black, position, matrix);
        }

        private static void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor, Rect position, MyMatrix matrix)
        {
            var windowHeight = position.height;
            var windowWidth = position.width;
            var scaledSpacing = gridSpacing * matrix.scale;
            var widthDivs = Mathf.CeilToInt(windowWidth / scaledSpacing);
            var heightDivs = Mathf.CeilToInt(windowHeight / scaledSpacing);

            var offset = new Vector2(matrix.position.x % scaledSpacing, matrix.position.y % scaledSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            for (int i = 0; i < widthDivs; i++)
            {
                var pointA = new Vector2((scaledSpacing * i) + offset.x, 0f);
                var pointB = new Vector2((scaledSpacing * i) + offset.x, windowHeight);
                Handles.DrawLine(pointA, pointB);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                var pointA = new Vector2(0f, (scaledSpacing * j) + offset.y);
                var pointB = new Vector2(windowWidth, (scaledSpacing * j) + offset.y);
                Handles.DrawLine(pointA, pointB);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }
        #endregion
        #region
        List<Node> _nodes;
        Node _draggedNode;
        Vector2 _draggedNodeStartPosition;

        Vector2 _dragAmount;
        const float _gridSpacing = 10f;

        void NodesInit()
        {
            _nodes = new List<Node>(16);
            _draggedNode = null;
        }

        void NodesDraw()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].InternalDraw(ref _matrix);
            }
        }

        void NodesOnEvent(Event e)
        {
            var localMousePoint = GetLocalMousePosition();
            var localDelta = _matrix.Inverse().ApplyDirection(e.delta);

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        var clickNode = GetNode(localMousePoint);

                        if (clickNode != null)
                        {
                            _draggedNode = clickNode;
                            _draggedNodeStartPosition = _draggedNode.position;
                            _draggedNode.OnDragBegin();
                            _dragAmount = Vector2.zero;

                            SelectedNode_Set(clickNode);
                            GUI.changed = true;
                        }
                        else
                        {
                            SelectedNode_Set(null);
                            GUI.changed = true;
                        }
                    }
                    else if (e.button == 1)
                    {
                        var clickNode = GetNode(localMousePoint);

                        if (clickNode != null)
                        {
                            SelectedNode_Set(clickNode);
                            GUI.changed = true;
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    if (_draggedNode != null)
                    {
                        _dragAmount += localDelta;

                        var offset = Mathf.FloorToInt(Mathf.Abs(_dragAmount.x) / _gridSpacing);
                        if (offset >= 1)
                        {
                            _draggedNode.Move(new Vector2(Mathf.Sign(_dragAmount.x) * _gridSpacing * offset, 0f));
                            _dragAmount.x += -Mathf.Sign(_dragAmount.x) * _gridSpacing * offset;
                            GUI.changed = true;
                        }

                        offset = Mathf.FloorToInt(Mathf.Abs(_dragAmount.y) / _gridSpacing);
                        if (offset >= 1)
                        {
                            _draggedNode.Move(new Vector2(0f, Mathf.Sign(_dragAmount.y) * _gridSpacing * offset));
                            _dragAmount.y += -Mathf.Sign(_dragAmount.y) * _gridSpacing * offset;
                            GUI.changed = true;
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (_draggedNode != null)
                    {
                        _draggedNode.OnDragEnd(_draggedNodeStartPosition, _draggedNode.position);
                        _draggedNode = null;
                    }
                    break;

                case EventType.ScrollWheel:
                    {
                        const float minScale = 0.1f;
                        const float maxScale = 3f;
                        const float scaleStep = 0.1f;
                        var scrollWheelValue = e.delta.y < 0f ? 1f : -1f;
                        var delta = Mathf.Clamp((scrollWheelValue * scaleStep) + _matrix.scale, minScale, maxScale) - _matrix.scale;
                        _matrix.position -= localMousePoint * delta;
                        _matrix.scale += delta;
                        GUI.changed = true;
                    }
                    break;
            }
        }

       
        public Node GetNode(Vector2 point)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].rect.Contains(point))
                {
                    return _nodes[i];
                }
            }

            return null;
        }

        public void AddNode(Node node)
        {
            _nodes.Add(node);
            node.Awake();
        }

        public void DeleteNode(Node node)
        {
            _nodes.Remove(node);
            SelectedNode_OnDeleteNode(node);
        }

        public void NodesClear()
        {
            while(_nodes.Count > 0)
            {
                DeleteNode(_nodes[0]);
            }
        }

       
        #endregion
        #region
        public event System.Action<Node> onSelectedNode;
        public Node selectedNode => _selectedNode;

        Node _selectedNode;
        void SelectedNode_Set(Node node)
        {
            if (_selectedNode != node)
            {
                if (_selectedNode != null)
                {
                    _selectedNode.OnUnselected();
                    _selectedNode = null;
                }

                if (node != null)
                {
                    // set higher sorting order
                    var lastNode = _nodes[_nodes.Count - 1];
                    _nodes[_nodes.Count - 1] = node;
                    _nodes[_nodes.IndexOf(node)] = lastNode;

                    _selectedNode = node;
                    _selectedNode.OnSelected();
                }

                onSelectedNode?.Invoke(_selectedNode);
            }
        }

        void SelectedNode_OnDeleteNode(Node node)
        {
            if (_selectedNode == node)
            {
                _selectedNode = null;
            }
        }
        #endregion

        public Vector2 GetLocalMousePosition()
        {
            return _matrix.Inverse().ApplyPoint(Event.current.mousePosition);
        }

    }

    public sealed class DragSpaceBehaviour
    {
        NodeWindow _window;
        bool _isActive;

        public DragSpaceBehaviour(NodeWindow fsmWindow)
        {
            _window = fsmWindow;
            _isActive = false;
        }

        public void OnEvent(Event e)
        {
            if (e.type == EventType.MouseDown && e.button == 2)
            {
                _isActive = true;
            }

            if (_isActive && e.type == EventType.MouseDrag)
            {
                var matrix = _window.spaceMatrix;

                matrix.position += e.delta;
                _window.spaceMatrix = matrix;
                e.Use();
            }

            if (e.type == EventType.MouseUp)
            {
                _isActive = false;
            }
        }

    }

    public class Node
    {
        static readonly Vector2 TabWindowCompensation = new Vector2(1f, 19f);
        static readonly Vector2 BoxDrawCullingCompensation = new Vector2(7f, 6f);
        
        Vector2 _position;
        public virtual Vector2 position
        {
            set { _position = value; }
            get { return _position; }
        }

        public Rect rect
        {
            get { return new Rect(position, new Vector2(200f, 40f)); }
        }

        public Node()
        {
        }

        public void InternalDraw(ref MyMatrix matrix)
        {
            GUI.matrix = Matrix4x4.TRS(TabWindowCompensation + matrix.ApplyPoint(position - BoxDrawCullingCompensation - TabWindowCompensation), Quaternion.identity, new Vector3(matrix.scale, matrix.scale, matrix.scale));

            Draw();

            //GUI.matrix = Matrix4x4.identity;
            //Handles.BeginGUI();
            //Handles.color = Color.cyan;
            //Handles.DrawLine(matrix.ApplyPoint(_rect.min), matrix.ApplyPoint(_rect.max));
            //Handles.EndGUI();
        }

        public virtual void Draw()
        { }

        public virtual void OnSelected()
        { }

        public virtual void OnUnselected()
        { }

        public virtual void OnDragBegin()
        { }

        public virtual void OnDragEnd(Vector2 from, Vector2 to)
        { }

        public virtual void Awake()
        { }

        public void Move(Vector2 delta)
        {
            position += delta;
        }
    }

    public class NodeWindopDropdowm : AdvancedDropdown
    {
        public event System.Action onStateCreate;


        public NodeWindopDropdowm(AdvancedDropdownState state) : base(state)
        { }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Weekdays");

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            onStateCreate?.Invoke();
        }
    }

    public struct MyMatrix
    {
        public Vector2 axisX, axisY, position;

        public float scale
        {
            set
            {
                axisX = axisX.normalized * value;
                axisY = axisY.normalized * value;
            }
            get
            {
                return axisX.magnitude;
            }
        }

        public Vector2 ApplyDirection(Vector2 d)
        {
            return (d.x * axisX) + (d.y * axisY);
        }

        public Vector2 ApplyPoint(Vector2 p)
        {
            return (p.x * axisX) + (p.y * axisY) + position;
        }

        public float det
        {
            get
            {
                return axisX.x * axisY.y - axisX.y * axisY.x;
            }
        }

        public MyMatrix Inverse()
        {
            var m = new MyMatrix();
            var mul = 1f / det;

            m.axisY.y = axisX.x * mul;
            m.axisX.y = -axisX.y * mul;

            m.axisY.x = -axisY.x * mul;
            m.axisX.x = axisY.y * mul;

            m.position.x = (-position.x) / scale;
            m.position.y = (-position.y) / scale;

            return m;
        }

        public static MyMatrix Create()
        {
            var m = new MyMatrix();
            m.axisX = Vector2.right;
            m.axisY = Vector2.up;
            m.position = Vector2.zero;
            return m;
        }
    }
}