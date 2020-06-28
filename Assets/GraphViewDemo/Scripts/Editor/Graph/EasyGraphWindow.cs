using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AillieoUtils.GraphViewDemo.Editor
{
    public class EasyGraphWindow : EditorWindow
    {
        private EasyGraphView _graphView;
        private MiniMap _miniMap;
        private Toolbar _toolbar;

        private EasyGraphAsset _asset;

        public string AssetGuid { get; private set; }

        [MenuItem("Window/GraphViewDemo")]
        public static void CreateGraphViewWindow()
        {
            var window = GetWindow<EasyGraphWindow>();
            window.titleContent = new GUIContent("GraphViewDemo");
        }

        private void CreateGraphView()
        {
            _graphView = new EasyGraphView(this)
            {
                name = "GraphViewDemo",
            };
            _graphView.StretchToParentSize();
            _graphView.deleteSelection += DeleteSelectionImplementation;

            rootVisualElement.Add(_graphView);
        }

        private void CreateToolbar()
        {
            _toolbar = new Toolbar();
            StyleSheetUtils.AddStyleSheets(_toolbar, "Toolbar");

            _toolbar.Add(new Button(() => SaveLoadUtils.SaveGraphView(_graphView, AssetGuid)) { text = "Save" });

            _toolbar.Add(new Button(() => SaveLoadUtils.LoadGraphViewAsset(_graphView)) {text = "Load"});

            var toggleMiniMap = new Toggle("MiniMap");
            toggleMiniMap.RegisterValueChangedCallback((change) => {
                _miniMap.visible = change.newValue;
            });
            _miniMap.visible = toggleMiniMap.value;
            _toolbar.Add(toggleMiniMap);

            rootVisualElement.Add(_toolbar);
        }

        private void DeleteSelectionImplementation(string operationName, GraphView.AskUser askUser)
        {
            GraphElement[] selected = _graphView.selection.Where(e => e is GraphElement).Cast<GraphElement>().ToArray();
 
            foreach(var sel in selected)
            {
                switch(sel)
                {
                    case BaseNode node:
                    case Edge edge:
                        sel.RemoveFromHierarchy();
                        break;
                    case Group group:
                        var children = group.Children().ToArray();
                        foreach (var n in children)
                        {
                            n.RemoveFromHierarchy();
                            group.parent.Add(n);
                        }
                        group.RemoveFromHierarchy();
                        break;
                }
            }
            _graphView.selection.Clear();
        }

        public void Initialize(string guid)
        {
            AssetGuid = guid;
            string path = AssetDatabase.GUIDToAssetPath(guid);

            EasyGraphAsset easyGraphAsset = AssetDatabase.LoadAssetAtPath<EasyGraphAsset>(path);
            if (easyGraphAsset == null)
            {
                Debug.LogError("load failed");
                return;
            }

            SaveLoadUtils.LoadGraphViewAsset(easyGraphAsset, _graphView);
            Repaint();
        }

        private void OnEnable()
        {
            CleanUp();
            CreateGraphView();
            CreateMiniMap();
            CreateToolbar();
        }

        private void CreateMiniMap()
        {
            if(_miniMap != null)
            {
                _graphView.Remove(_miniMap);
                _miniMap = null;
            }
            _miniMap = new MiniMap();
            var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
            _miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
            _graphView.Add(_miniMap);
        }

        private void CleanUp()
        {
            if (_graphView != null)
            {
                _graphView.deleteSelection -= DeleteSelectionImplementation;
                rootVisualElement.Remove(_graphView);
                _graphView = null;
            }
            if (_toolbar != null)
            {
                rootVisualElement.Remove(_toolbar);
                _toolbar = null;
            }
            _miniMap = null;
        }

        private void OnDisable()
        {
            CleanUp();
        }
    }
}
