using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AillieoUtils.GraphViewDemo.Editor
{
    public class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private EditorWindow _window;
        private EasyGraphView _graphView;

        private Texture2D _icon;

        public void Initialize(EditorWindow window, EasyGraphView graphView)
        {
            _window = window;
            _graphView = graphView;


            _icon = new Texture2D(1,1);
            _icon.SetPixel(0,0,new Color(0,0,0,0));
            _icon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            return new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
                new SearchTreeGroupEntry(new GUIContent("Instruction"), 1),
                new SearchTreeEntry(new GUIContent("Trigger Node", _icon))
                {
                    level = 2, userData = new TriggerNode()
                },
                new SearchTreeEntry(new GUIContent("Condition Node", _icon))
                {
                    level = 2, userData = new ConditionNode()
                },
                new SearchTreeEntry(new GUIContent("Action Node", _icon))
                {
                    level = 2, userData = new ActionNode()
                },
                new SearchTreeEntry(new GUIContent("Comment Group",_icon))
                {
                    level = 1,
                    userData = new Group()
                }
            };
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
                context.screenMousePosition - _window.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);
            switch (SearchTreeEntry.userData)
            {
            case Group group:
                var rect = new Rect(graphMousePosition, EasyGraphView.CommentGroupSize);
                _graphView.CreateCommentGroup(rect);
                return true;

            case TriggerNode triggerNode:
                _graphView.CreateAndAddNode<TriggerNode>(graphMousePosition);
                return true;
            case ConditionNode conditionNode:
                _graphView.CreateAndAddNode<ConditionNode>(graphMousePosition);
                return true;
            case ActionNode actionNode:
                _graphView.CreateAndAddNode<ActionNode>(graphMousePosition);
                return true;
            }
            return false;
        }

        void OnDestroy()
        {
            if (_icon != null)
            {
                DestroyImmediate(_icon);
                _icon = null;
            }
        }
    }
}
