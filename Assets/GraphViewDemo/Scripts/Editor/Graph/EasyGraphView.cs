using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AillieoUtils.GraphViewDemo.Editor
{
    public class EasyGraphView : GraphView
    {
        public static Vector2 CommentGroupSize => new Vector2(300, 200);

        private SearchWindowProvider _searchWindow;

        private EasyGraphView()
        {
            StyleSheetUtils.AddStyleSheets(this, "GraphView");
        }

        public EasyGraphView(EasyGraphWindow editorWindow):this()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddSearchWindow(editorWindow);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());
        }


        private void AddSearchWindow(EasyGraphWindow editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<SearchWindowProvider>();
            _searchWindow.Initialize(editorWindow, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }

        public Group CreateCommentGroup(Rect rect, CommentGroupData CommentGroupData = null)
        {
            if (CommentGroupData == null)
                CommentGroupData = new CommentGroupData();
            var group = new Group
            {
                autoUpdateGeometry = true,
                title = CommentGroupData.title
            };
            AddElement(group);
            group.SetPosition(rect);
            return group;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().FindAll(port=> startPort != port && startPort.node != port.node);
        }

        public BaseNode CreateAndAddNode<T>(Vector2 position) where T : BaseNode, new()
        {
            var node = BaseNode.CreateNode<T>();

            node.SetPosition(new Rect(position, node.Size));
            AddElement(node);
            return node;
        }

        public BaseNode LoadAndAddNode(CommonNodeData commonNodeData)
        {
            var node = BaseNode.LoadNode(commonNodeData);

            node.SetPosition(new Rect(commonNodeData.position, node.Size));
            AddElement(node);
            return node;
        }
    }
}
